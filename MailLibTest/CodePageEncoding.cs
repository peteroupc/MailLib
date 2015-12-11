using PeterO;
using PeterO.Text;
using System;
using System.Text;

namespace MailLibTest {
    /// <summary>A character encoding class that implements a code page
    /// read from the code page file format described in the Windows
    /// Protocols Unicode Reference
    /// (https://msdn.microsoft.com/en-us/library/cc248954.aspx), section
    /// 2.2.2.1. The code page file format supports single-byte encodings
    /// and certain multi-byte encodings in which each character is encoded
    /// in one or two bytes.
    /// <para>The code page format defines a single-byte as a replacement
    /// character and can specify certain "best-fit" mappings from certain
    /// Unicode characters to the code page encoding if the Unicode
    /// character is unsupported in the code page encoding. When decoding,
    /// any invalid bytes or unassigned bytes in the code page encoding are
    /// converted to the given replacement code point.</para></summary>
  public class CodePageEncoding : ICharacterEncoding {
    CodePageCoder coder;

    /// <summary>Gets the code page's number.</summary>
    /// <value>The code page&apos;s number.</value>
    public int Number {
      get {
        return coder.Number;
      }
    }

    public CodePageEncoding(ICharacterInput input) {
      this.coder = new CodePageCoder(input);
    }

    public ICharacterDecoder GetDecoder() {
      return new CodePageCoder(this.coder);
    }

    public ICharacterEncoder GetEncoder() {
      return new CodePageCoder(this.coder);
    }

    private sealed class CodePageCoder : ICharacterEncoder, ICharacterDecoder {
      private sealed class InputWithUnget : ICharacterInput {
        private readonly ICharacterInput transform;
        private int lastByte;
        private bool unget;

        public InputWithUnget(ICharacterInput stream) {
          this.lastByte = -1;
          this.transform = stream;
        }

        public int ReadChar() {
          if (this.unget) {
            this.unget = false;
          } else {
            this.lastByte = this.transform.ReadChar();
          }
          return this.lastByte;
        }

        public void Unget() {
          this.unget = true;
        }

        public int Read(int[] chars, int index, int length) {
          if (length == 0) {
            return 0;
          }
          var count = 0;
          for (var i = 0; i < length; ++i) {
            int c = this.ReadChar();
            if (c < 0) {
              break;
            }
            chars[index] = c;
            ++index;
          }
          return (count == 0) ? -1 : count;
        }
      }

      public static int ParseNumber(string word) {
        if (word.Length > 2 && word[0] == '0' && word[1] == 'x') {
          var value = 0;
          var index = 2;
          while (index < word.Length) {
            char c = word[index];
            ++index;
            if (c >= '0' && c <= '9') {
              value <<= 4;
              value |= (c - '0');
            } else if (c >= 'a' && c <= 'f') {
              value <<= 4;
              value |= ((c - 'a') + 10);
            } else if (c >= 'A' && c <= 'F') {
              value <<= 4;
              value |= ((c - 'f') + 10);
            } else {
              return -1;
            }
          }
          return value;
        }
        if (word.Length > 0 && word[0] >= '0' && word[0] <= '9') {
          var value = 0;
          var index = 0;
          while (index < word.Length) {
            char c = word[index];
            ++index;
            if (c >= '0' && c <= '9') {
              value *= 10;
              value += (c - '0');
            } else {
              return -1;
            }
          }
          return value;
        }
        return -1;
      }

      public int Encode(int c, IWriter output) {
        if (c < 0) {
          return -1;
        }
        int mapping = ucsToBytes.GetMapping(c);
        if (mapping < 0) {
          // Encode the default byte for this code page
          output.WriteByte((byte)(this.defaultNative & 0xff));
          return 1;
        }
        var ret = 1;
        if (mapping >= 0x100) {
          output.WriteByte((byte)((mapping >> 8) & 0xff));
          ret = 2;
        }
        output.WriteByte((byte)(mapping & 0xff));
        return ret;
      }

      private int lastByte = -1;
      private bool unget = false;

      public int ReadChar(IByteReader input) {
        int b1 = unget ? lastByte : input.ReadByte();
        unget = false;
        if (b1 < 0) {
          return -1;
        }
        int b = bytesToUCS[b1];
        if (b == -2) {
          return this.defaultUCS;
        } else if (b == -3) {
          int b2 = input.ReadByte();
          if (b2 < 0) {
            return this.defaultUCS;
          }
          int ret = dbcsToUCS.GetMapping((b1 << 8) | b2);
          if (ret == -2) {
            unget = true;
            lastByte = b2;
            return this.defaultUCS;
          } else {
            return ret;
          }
        } else {
          return b;
        }
      }

      private enum TokenType {
        Word,

        Number,

        LineBreak,

        End }

      private sealed class TokenReader {
        private int number;
        private string word;
        private TokenType type;
        private InputWithUnget input;
        public TokenReader(ICharacterInput ci) {
          this.input = new InputWithUnget(ci);
        }
        public bool IsWord(string str) {
          return type == TokenType.Word && word.Equals(str);
        }
        public void SkipToLine() {
          while (true) {
            ReadToken();
            if (type == TokenType.LineBreak || type == TokenType.End) {
              return;
            }
          }
        }
        public bool IsNumber() {
          return type == TokenType.Number;
        }
        public int ExpectNumberOnSameLine() {
          ReadToken();
          if (type != TokenType.Number) {
            throw new ArgumentException("number expected");
          }
          return this.number;
        }
        public int ExpectNumber() {
          do {
            ReadToken();
          } while (type == TokenType.LineBreak);
          if (type != TokenType.Number) {
            throw new ArgumentException("number expected");
          }
          return this.number;
        }
        public int ExpectByte() {
          int number = ExpectNumber();
          if (number >= 256) {
      throw new ArgumentException("expected number from 0-255, got " +
              number);
          }
          return number;
        }
        public int ExpectByteOnSameLine() {
          int number = ExpectNumberOnSameLine();
          if (number >= 256) {
      throw new ArgumentException("expected number from 0-255, got " +
              number);
          }
          return number;
        }
        public int ExpectUInt16OnSameLine() {
          int number = ExpectNumberOnSameLine();
          if (number >= 65536) {
    throw new ArgumentException("expected number from 0-65536, got " +
              number);
          }
          return number;
        }
        public int ExpectCodePointOnSameLine() {
          int number = ExpectNumberOnSameLine();
          if (number >= 0x110000) {
 throw new ArgumentException("expected number from 0-0x10ffff, got " +
              number);
          }
          return number;
        }
        public int ExpectCodePoint() {
          int number = ExpectNumber();
          if (number >= 0x110000) {
 throw new ArgumentException("expected number from 0-0x10ffff, got " +
              number);
          }
          return number;
        }
        public string ExpectWord() {
          do {
            ReadToken();
          } while (type == TokenType.LineBreak);
          if (type != TokenType.Word) {
            throw new ArgumentException("word expected");
          }
          return word;
        }
        public void ExpectSpecificWord(string word) {
          do {
            ReadToken();
          } while (type == TokenType.LineBreak);
          if (type != TokenType.Word && !word.Equals(this.word)) {
            throw new ArgumentException("word '" + word + "' expected, got '" +
              this.word + "'");
          }
        }
        private void ReadToken() {
          while (true) {
            int c = input.ReadChar();
            if (c == 0x0a) {
              this.type = TokenType.LineBreak;
              break;
            } else if (c == 0x0d) {
              c = input.ReadChar();
              if (c == 0x0a) {
                this.type = TokenType.LineBreak;
              } else {
                input.Unget();
              }
              break;
            } else if (c == -1) {
              this.type = TokenType.End;
              break;
            } else if (c == 0x20 || c == 0x09) {
              continue;  // whitespace
            } else if (c == (int)';') {
              // comment
              while (true) {
                c = input.ReadChar();
                if (c == -1 || c == 0x0d || c == 0x0a) {
                  input.Unget();
                  break;
                }
              }
              continue;
            } else {
              // Word
              var sb = new StringBuilder();
              if (c <= 0xffff) {
                sb.Append((char)(c));
              } else if (c <= 0x10ffff) {
                sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
                sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
              }
              while (true) {
                c = input.ReadChar();
                if (c == -1 || c == 0x0d || c == 0x0a ||
                  c == 0x09 || c == 0x20 || c == (int)';') {
                  input.Unget();
                  break;
                } else {
                  if (c <= 0xffff) {
                    sb.Append((char)(c));
                  } else if (c <= 0x10ffff) {
                    sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
                    sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
                  }
                }
              }
              string word = sb.ToString();
              int number = ParseNumber(word);
              if (number >= 0) {
                this.number = number;
                this.type = TokenType.Number;
              } else {
                this.word = word;
                this.type = TokenType.Word;
              }
              break;
            }
          }
        }
      }

      private int[] bytesToUCS;
      private UCSMapping dbcsToUCS;
      private UCSMapping ucsToBytes;
      private int codepageNumber;
      private int defaultNative;
      private int defaultUCS;

      private sealed class UCSMapping {
        private int[] array;
        public UCSMapping() {
          array = new int[256];
          for (var i = 0; i < array.Length; ++i) {
            array[i] = -2;
          }
        }
        public int GetMapping(int ucs) {
    return (ucs < 0 || ucs > array.Length) ? (-2) :
            (array[ucs]);
        }
        public void AddMapping(int ucs, int value) {
          if (ucs >= array.Length) {
            int[] newarray = null;
            if (ucs >= 0x30000) {
              newarray = new int[Math.Max(ucs + 0x1000, 0x110000)];
            } else if (ucs >= 0x10000) {
              newarray = new int[0x30000];
            } else if (ucs >= 0x3000) {
              newarray = new int[0x10000];
            } else if (ucs >= 0x100) {
              newarray = new int[0x3000];
            }
          Array.Copy(array, 0, newarray, 0,
              array.Length);
            for (var i = array.Length; i < newarray.Length; ++i) {
              newarray[i] = -2;
            }
            array = newarray;
          }
          array[ucs] = value;
        }
      }

      public int Number {
        get {
          return codepageNumber;
        }
      }

      public CodePageCoder(CodePageCoder other) {
        this.bytesToUCS = other.bytesToUCS;
        this.dbcsToUCS = other.dbcsToUCS;
        this.lastByte = -1;
        this.unget = false;
        this.codepageNumber = other.codepageNumber;
        this.defaultNative = other.defaultNative;
        this.defaultUCS = other.defaultUCS;
        this.ucsToBytes = other.ucsToBytes;
      }

      public CodePageCoder(ICharacterInput input) {
        var token = new TokenReader(input);
        var state = 0;
        var byteCount = 0;
        this.defaultNative = 0;
        this.defaultUCS = 0;
        var lineCount = 0;
        var ranges = 0;
        var rangeLow = 0;
        var rangeHigh = 0;
        var done = false;
        var haveMbTable = false;
        var haveWcTable = false;
        dbcsToUCS = new UCSMapping();
        ucsToBytes = new UCSMapping();
        bytesToUCS = new int[256];
        for (var i = 0; i < 256; ++i) {
          bytesToUCS[i] = -2;
        }
        while (!done) {
          switch (state) {
            case 0: {
                token.ExpectSpecificWord("CODEPAGE");
                this.codepageNumber = token.ExpectNumberOnSameLine();
                token.SkipToLine();
                token.ExpectSpecificWord("CPINFO");
                byteCount = token.ExpectNumberOnSameLine();
                if (byteCount != 1 && byteCount != 2) {
                  throw new ArgumentException("Expected byte count 1 or 2");
                }
                defaultNative = token.ExpectByteOnSameLine();
                defaultUCS = token.ExpectCodePointOnSameLine();
                token.SkipToLine();
                state = 1;
              }
              break;
            case 1: {
                string word = token.ExpectWord();
                if (word.Equals("MBTABLE")) {
                  lineCount = token.ExpectNumberOnSameLine();
                  token.SkipToLine();
                  state = 2;
                  haveMbTable = true;
                } else if (word.Equals("DBCSRANGE")) {
                  ranges = token.ExpectNumberOnSameLine();
                  if (ranges == 0) {
                    throw new ArgumentException("ranges is 0");
                  }
                  token.SkipToLine();
                  state = 4;
                } else if (word.Equals("WCTABLE")) {
                  lineCount = token.ExpectNumberOnSameLine();
                  token.SkipToLine();
                  state = 3;
                  haveWcTable = true;
                } else if (word.Equals("GLYPHTABLE")) {
                  // Alternate characters for some bytes, for
                  // display purposes.
                  lineCount = token.ExpectNumberOnSameLine();
                  token.SkipToLine();
                  state = 5;
                } else if (word.Equals("ENDCODEPAGE") && haveMbTable &&
                    haveWcTable) {
                  done = true;
                } else {
                  throw new ArgumentException("Unexpected word: " + word);
                }
              }
              break;
            case 2: {
                for (int i = 0; i < lineCount; ++i) {
                  int nativeValue = token.ExpectByte();
                  int ucs = token.ExpectCodePointOnSameLine();
                  bytesToUCS[nativeValue] = ucs;
                  token.SkipToLine();
                }
                state = 1;
              }
              break;
            case 3: {
                for (int i = 0; i < lineCount; ++i) {
                  int ucs = token.ExpectCodePoint();
                  int nativeValue = (byteCount == 1) ?
                token.ExpectByteOnSameLine() : token.ExpectUInt16OnSameLine();
                  ucsToBytes.AddMapping(ucs, nativeValue);
                  token.SkipToLine();
                }
                state = 1;
              }
              break;
            case 4: {
                rangeLow = token.ExpectByte();
                rangeHigh = token.ExpectByteOnSameLine();
                if (rangeLow > rangeHigh) {
                  throw new ArgumentException("invalid range");
                }
                token.SkipToLine();
                for (int i = rangeLow; i <= rangeHigh; ++i) {
                  bytesToUCS[i] = -3;
                  token.ExpectSpecificWord("DBCSTABLE");
                  lineCount = token.ExpectNumberOnSameLine();
                  token.SkipToLine();
                  int range = i << 8;
                  for (int j = 0; j < lineCount; ++j) {
                    int nativeValue = token.ExpectByte() | range;
                    int ucs = token.ExpectCodePointOnSameLine();
                    dbcsToUCS.AddMapping(nativeValue, ucs);
                    token.SkipToLine();
                  }
                }
                --ranges;
                if (ranges <= 0) {
                  state = 1;
                }
              }
              break;
            case 5: {
                // Ignore glyph table.
                // NOTE: This table, if implemented, would replace
                // the appropriate entries in the WCTABLE.
                for (int i = 0; i < lineCount; ++i) {
                  token.ExpectByte();
                  token.ExpectCodePointOnSameLine();
                  token.SkipToLine();
                }
                state = 1;
              }
              break;
          }
        }
      }
    }
  }
}
