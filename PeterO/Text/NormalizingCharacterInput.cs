/*
  Written by Peter O. in 2014.
  Any copyright is dedicated to the Public Domain.
  http://creativecommons.org/publicdomain/zero/1.0/
  If you like this, you should donate to Peter O.
  at: http://peteroupc.github.io/
   */
  using System;
  using System.Collections.Generic;

  namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.NormalizingCharacterInput"]/*'/>
    [Obsolete("Renamed to NormalizerInput.")]
    public sealed class NormalizingCharacterInput : ICharacterInput {
      private readonly ICharacterInput nci;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String)"]/*'/>
      public NormalizingCharacterInput(
    string str) : this(
    str,
    Normalization.NFC) {
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(PeterO.Text.ICharacterInput)"]/*'/>
      public NormalizingCharacterInput(
    ICharacterInput input) : this(
    input,
    Normalization.NFC) {
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.Collections.Generic.IList{System.Int32})"]/*'/>
        public NormalizingCharacterInput(IList<int> characterList) :
        this(characterList, Normalization.NFC) {
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.Collections.Generic.IList{System.Int32},PeterO.Text.Normalization)"]/*'/>
        public NormalizingCharacterInput(
    IList<int> characterList,
    Normalization form) :
    this(new PartialListCharacterInput(characterList), form) {
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String,System.Int32,System.Int32,PeterO.Text.Normalization)"]/*'/>
      public NormalizingCharacterInput(
    string str,
    int index,
    int length,
    Normalization form) {
       this.nci = new NormalizerInput(str, index, length, form);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(System.String,PeterO.Text.Normalization)"]/*'/>
      public NormalizingCharacterInput(string str, Normalization form) {
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizingCharacterInput.#ctor(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
      public NormalizingCharacterInput(
     ICharacterInput stream,
     Normalization form) {
       this.nci = new NormalizerInput(stream, form);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.IsNormalized(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(
  ICharacterInput chars,
  Normalization form) {
      return NormalizerInput.IsNormalized(chars, form);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.Normalize(System.String,PeterO.Text.Normalization)"]/*'/>
      public static string Normalize(string str, Normalization form) {
       return NormalizerInput.Normalize(str, form);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.IsNormalized(System.String,PeterO.Text.Normalization)"]/*'/>
      public static bool IsNormalized(string str, Normalization form) {
        return NormalizerInput.IsNormalized(str, form);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.GetChars(System.String,PeterO.Text.Normalization)"]/*'/>
  [Obsolete("Instead of this method, create a NormalizerInput on the string and call ReadChar to get the normalized string's code points.")]
      public static IList<int> GetChars(string str, Normalization form) {
        if (str == null) {
          throw new ArgumentNullException("str");
        }
        IList<int> ret = new List<int>();
        int ch;
        var input = new NormalizingCharacterInput(str, form);
        while ((ch = input.ReadChar()) >= 0) {
           ret.Add(ch);
        }
        return ret;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.GetChars(PeterO.Text.ICharacterInput,PeterO.Text.Normalization)"]/*'/>
  [Obsolete("Instead of this method, create a NormalizerInput on the input and call ReadChar to get the normalized string's code points.")]
   public static IList<int> GetChars(
  ICharacterInput input,
  Normalization form) {
        if (input == null) {
          throw new ArgumentNullException("input");
        }
        IList<int> ret = new List<int>();
        int ch;
        input = new NormalizingCharacterInput(input, form);
        while ((ch = input.ReadChar()) >= 0) {
           ret.Add(ch);
        }
        return ret;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.IsNormalized(System.Int32[],PeterO.Text.Normalization)"]/*'/>
  [Obsolete("Either convert the array to a string or wrap it in an ICharacterInput and call the corresponding overload instead.")]
    public static bool IsNormalized(int[] charArray, Normalization form) {
     if (charArray == null) {
      throw new ArgumentNullException("charArray");
     }
     return IsNormalized(new PartialArrayCharacterInput(charArray), form);
   }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.IsNormalized(System.Collections.Generic.IList{System.Int32},PeterO.Text.Normalization)"]/*'/>
  [Obsolete("Either convert the list to a string or wrap it in an ICharacterInput and call the corresponding overload instead.")]
      public static bool IsNormalized(IList<int> charList, Normalization form) {
          return IsNormalized(
  new PartialListCharacterInput(charList),
  form);
     }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.ReadChar"]/*'/>
      public int ReadChar() {
        return this.nci.ReadChar();
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.NormalizerInput.Read(System.Int32[],System.Int32,System.Int32)"]/*'/>
      public int Read(int[] chars, int index, int length) {
        return this.nci.Read(chars, index, length);
      }

      private sealed class PartialArrayCharacterInput : ICharacterInput {
        private readonly int endPos;
        private readonly int[] array;
        private int pos;

        public PartialArrayCharacterInput(int[] array, int start, int length) {
          this.array = array;
          this.pos = start;
          this.endPos = start + length;
        }

        public PartialArrayCharacterInput(int[] array) {
          this.array = array;
          this.pos = 0;
          this.endPos = array.Length;
        }

        public int ReadChar() {
          return (this.pos < this.endPos) ? this.array[this.pos++] : (-1);
        }

        public int Read(int[] buf, int offset, int unitCount) {
          if (unitCount == 0) {
            return 0;
          }
          int maxsize = Math.Min(unitCount, this.endPos - this.pos);
          Array.Copy(this.array, this.pos, buf, offset, maxsize);
          this.pos += maxsize;
          return maxsize == 0 ? -1 : maxsize;
        }
      }
    }
  }
