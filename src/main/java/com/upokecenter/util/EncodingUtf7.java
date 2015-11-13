package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

 class EncodingUtf7 implements ICharacterReader {
      static final int[] Alphabet = { -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
          -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

      DecoderState state;
      int alphavalue = 0;
      int base64value = 0;
      int base64count = 0;
   CodeUnitAppender appender;
      // 0: not in base64; 1: start of base 64; 2: continuing base64
      int machineState = 0;
      public EncodingUtf7 (ITransform transform) {
        this.state = new DecoderState(transform, 4);
        this.appender = new CodeUnitAppender();
      }

      private static final class CodeUnitAppender {
        private int surrogate;
        private int lastByte;

        public CodeUnitAppender () {
          this.surrogate = -1;
          this.lastByte = -1;
        }

        public void FinalizeAndReset(DecoderState state) {
          if (this.surrogate >= 0 && this.lastByte >= 0) {
            // Unpaired surrogate and an unpaired byte value
            state.AppendChar(0xfffd);
            state.AppendChar(0xfffd);
          } else if (this.surrogate >= 0 || this.lastByte >= 0) {
            // Unpaired surrogate or byte value remains
            state.AppendChar(0xfffd);
          }
          this.surrogate = -1;
          this.lastByte = -1;
        }

        public void AppendIncompleteByte() {
          // Make sure lastByte isn't -1, for FinalizeAndReset
          // purposes
          this.lastByte = 0;
        }

        public void AppendByte(int value, DecoderState state) {
          if (this.lastByte >= 0) {
            int codeunit = this.lastByte << 8;
            codeunit |= value & 0xff;
            this.AppendCodeUnit(codeunit, state);
            this.lastByte = -1;
          } else {
            this.lastByte = value;
          }
        }

        private void AppendCodeUnit(int codeunit, DecoderState state) {
          if (this.surrogate >= 0) {
            // If we have a surrogate, "codeunit"
            // must be a valid "low surrogate" to complete the pair
            if ((codeunit & 0xfc00) == 0xdc00) {
              // valid low surrogate
              state.AppendChar((char)this.surrogate);
              state.AppendChar((char)codeunit);
              this.surrogate = -1;
            } else if ((codeunit & 0xfc00) == 0xd800) {
              // unpaired high surrogate
              state.AppendChar((char)0xfffd);
              this.surrogate = codeunit;
            } else {
              // not a surrogate, output the first as U + FFFD
              // and the second as is
              state.AppendChar((char)0xfffd);
              state.AppendChar((char)codeunit);
              this.surrogate = -1;
            }
          } else {
            if ((codeunit & 0xfc00) == 0xdc00) {
              // unpaired low surrogate
              state.AppendChar((char)0xfffd);
            } else if ((codeunit & 0xfc00) == 0xd800) {
              // valid high surrogate
              this.surrogate = codeunit;
            } else {
              // not a surrogate
              state.AppendChar((char)codeunit);
            }
          }
        }

        public void Reset() {
          this.surrogate = -1;
          this.lastByte = -1;
        }
      }

      public int ReadChar() {
        int ch = state.GetChar();
        if (ch >= 0) {
 return ch;
}
        while (true) {
          int b;
          switch (machineState) {
            case 0:
               // not in base64
              b = state.read();
              if (b < 0) {
                // done
                return -1;
              }
              if (b == 0x09 || b == 0x0a || b == 0x0d) {
                return b;
              } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                // Illegal byte in UTF-7
                return 0xfffd;
              } else if (b == 0x2b) {
                // plus sign
                machineState = 1;  // change state to "start of base64"
                base64value = 0;
                base64count = 0;
                appender.Reset();
              } else {
                return b;
              }
              break;
            case 1:  // start of base64
              b = state.read();
              if (b < 0) {
                // End of stream, illegal
                machineState = 0;
                return 0xfffd;
              }
              if (b == 0x2d) {
                // hyphen, so output a plus sign
                machineState = 0;
                state.AppendChar('+');
                ch = state.GetChar();
                if (ch >= 0) {
 return ch;
}
              } else if (b >= 0x80) {
                // Non-ASCII byte, illegal
                machineState = 0;
                state.AppendChar((char)0xfffd);  // for the illegal plus
             state.AppendChar((char)0xfffd);  // for the illegal non-ASCII byte
                ch = state.GetChar();
                if (ch >= 0) {
 return ch;
}
              } else {
                alphavalue = Alphabet[b];
                if (alphavalue >= 0) {
                  machineState = 2;  // change state to "continuing base64"
                  base64value <<= 6;
                  base64value |= alphavalue;
                  ++base64count;
                } else {
                  // Non-base64 byte (NOTE: Can't be plus or
                  // minus at this point)
                  machineState = 0;
                  state.AppendChar((char)0xfffd);  // for the illegal plus
                  if (b == 0x09 || b == 0x0a || b == 0x0d) {
                    state.AppendChar((char)b);
                  } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                    // Illegal byte in UTF-7
                    state.AppendChar((char)0xfffd);
                  } else {
                    state.AppendChar((char)b);
                  }
                  ch = state.GetChar();
                  if (ch >= 0) {
 return ch;
}
                }
              }
              break;
            case 2:
              // continuing base64
              b = state.read();
              alphavalue = (b < 0 || b >= 0x80) ? -1 : Alphabet[b];
              if (alphavalue >= 0) {
                // Base64 alphabet (except padding)
                base64value <<= 6;
                base64value |= alphavalue;
                ++base64count;
                if (base64count == 4) {
                  // Generate UTF-16 bytes
                  appender.AppendByte((base64value >> 16) & 0xff, state);
                  appender.AppendByte((base64value >> 8) & 0xff, state);
                  appender.AppendByte(base64value & 0xff, state);
                  base64count = 0;
                }
              } else {
                machineState = 0;
                 switch (base64count) {
                    case 1: {
                    // incomplete base64 byte
                    appender.AppendIncompleteByte();
                    break;
                    }
                    case 2: {
                    base64value <<= 12;
                    appender.AppendByte((base64value >> 16) & 0xff, state);
                    if ((base64value & 0xffff) != 0) {
                    // Redundant pad bits
                    appender.AppendIncompleteByte();
                    }
                    break;
                    }
                    case 3: {
                    base64value <<= 6;
                    appender.AppendByte((base64value >> 16) & 0xff, state);
                    appender.AppendByte((base64value >> 8) & 0xff, state);
                    if ((base64value & 0xff) != 0) {
                    // Redundant pad bits
                    appender.AppendIncompleteByte();
                    }
                    break;
                    }
                }
                appender.FinalizeAndReset(state);
                if (b < 0) {
                  // End of stream
                  ch = state.GetChar();
                  return (ch >= 0) ? (ch) : (-1);
                }
                if (b == 0x2d) {
                  // Ignore the hyphen
                } else if (b == 0x09 || b == 0x0a || b == 0x0d) {
                  state.AppendChar((char)b);
                } else if (b == 0x5c || b >= 0x7e || b < 0x20) {
                  // Illegal byte in UTF-7
                  state.AppendChar((char)0xfffd);
                } else {
                  state.AppendChar((char)b);
                }
              }
              ch = state.GetChar();
              if (ch >= 0) {
 return ch;
}
              break;
            default: throw new IllegalStateException("Unexpected state");
          }
        }
      }
  }
