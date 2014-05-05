package com.upokecenter.mail;

  final class HeaderParser {
private HeaderParser() {
}
    public static int ParseAddrSpec(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseLocalPart(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        tx2 = ParseDomain(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAddress(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseGroup(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAddressList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        int tx2 = ParseAddress(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexTemp4 = ParseAddress(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAngleAddr(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseObsRoute(str, index, endIndex, tokener);
        int tx2 = ParseAddrSpec(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAtext(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 33) || (str.charAt(index) == 35) || (str.charAt(index) == 36) || (str.charAt(index) == 37) || (str.charAt(index) == 38) || (str.charAt(index) == 39) || (str.charAt(index) == 42) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) == 47) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) == 94) || (str.charAt(index) == 95) || (str.charAt(index) == 96) || (str.charAt(index) == 123) || (str.charAt(index) == 124) || (str.charAt(index) == 125) || (str.charAt(index) == 126) || (str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
          ++indexTemp; break;
        }
        if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
          indexTemp += 2; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseAtom(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAuthresVersion(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
        } else {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseAuthservId(String str, int index, int endIndex, ITokener tokener) {
      return ParseValue(str, index, endIndex, tokener);
    }

    public static int ParseCFWS(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          for (int i2 = 0;; ++i2) {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              index = ParseFWS(str, index, endIndex, tokener);
              int tx4 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
              if (tx4 == index) {
                index = indexStart3; break;
              } else {
                index = tx4;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        for (int i = 0;; ++i) {
          indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        index = indexStart;
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseCertifierList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseDomainName(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 58)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseDomainName(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseCharset(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
            ++index;
          }
        } else {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseDate(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseDay(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        if (index + 2 < endIndex && (((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 78) || ((str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 66) || ((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 82) || ((str.charAt(index) & ~32) == 65 && (str.charAt(index + 1) & ~32) == 80 && (str.charAt(index + 2) & ~32) == 82) || ((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 89) || ((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 78) || ((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 76) || ((str.charAt(index) & ~32) == 65 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 71) || ((str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 80) || ((str.charAt(index) & ~32) == 79 && (str.charAt(index + 1) & ~32) == 67 && (str.charAt(index + 2) & ~32) == 84) || ((str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 86) || ((str.charAt(index) & ~32) == 68 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 67))) {
          index += 3;
        } else {
          index = indexStart; break;
        }
        tx2 = ParseYear(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDateTime(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = ParseDayOfWeek(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        int tx2 = ParseDate(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        tx2 = ParseTime(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDay(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 2; ++i) {
          if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          } else if (i < 1) {
            index = indexStart; break;
          } else {
            break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDayOfWeek(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 2 < endIndex && (((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 78) || ((str.charAt(index) & ~32) == 84 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 69) || ((str.charAt(index) & ~32) == 87 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 68) || ((str.charAt(index) & ~32) == 84 && (str.charAt(index + 1) & ~32) == 72 && (str.charAt(index + 2) & ~32) == 85) || ((str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 73) || ((str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 84) || ((str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 78))) {
          index += 3;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDesignator(String str, int index, int endIndex, ITokener tokener) {
      return ParseMilitaryString(str, index, endIndex, tokener);
    }

    public static int ParseDispNotParam(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        if (index + 8 < endIndex && (str.charAt(index) == 61) && (((str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 69 && (str.charAt(index + 3) & ~32) == 81 && (str.charAt(index + 4) & ~32) == 85 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 82 && (str.charAt(index + 7) & ~32) == 69 && (str.charAt(index + 8) & ~32) == 68) || ((str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 80 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 73 && (str.charAt(index + 5) & ~32) == 79 && (str.charAt(index + 6) & ~32) == 78 && (str.charAt(index + 7) & ~32) == 65 && (str.charAt(index + 8) & ~32) == 76))) {
          index += 9;
        } else {
          index = indexStart; break;
        }
        if (index < endIndex && (str.charAt(index) == 44)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        int tx2 = ParseValue(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseValue(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDisplayName(String str, int index, int endIndex, ITokener tokener) {
      return ParsePhrase(str, index, endIndex, tokener);
    }

    public static int ParseDomain(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          int tx3 = ParseAtom(str, index, endIndex, tokener);
          if (tx3 == index) {
            break;
          } else {
            index = tx3;
          }
          while (true) {
            int indexTemp3;
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 46)) {
                ++index;
              } else {
                break;
              }
              int tx4 = ParseAtom(str, index, endIndex, tokener);
              if (tx4 == index) {
                index = indexStart3; break;
              } else {
                index = tx4;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else if (tokener != null) {
              tokener.RestoreState(state3); break;
            } else {
              break;
            }
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(9, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseDomainLiteral(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 91)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseDtext(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 93)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDomainName(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57))) {
          ++index;
        } else {
          break;
        }
        index = ParseLdhStr(str, index, endIndex, tokener);
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && (str.charAt(index) == 46) && ((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
              index += 2;
            } else {
              break;
            }
            index = ParseLdhStr(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDomainNoCfws(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) == 91)) {
            ++index;
          } else {
            break;
          }
          while (true) {
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                ++indexTemp3; break;
              }
              int indexTemp4 = index;
              do {
                if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                  ++indexTemp4; break;
                }
                if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                  indexTemp4 += 2; break;
                }
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              break;
            }
          }
          if (index < endIndex && (str.charAt(index) == 93)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDotAtom(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDotAtomText(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 46)) {
              ++index;
            } else {
              break;
            }
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseAtext(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseDtext(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 1 && str.charAt(index) <= 8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) == 127))) {
          ++indexTemp; break;
        }
        int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
          ++indexTemp; break;
        }
        if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
          indexTemp += 2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseEncodingCount(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseEncodingKeyword(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
          ++index;
        } else {
          index = indexStart; break;
        }
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseFWS(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && str.charAt(index) == 13 && str.charAt(index + 1) == 10) {
              index += 2;
            }
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseGeneralKeyword(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseGroup(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseDisplayName(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseGroupList(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(5, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseGroupList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseObsGroupList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          for (int i2 = 0;; ++i2) {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              index = ParseFWS(str, index, endIndex, tokener);
              int tx4 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
              if (tx4 == index) {
                index = indexStart3; break;
              } else {
                index = tx4;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        for (int i = 0;; ++i) {
          indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        index = indexStart;
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAcceptLanguage(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          int tx3 = ParseLanguageQ(str, index, endIndex, tokener);
          if (tx3 == index) {
            index = indexStart2; break;
          } else {
            index = tx3;
          }
          while (true) {
            int indexTemp3;
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 44)) {
                ++index;
              } else {
                break;
              }
              index = ParseCFWS(str, index, endIndex, tokener);
              int tx4 = ParseLanguageQ(str, index, endIndex, tokener);
              if (tx4 == index) {
                index = indexStart3; break;
              } else {
                index = tx4;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else if (tokener != null) {
              tokener.RestoreState(state3); break;
            } else {
              break;
            }
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseObsAcceptLanguage(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAlternateRecipient(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderArchivedAt(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAuthenticationResults(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseAuthservId(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = ParseCFWS(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            tx3 = ParseAuthresVersion(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParseNoResult(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int indexStart2 = index;
            for (int i2 = 0;; ++i2) {
              indexTemp3 = ParseResinfo(str, index, endIndex, tokener);
              if (indexTemp3 == index) { if (i2 < 1) {
                  indexTemp2 = indexStart2;
                } break;
              } else {
                index = indexTemp3;
              }
            }
            index = indexStart2;
            if (indexTemp3 != indexStart2) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAutoSubmitted(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2 = ParseOptParameterList(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderAutoforwarded(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderAutosubmitted(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderBcc(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParseAddressList(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentBase(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentDisposition(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseParameter(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentDuration(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 10; ++i) {
          if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          } else if (i < 1) {
            index = indexStart; break;
          } else {
            break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentId(String str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderContentLanguage(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseLanguageList(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentLocation(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 59) || (str.charAt(index) == 61) || (str.charAt(index) >= 63 && str.charAt(index) <= 126))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentMd5(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 47 && str.charAt(index) <= 57) || (str.charAt(index) == 43) || (str.charAt(index) == 61))) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentTransferEncoding(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderContentType(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseRestrictedName(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 47)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        tx2 = ParseRestrictedName(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseParameter(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderConversion(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderConversionWithLoss(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDate(String str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDeferredDelivery(String str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDeliveryDate(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDateTime(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          int tx3 = ParseDayOfWeek(str, index, endIndex, tokener);
          if (tx3 == index) {
            index = indexStart2; break;
          } else {
            index = tx3;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index + 2 < endIndex && (((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 78) || ((str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 66) || ((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 82) || ((str.charAt(index) & ~32) == 65 && (str.charAt(index + 1) & ~32) == 80 && (str.charAt(index + 2) & ~32) == 82) || ((str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 89) || ((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 78) || ((str.charAt(index) & ~32) == 74 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 76) || ((str.charAt(index) & ~32) == 65 && (str.charAt(index + 1) & ~32) == 85 && (str.charAt(index + 2) & ~32) == 71) || ((str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 80) || ((str.charAt(index) & ~32) == 79 && (str.charAt(index + 1) & ~32) == 67 && (str.charAt(index + 2) & ~32) == 84) || ((str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 86) || ((str.charAt(index) & ~32) == 68 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 67))) {
            index += 3;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          for (int i2 = 0; i2 < 2; ++i2) {
            if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
              ++index;
            } else if (i2 < 1) {
              index = indexStart2; break;
            } else {
              break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index + 2 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57)) && (str.charAt(index + 2) == 58)) {
            index += 3;
          } else {
            index = indexStart2; break;
          }
          if (index + 2 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57)) && (str.charAt(index + 2) == 58)) {
            index += 3;
          } else {
            index = indexStart2; break;
          }
          if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
            index += 2;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          for (int i2 = 0;; ++i2) {
            if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
              ++index;
            } else if (i2 < 4) {
              index = indexStart2; break;
            } else {
              break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderDiscloseRecipients(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDispositionNotificationOptions(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseDispNotParam(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseDispNotParam(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderDispositionNotificationTo(String str, int index, int endIndex, ITokener tokener) {
      return ParseMailboxList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderDkimSignature(String str, int index, int endIndex, ITokener tokener) {
      return ParseNoEncodedWords(str, index, endIndex, tokener);
    }

    public static int ParseHeaderEdiintFeatures(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          ++index;
        }
        if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
              ++index;
            }
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
              ++index;
            }
            if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 45))) {
              ++index;
              while (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 45))) {
                ++index;
              }
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseHeaderEncoding(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = ParseEncodingCount(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseEncodingKeyword(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseEncodingCount(str, index, endIndex, tokener);
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseEncodingKeyword(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderEncrypted(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseWord(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseWord(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderFrom(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddressList(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderGenerateDeliveryReport(String str, int index, int endIndex, ITokener tokener) {
      return ParseFWS(str, index, endIndex, tokener);
    }

    public static int ParseHeaderImportance(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderInReplyTo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexTemp3 = ParsePhrase(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseMsgId(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderIncompleteCopy(String str, int index, int endIndex, ITokener tokener) {
      return ParseFWS(str, index, endIndex, tokener);
    }

    public static int ParseHeaderJabberId(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) == 32)) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          ++index;
        }
        int tx2 = ParsePathxmpp(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderKeywords(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParsePhrase(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParsePhrase(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderLanguage(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && (((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122)) && ((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122)))) {
              index += 2;
            } else {
              break;
            }
            while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
              ++index;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && (str.charAt(index) == 40)) {
                  ++index;
                } else {
                  break;
                }
                int tx4 = ParseLanguageDescription(str, index, endIndex, tokener);
                if (tx4 == index) {
                  index = indexStart3; break;
                } else {
                  index = tx4;
                }
                if (index < endIndex && (str.charAt(index) == 41)) {
                  ++index;
                } else {
                  index = indexStart3; break;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderLatestDeliveryTime(String str, int index, int endIndex, ITokener tokener) {
      return ParseDateTime(str, index, endIndex, tokener);
    }

    public static int ParseHeaderListId(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParsePhrase(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        if (index < endIndex && (str.charAt(index) == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseAtext(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 46)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseDotAtomText(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        if (index < endIndex && (str.charAt(index) == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMessageContext(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMessageId(String str, int index, int endIndex, ITokener tokener) {
      return ParseMsgId(str, index, endIndex, tokener);
    }

    public static int ParseHeaderMimeVersion(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 46)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsAcp127MessageIdentifier(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsCodressMessageIndicator(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseNonnegInteger(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsCopyPrecedence(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParsePrecedence(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsExemptedAddress(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseAddressList(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsExtendedAuthorisationInfo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseDateTime(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsHandlingInstructions(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseMilitaryStringSequence(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsMessageInstructions(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseMilitaryStringSequence(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsMessageType(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            if (index < endIndex && (str.charAt(index) == 48)) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
                ++index;
              } else {
                break;
              }
              while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
                ++index;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index + 7 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1) & ~32) == 88 && (str.charAt(index + 2) & ~32) == 69 && (str.charAt(index + 3) & ~32) == 82 && (str.charAt(index + 4) & ~32) == 67 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 83 && (str.charAt(index + 7) & ~32) == 69) {
              indexTemp2 += 8; break;
            }
            if (index + 8 < endIndex && (str.charAt(index) & ~32) == 79 && (str.charAt(index + 1) & ~32) == 80 && (str.charAt(index + 2) & ~32) == 69 && (str.charAt(index + 3) & ~32) == 82 && (str.charAt(index + 4) & ~32) == 65 && (str.charAt(index + 5) & ~32) == 84 && (str.charAt(index + 6) & ~32) == 73 && (str.charAt(index + 7) & ~32) == 79 && (str.charAt(index + 8) & ~32) == 78) {
              indexTemp2 += 9; break;
            }
            if (index + 6 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 79 && (str.charAt(index + 3) & ~32) == 74 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 67 && (str.charAt(index + 6) & ~32) == 84) {
              indexTemp2 += 7; break;
            }
            if (index + 4 < endIndex && (str.charAt(index) & ~32) == 68 && (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 73 && (str.charAt(index + 3) & ~32) == 76 && (str.charAt(index + 4) & ~32) == 76) {
              indexTemp2 += 5; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseMessageTypeParam(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsOriginatorPlad(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsOriginatorReference(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsOtherRecipientsIndicatorCc(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseDesignator(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseDesignator(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsOtherRecipientsIndicatorTo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseDesignator(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseDesignator(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsPrimaryPrecedence(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParsePrecedence(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMmhsSubjectIndicatorCodes(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseSicSequence(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderMtPriority(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          do {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 45)) {
                ++index;
              }
              if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              index = indexStart2; break;
            }
          } while (false);
          if (index == indexStart2) {
            break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) == 48)) {
            ++index;
          } else {
            break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderObsoletes(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseMsgId(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseMsgId(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderOriginalRecipient(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseAtom(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2 = ParseText(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderPreventNondeliveryReport(String str, int index, int endIndex, ITokener tokener) {
      return ParseFWS(str, index, endIndex, tokener);
    }

    public static int ParseHeaderPriority(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderReceived(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = index;
            int indexStart2 = index;
            for (int i2 = 0;; ++i2) {
              indexTemp3 = ParseReceivedToken(str, index, endIndex, tokener);
              if (indexTemp3 == index) { if (i2 < 1) {
                  indexTemp2 = indexStart2;
                } break;
              } else {
                index = indexTemp3;
              }
            }
            index = indexStart2;
            if (indexTemp3 != indexStart2) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        int tx2 = ParseDateTime(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderReceivedSpf(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            if (index + 3 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 83 && (str.charAt(index + 3) & ~32) == 83) {
              indexTemp2 += 4; break;
            }
            if (index + 3 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 73 && (str.charAt(index + 3) & ~32) == 76) {
              indexTemp2 += 4; break;
            }
            if (index + 7 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 70 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 70 && (str.charAt(index + 5) & ~32) == 65 && (str.charAt(index + 6) & ~32) == 73 && (str.charAt(index + 7) & ~32) == 76) {
              indexTemp2 += 8; break;
            }
            if (index + 6 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 85 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 82 && (str.charAt(index + 5) & ~32) == 65 && (str.charAt(index + 6) & ~32) == 76) {
              indexTemp2 += 7; break;
            }
            if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 78 && (str.charAt(index + 3) & ~32) == 69) {
              indexTemp2 += 4; break;
            }
            if (index + 8 < endIndex && (str.charAt(index) & ~32) == 84 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 77 && (str.charAt(index + 3) & ~32) == 80 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 82 && (str.charAt(index + 7) & ~32) == 79 && (str.charAt(index + 8) & ~32) == 82) {
              indexTemp2 += 9; break;
            }
            if (index + 8 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 82 && (str.charAt(index + 3) & ~32) == 77 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 82 && (str.charAt(index + 7) & ~32) == 79 && (str.charAt(index + 8) & ~32) == 82) {
              indexTemp2 += 9; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        for (int i = 0;; ++i) {
          int indexTemp2 = ParseFWS(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            for (int i2 = 0;; ++i2) {
              int indexTemp3 = ParseFWS(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        index = ParseKeyValueList(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderResentTo(String str, int index, int endIndex, ITokener tokener) {
      return ParseStrictHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderReturnPath(String str, int index, int endIndex, ITokener tokener) {
      return ParsePath(str, index, endIndex, tokener);
    }

    public static int ParseHeaderSender(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseGroup(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderSensitivity(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderSolicitation(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseSolicitationKeywords(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderTo(String str, int index, int endIndex, ITokener tokener) {
      return ParseLaxHeaderTo(str, index, endIndex, tokener);
    }

    public static int ParseHeaderVbrInfo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            do {
              int indexTemp3 = index;
              do {
                int indexTemp4 = ParseMdElement(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseMcElement(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseMvElement(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                index = indexStart2; break;
              }
            } while (false);
            if (index == indexStart2) {
              break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseHeaderX400ContentIdentifier(String str, int index, int endIndex, ITokener tokener) {
      return ParseNoEncodedWords(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400ContentReturn(String str, int index, int endIndex, ITokener tokener) {
      return ParseMixerKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400MtsIdentifier(String str, int index, int endIndex, ITokener tokener) {
      return ParseNoEncodedWords(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400Originator(String str, int index, int endIndex, ITokener tokener) {
      return ParseMailbox(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400Received(String str, int index, int endIndex, ITokener tokener) {
      return ParseNoEncodedWords(str, index, endIndex, tokener);
    }

    public static int ParseHeaderX400Recipients(String str, int index, int endIndex, ITokener tokener) {
      return ParseMailboxList(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXRicevuta(String str, int index, int endIndex, ITokener tokener) {
      return ParseGeneralKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXTiporicevuta(String str, int index, int endIndex, ITokener tokener) {
      return ParseGeneralKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXTrasporto(String str, int index, int endIndex, ITokener tokener) {
      return ParseGeneralKeyword(str, index, endIndex, tokener);
    }

    public static int ParseHeaderXVerificasicurezza(String str, int index, int endIndex, ITokener tokener) {
      return ParseGeneralKeyword(str, index, endIndex, tokener);
    }

    public static int ParseIdLeft(String str, int index, int endIndex, ITokener tokener) {
      return ParseLocalPart(str, index, endIndex, tokener);
    }

    public static int ParseIdRight(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseNoFoldLiteral(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseKey(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 95) || (str.charAt(index) >= 45 && str.charAt(index) <= 46))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseKeyValueList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseKeyValuePair(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseKeyValuePair(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseKeyValuePair(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseKey(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParseDotAtom(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseQuotedString(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseLanguageDescription(String str, int index, int endIndex, ITokener tokener) {
      return ParsePrintablestring(str, index, endIndex, tokener);
    }

    public static int ParseLanguageList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseLanguageTag(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseLanguageTag(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseLanguageQ(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseLanguageRange(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index + 1 < endIndex && (str.charAt(index) & ~32) == 81 && str.charAt(index + 1) == 61) {
              index += 2;
            } else {
              index = indexStart2; break;
            }
            int tx3 = ParseQvalue(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseLanguageRange(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          for (int i2 = 0; i2 < 8; ++i2) {
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
              ++index;
            } else if (i2 < 1) {
              index = indexStart2; break;
            } else {
              break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          while (index + 1 < endIndex && ((str.charAt(index) == 45) && ((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57)))) {
            index += 2;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && (str.charAt(index) == 42)) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseLanguageTag(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseLaxHeaderTo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParseAddressList(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseLdhStr(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index + 1 < endIndex && ((str.charAt(index) == 45) && ((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57)))) {
              indexTemp2 += 2; break;
            }
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseLocalPart(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseWord(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 46)) {
              ++index;
            } else {
              break;
            }
            int tx3 = ParseWord(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(8, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseLocalPartNoCfws(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) == 34)) {
            ++index;
          } else {
            break;
          }
          while (true) {
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9))) {
                ++indexTemp3; break;
              }
              if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 91) || (str.charAt(index) >= 93 && str.charAt(index) <= 126))) {
                ++indexTemp3; break;
              }
              int indexTemp4 = index;
              do {
                int indexStart4 = index;
                if (index < endIndex && (str.charAt(index) == 92)) {
                  ++index;
                }
                do {
                  int indexTemp5 = index;
                  do {
                    if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                      ++indexTemp5; break;
                    }
                    if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                      indexTemp5 += 2; break;
                    }
                  } while (false);
                  if (indexTemp5 != index) {
                    index = indexTemp5;
                  } else {
                    index = indexStart4; break;
                  }
                } while (false);
                if (index == indexStart4) {
                  break;
                }
                indexTemp4 = index;
                index = indexStart4;
              } while (false);
              if (indexTemp4 != index) {
                indexTemp3 = indexTemp4; break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) == 92) && ((str.charAt(index + 1) >= 32 && str.charAt(index + 1) <= 126) || (str.charAt(index + 1) == 9)))) {
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              break;
            }
          }
          if (index < endIndex && (str.charAt(index) == 34)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMailbox(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(6, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseMailboxList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        int tx2 = ParseMailbox(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            do {
              int indexTemp3 = index;
              do {
                int indexTemp4 = ParseMailbox(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
                indexTemp4 = ParseCFWS(str, index, endIndex, tokener);
                if (indexTemp4 != index) {
                  indexTemp3 = indexTemp4; break;
                }
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMcElement(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 67 && str.charAt(index + 2) == 61) {
          index += 3;
        } else {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseTypeString(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMdElement(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 68 && str.charAt(index + 2) == 61) {
          index += 3;
        } else {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseDomainName(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMessageTypeParam(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 9 < endIndex && (str.charAt(index) & ~32) == 73 && (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32) == 69 && (str.charAt(index + 3) & ~32) == 78 && (str.charAt(index + 4) & ~32) == 84 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 70 && (str.charAt(index + 7) & ~32) == 73 && (str.charAt(index + 8) & ~32) == 69 && (str.charAt(index + 9) & ~32) == 82) {
          index += 10;
        } else {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseQuotedMilitaryString(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMethod(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = ParseLdhStr(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 47)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseMethodVersion(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMethodVersion(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
        } else {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMethodspec(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseMethod(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        tx2 = ParseResult(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMilitaryString(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
              ++indexTemp2; break;
            }
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 32) || (str.charAt(index) == 39) || (str.charAt(index) >= 43 && str.charAt(index) <= 58) || (str.charAt(index) == 61) || (str.charAt(index) == 63))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseMilitaryStringSequence(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            for (int i2 = 0; i2 < 69; ++i2) {
              int indexTemp3 = ParsePsChar(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 1) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMixerKeyword(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 45))) {
            ++index;
          }
        } else {
          index = indexStart; break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMsgId(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 60)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        int tx2 = ParseIdLeft(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        tx2 = ParseIdRight(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 62)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseMvElement(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 86 && str.charAt(index + 2) == 61) {
          index += 3;
        } else {
          break;
        }
        index = ParseFWS(str, index, endIndex, tokener);
        int tx2 = ParseCertifierList(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNameAddr(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseDisplayName(str, index, endIndex, tokener);
        int tx2 = ParseAngleAddr(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNoEncodedWords(String str, int index, int endIndex, ITokener tokener) {
      return ParseObsUnstruct(str, index, endIndex, tokener);
    }

    public static int ParseNoFoldLiteral(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) == 91)) {
          ++index;
        } else {
          break;
        }
        while (true) {
          int indexTemp2 = ParseDtext(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        if (index < endIndex && (str.charAt(index) == 93)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNoResult(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 78 && (str.charAt(index + 3) & ~32) == 69) {
          index += 4;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseNodeid(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 95) || (str.charAt(index) == 126))) {
              ++indexTemp2; break;
            }
            if (index + 2 < endIndex && ((str.charAt(index) == 37) && (((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102)) && ((str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102))))) {
              indexTemp2 += 3; break;
            }
            if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) == 36) || (str.charAt(index) >= 40 && str.charAt(index) <= 44) || (str.charAt(index) == 59) || (str.charAt(index) == 61))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseNonnegInteger(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) == 48)) {
          ++indexTemp; break;
        }
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
            ++index;
          } else {
            break;
          }
          while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
            ++index;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseObsAcceptLanguage(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseObsLanguageQ(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseObsLanguageQ(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsDomainList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        if (index < endIndex && (str.charAt(index) == 64)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        int tx2 = ParseDomain(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            do {
              int indexTemp3 = index;
              do {
                int indexStart3 = index;
                if (index < endIndex && (str.charAt(index) == 64)) {
                  ++index;
                } else {
                  break;
                }
                int tx4 = ParseDomain(str, index, endIndex, tokener);
                if (tx4 == index) {
                  index = indexStart3; break;
                } else {
                  index = tx4;
                }
                indexTemp3 = index;
                index = indexStart3;
              } while (false);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else { break;
              }
            } while (false);
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsGroupList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 44)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsLanguageQ(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseLanguageRange(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && ((str.charAt(index) == 81) || (str.charAt(index) == 113))) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 61)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            int tx3 = ParseQvalue(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsRoute(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseObsDomainList(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        if (index < endIndex && (str.charAt(index) == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseObsUnstruct(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexTemp3;
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              while (index < endIndex && (str.charAt(index) == 13)) {
                ++index;
              }
              for (int i3 = 0;; ++i3) {
                int indexTemp4 = index;
                do {
                  int indexTemp5 = index;
                  do {
                    if (index < endIndex && ((str.charAt(index) == 0) || (str.charAt(index) >= 1 && str.charAt(index) <= 8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) == 127) || (str.charAt(index) >= 33 && str.charAt(index) <= 126) || (str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                      ++indexTemp5; break;
                    }
                    if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                      indexTemp5 += 2; break;
                    }
                  } while (false);
                  if (indexTemp5 != index) {
                    indexTemp4 = indexTemp5; break;
                  }
                  int indexStart4 = index;
                  for (int i4 = 0;; ++i4) {
                    indexTemp5 = ParseFWS(str, index, endIndex, tokener);
                    if (indexTemp5 == index) { if (i4 < 1) {
                        indexTemp4 = indexStart4;
                      } break;
                    } else {
                      index = indexTemp5;
                    }
                  }
                  index = indexStart4;
                  if (indexTemp5 != indexStart4) {
                    indexTemp4 = indexTemp5; break;
                  }
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  if (i3 < 1) {
                    index = indexStart3;
                  } break;
                }
              }
              if (index == indexStart3) {
                break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
            if (index < endIndex && (str.charAt(index) == 10)) {
              ++indexTemp2;
              while (indexTemp2 < endIndex && (str.charAt(indexTemp2) == 10)) {
                ++indexTemp2;
              }
              break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        while (index < endIndex && (str.charAt(index) == 13)) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseOptParameterList(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            int tx3 = ParseParameter(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseOtherSections(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index + 1 < endIndex && (str.charAt(index) == 42) && (str.charAt(index + 1) >= 49 && str.charAt(index + 1) <= 57)) {
          index += 2;
        } else {
          break;
        }
        while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseParameter(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            int indexTemp3 = ParseRegularParameter(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              do {
                int indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                    ++index;
                    while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                      ++index;
                    }
                  } else {
                    break;
                  }
                  if (index + 1 < endIndex && str.charAt(index) == 42 && str.charAt(index + 1) == 48) {
                    index += 2;
                  }
                  if (index < endIndex && (str.charAt(index) == 42)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  index = indexStart3; break;
                }
              } while (false);
              if (index == indexStart3) {
                break;
              }
              index = ParseCFWS(str, index, endIndex, tokener);
              if (index < endIndex && (str.charAt(index) == 61)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
              index = ParseCFWS(str, index, endIndex, tokener);
              do {
                int indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  index = ParseCharset(str, index, endIndex, tokener);
                  if (index < endIndex && (str.charAt(index) == 39)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  index = ParseLanguageTag(str, index, endIndex, tokener);
                  if (index < endIndex && (str.charAt(index) == 39)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  while (true) {
                    int indexTemp5 = index;
                    do {
                      if (index + 2 < endIndex && ((str.charAt(index) == 37) && (((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102)) && ((str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102))))) {
                        indexTemp5 += 3; break;
                      }
                      if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                        ++indexTemp5; break;
                      }
                    } while (false);
                    if (indexTemp5 != index) {
                      index = indexTemp5;
                    } else {
                      break;
                    }
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  index = indexStart3; break;
                }
              } while (false);
              if (index == indexStart3) {
                break;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
            state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              do {
                int indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                    ++index;
                    while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                      ++index;
                    }
                  } else {
                    break;
                  }
                  int tx5 = ParseOtherSections(str, index, endIndex, tokener);
                  if (tx5 == index) {
                    index = indexStart4; break;
                  } else {
                    index = tx5;
                  }
                  if (index < endIndex && (str.charAt(index) == 42)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  index = indexStart3; break;
                }
              } while (false);
              if (index == indexStart3) {
                break;
              }
              if (index < endIndex && (str.charAt(index) == 61)) {
                ++index;
              } else {
                index = indexStart3; break;
              }
              while (true) {
                int indexTemp4 = index;
                do {
                  if (index + 2 < endIndex && ((str.charAt(index) == 37) && (((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102)) && ((str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102))))) {
                    indexTemp4 += 3; break;
                  }
                  if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
                    ++indexTemp4; break;
                  }
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else {
                  break;
                }
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePath(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str.charAt(index) == 60)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          if (index < endIndex && (str.charAt(index) == 62)) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePathxmpp(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            while (true) {
              int indexTemp3 = ParseNodeid(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                break;
              }
            }
            if (index < endIndex && (str.charAt(index) == 64)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        while (true) {
          int indexTemp2 = ParseRegName(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index < endIndex && (str.charAt(index) == 47)) {
              ++index;
            } else {
              break;
            }
            while (true) {
              int indexTemp3 = ParseResid(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                break;
              }
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePhrase(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParsePhraseWord(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        while (true) {
          int indexTemp2 = ParsePhraseWordOrDot(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(1, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParsePhraseAtom(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0;; ++i) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
              ++indexTemp2; break;
            }
            if (index < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 33) || (str.charAt(index) == 35) || (str.charAt(index) == 36) || (str.charAt(index) == 37) || (str.charAt(index) == 38) || (str.charAt(index) == 39) || (str.charAt(index) == 42) || (str.charAt(index) == 43) || (str.charAt(index) == 45) || (str.charAt(index) == 47) || (str.charAt(index) == 61) || (str.charAt(index) == 63) || (str.charAt(index) == 94) || (str.charAt(index) == 95) || (str.charAt(index) == 96) || (str.charAt(index) == 123) || (str.charAt(index) == 124) || (str.charAt(index) == 125) || (str.charAt(index) == 126))) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              if (index < endIndex && ((str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
                ++indexTemp3; break;
              }
              if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
                indexTemp3 += 2; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(3, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParsePhraseAtomOrDot(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = index;
        for (int i = 0;; ++i) {
          indexTemp2 = ParseAtext(str, index, endIndex, tokener);
          if (indexTemp2 == index) { if (i < 1) {
              indexTemp = indexStart;
            } break;
          } else {
            index = indexTemp2;
          }
        }
        index = indexStart;
        if (indexTemp2 != indexStart) {
          indexTemp = indexTemp2; break;
        }
        if (index < endIndex && (str.charAt(index) == 46)) {
          ++indexTemp; break;
        }
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(4, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParsePhraseWord(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          int tx3 = ParsePhraseAtom(str, index, endIndex, tokener);
          if (tx3 == index) {
            index = indexStart2; break;
          } else {
            index = tx3;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePhraseWordOrDot(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          int tx3 = ParsePhraseAtomOrDot(str, index, endIndex, tokener);
          if (tx3 == index) {
            index = indexStart2; break;
          } else {
            index = tx3;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePrecedence(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        do {
          int indexTemp2 = index;
          do {
            if (index < endIndex && (str.charAt(index) == 48)) {
              ++indexTemp2; break;
            }
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) >= 49 && str.charAt(index) <= 57)) {
                ++index;
              } else {
                break;
              }
              while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
                ++index;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (index + 7 < endIndex && (str.charAt(index) & ~32) == 68 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 70 && (str.charAt(index + 3) & ~32) == 69 && (str.charAt(index + 4) & ~32) == 82 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 69 && (str.charAt(index + 7) & ~32) == 68) {
              indexTemp2 += 8; break;
            }
            if (index + 6 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 85 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 73 && (str.charAt(index + 5) & ~32) == 78 && (str.charAt(index + 6) & ~32) == 69) {
              indexTemp2 += 7; break;
            }
            if (index + 7 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 73 && (str.charAt(index + 3) & ~32) == 79 && (str.charAt(index + 4) & ~32) == 82 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 84 && (str.charAt(index + 7) & ~32) == 89) {
              indexTemp2 += 8; break;
            }
            if (index + 8 < endIndex && (str.charAt(index) & ~32) == 73 && (str.charAt(index + 1) & ~32) == 77 && (str.charAt(index + 2) & ~32) == 77 && (str.charAt(index + 3) & ~32) == 69 && (str.charAt(index + 4) & ~32) == 68 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 65 && (str.charAt(index + 7) & ~32) == 84 && (str.charAt(index + 8) & ~32) == 69) {
              indexTemp2 += 9; break;
            }
            if (index + 4 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 76 && (str.charAt(index + 2) & ~32) == 65 && (str.charAt(index + 3) & ~32) == 83 && (str.charAt(index + 4) & ~32) == 72) {
              indexTemp2 += 5; break;
            }
            if (index + 7 < endIndex && (str.charAt(index) & ~32) == 79 && (str.charAt(index + 1) & ~32) == 86 && (str.charAt(index + 2) & ~32) == 69 && (str.charAt(index + 3) & ~32) == 82 && (str.charAt(index + 4) & ~32) == 82 && (str.charAt(index + 5) & ~32) == 73 && (str.charAt(index + 6) & ~32) == 68 && (str.charAt(index + 7) & ~32) == 69) {
              indexTemp2 += 8; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePrintablestring(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
              ++indexTemp2; break;
            }
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 32) || (str.charAt(index) == 39) || (str.charAt(index) >= 43 && str.charAt(index) <= 58) || (str.charAt(index) == 61) || (str.charAt(index) == 63))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseProperty(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index + 7 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 73 && (str.charAt(index + 3) & ~32) == 76 && (str.charAt(index + 4) & ~32) == 70 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 79 && (str.charAt(index + 7) & ~32) == 77) {
          indexTemp += 8; break;
        }
        if (index + 5 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1) & ~32) == 67 && (str.charAt(index + 2) & ~32) == 80 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 84 && (str.charAt(index + 5) & ~32) == 79) {
          indexTemp += 6; break;
        }
        // Unlimited production in choice
      } while (false);
      return indexTemp;
    }

    public static int ParsePropspec(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParsePtype(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 46)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        tx2 = ParseProperty(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        tx2 = ParsePvalue(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParsePsChar(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) >= 40 && str.charAt(index) <= 41)) {
          ++indexTemp; break;
        }
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) == 32) || (str.charAt(index) == 39) || (str.charAt(index) >= 43 && str.charAt(index) <= 58) || (str.charAt(index) == 61) || (str.charAt(index) == 63))) {
          ++indexTemp; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParsePtype(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 77 && (str.charAt(index + 2) & ~32) == 84 && (str.charAt(index + 3) & ~32) == 80) {
          indexTemp += 4; break;
        }
        if (index + 5 < endIndex && (str.charAt(index) & ~32) == 72 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 65 && (str.charAt(index + 3) & ~32) == 68 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 82) {
          indexTemp += 6; break;
        }
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 66 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 68 && (str.charAt(index + 3) & ~32) == 89) {
          indexTemp += 4; break;
        }
        if (index + 5 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 76 && (str.charAt(index + 3) & ~32) == 73 && (str.charAt(index + 4) & ~32) == 67 && (str.charAt(index + 5) & ~32) == 89) {
          indexTemp += 6; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParsePvalue(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        do {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
              ++indexTemp2;
              while (indexTemp2 < endIndex && ((str.charAt(indexTemp2) == 33) || (str.charAt(indexTemp2) >= 35 && str.charAt(indexTemp2) <= 36) || (str.charAt(indexTemp2) >= 45 && str.charAt(indexTemp2) <= 46) || (str.charAt(indexTemp2) >= 48 && str.charAt(indexTemp2) <= 57) || (str.charAt(indexTemp2) >= 65 && str.charAt(indexTemp2) <= 90) || (str.charAt(indexTemp2) >= 94 && str.charAt(indexTemp2) <= 126) || (str.charAt(indexTemp2) >= 42 && str.charAt(indexTemp2) <= 43) || (str.charAt(indexTemp2) >= 38 && str.charAt(indexTemp2) <= 39) || (str.charAt(indexTemp2) == 63))) {
                ++indexTemp2;
              }
              break;
            }
            int indexTemp3 = ParseQuotedString(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            int state3 = (tokener != null) ? tokener.GetState() : 0;
            indexTemp3 = index;
            do {
              int indexStart3 = index;
              do {
                int indexTemp4 = index;
                do {
                  int indexStart4 = index;
                  index = ParseLocalPart(str, index, endIndex, tokener);
                  if (index < endIndex && (str.charAt(index) == 64)) {
                    ++index;
                  } else {
                    index = indexStart4; break;
                  }
                  indexTemp4 = index;
                  index = indexStart4;
                } while (false);
                if (indexTemp4 != index) {
                  index = indexTemp4;
                } else { break;
                }
              } while (false);
              int tx4 = ParseDomainName(str, index, endIndex, tokener);
              if (tx4 == index) {
                index = indexStart3; break;
              } else {
                index = tx4;
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              indexTemp2 = indexTemp3; break;
            }
            if (tokener != null) {
              tokener.RestoreState(state3);
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseQcontent(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 91) || (str.charAt(index) >= 93 && str.charAt(index) <= 126) || (str.charAt(index) >= 1 && str.charAt(index) <= 8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) == 127) || (str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
          ++indexTemp; break;
        }
        if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
          indexTemp += 2; break;
        }
        int indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseQuotedMilitaryString(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) == 34)) {
          ++index;
        } else {
          break;
        }
        for (int i = 0; i < 69; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 1) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        if (index < endIndex && (str.charAt(index) == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseQuotedPair(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int indexTemp = index;
      do {
        if (index < endIndex && (str.charAt(index) == 92)) {
          ++index;
        } else {
          break;
        }
        do {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 33 && str.charAt(index) <= 126) || (str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
              ++indexTemp2; break;
            }
            if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
              indexTemp2 += 2; break;
            }
            if (index < endIndex && ((str.charAt(index) == 32) || (str.charAt(index) == 9) || (str.charAt(index) == 0) || (str.charAt(index) >= 1 && str.charAt(index) <= 8) || (str.charAt(index) >= 11 && str.charAt(index) <= 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 31) || (str.charAt(index) == 127) || (str.charAt(index) == 10) || (str.charAt(index) == 13))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            index = indexStart; break;
          }
        } while (false);
        if (index == indexStart) {
          break;
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseQuotedString(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            int tx3 = ParseQcontent(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        index = ParseFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 34)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null) {
        if (indexTemp == indexStart) {
          tokener.RestoreState(state);
        } else {
          tokener.Commit(7, indexStart, indexTemp);
        }
      }
      return indexTemp;
    }

    public static int ParseQvalue(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        int indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) == 48)) {
            ++index;
          } else {
            break;
          }
          do {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 46)) {
                ++index;
              } else {
                break;
              }
              for (int i3 = 0; i3 < 3; ++i3) {
                if (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
                  ++index;
                } else {
                  break;
                }
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else { break;
            }
          } while (false);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          if (index < endIndex && (str.charAt(index) == 49)) {
            ++index;
          } else {
            break;
          }
          do {
            int indexTemp3 = index;
            do {
              int indexStart3 = index;
              if (index < endIndex && (str.charAt(index) == 46)) {
                ++index;
              } else {
                break;
              }
              for (int i3 = 0; i3 < 3; ++i3) {
                if (index < endIndex && (str.charAt(index) == 48)) {
                  ++index;
                } else {
                  break;
                }
              }
              indexTemp3 = index;
              index = indexStart3;
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else { break;
            }
          } while (false);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseReasonspec(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 5 < endIndex && (str.charAt(index) & ~32) == 82 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 65 && (str.charAt(index + 3) & ~32) == 83 && (str.charAt(index + 4) & ~32) == 79 && (str.charAt(index + 5) & ~32) == 78) {
          index += 6;
        } else {
          break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        int tx2 = ParseValue(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseReceivedToken(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAddrSpec(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseDomain(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseAtom(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseRegName(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 95) || (str.charAt(index) == 126))) {
              ++indexTemp2; break;
            }
            if (index + 2 < endIndex && ((str.charAt(index) == 37) && (((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102)) && ((str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102))))) {
              indexTemp2 += 3; break;
            }
            if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) == 36) || (str.charAt(index) >= 38 && str.charAt(index) <= 44) || (str.charAt(index) == 59) || (str.charAt(index) == 61))) {
              ++indexTemp2; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseRegularParameter(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseRegularParameterName(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 61)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        tx2 = ParseValue(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseRegularParameterName(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
          ++index;
          while (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) == 43) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126))) {
            ++index;
          }
        } else {
          break;
        }
        index = ParseSection(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseResid(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        while (true) {
          int indexTemp2 = index;
          do {
            if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 95) || (str.charAt(index) == 126))) {
              ++indexTemp2; break;
            }
            if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) == 36) || (str.charAt(index) >= 38 && str.charAt(index) <= 44) || (str.charAt(index) >= 58 && str.charAt(index) <= 59) || (str.charAt(index) == 61))) {
              ++indexTemp2; break;
            }
            if (index + 2 < endIndex && ((str.charAt(index) == 37) && (((str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 70) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 102)) && ((str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 2) >= 65 && str.charAt(index + 2) <= 70) || (str.charAt(index + 2) >= 97 && str.charAt(index + 2) <= 102))))) {
              indexTemp2 += 3; break;
            }
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseResinfo(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 59)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        int tx2 = ParseMethodspec(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = ParseCFWS(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            tx3 = ParseReasonspec(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            int tx3 = ParseCFWS(str, index, endIndex, tokener);
            if (tx3 == index) {
              break;
            } else {
              index = tx3;
            }
            tx3 = ParsePropspec(str, index, endIndex, tokener);
            if (tx3 == index) {
              index = indexStart2; break;
            } else {
              index = tx3;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseRestrictedName(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57))) {
          ++index;
        } else {
          break;
        }
        for (int i = 0; i < 126; ++i) {
          if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) == 38) || (str.charAt(index) >= 94 && str.charAt(index) <= 95) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 43))) {
            ++index;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseResult(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 83 && (str.charAt(index + 3) & ~32) == 83) {
          indexTemp += 4; break;
        }
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 70 && (str.charAt(index + 1) & ~32) == 65 && (str.charAt(index + 2) & ~32) == 73 && (str.charAt(index + 3) & ~32) == 76) {
          indexTemp += 4; break;
        }
        if (index + 7 < endIndex && (str.charAt(index) & ~32) == 83 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 70 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 70 && (str.charAt(index + 5) & ~32) == 65 && (str.charAt(index + 6) & ~32) == 73 && (str.charAt(index + 7) & ~32) == 76) {
          indexTemp += 8; break;
        }
        if (index + 6 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 85 && (str.charAt(index + 3) & ~32) == 84 && (str.charAt(index + 4) & ~32) == 82 && (str.charAt(index + 5) & ~32) == 65 && (str.charAt(index + 6) & ~32) == 76) {
          indexTemp += 7; break;
        }
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 78 && (str.charAt(index + 1) & ~32) == 79 && (str.charAt(index + 2) & ~32) == 78 && (str.charAt(index + 3) & ~32) == 69) {
          indexTemp += 4; break;
        }
        if (index + 8 < endIndex && (str.charAt(index) & ~32) == 84 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 77 && (str.charAt(index + 3) & ~32) == 80 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 82 && (str.charAt(index + 7) & ~32) == 79 && (str.charAt(index + 8) & ~32) == 82) {
          indexTemp += 9; break;
        }
        if (index + 8 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 69 && (str.charAt(index + 2) & ~32) == 82 && (str.charAt(index + 3) & ~32) == 77 && (str.charAt(index + 4) & ~32) == 69 && (str.charAt(index + 5) & ~32) == 82 && (str.charAt(index + 6) & ~32) == 82 && (str.charAt(index + 7) & ~32) == 79 && (str.charAt(index + 8) & ~32) == 82) {
          indexTemp += 9; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseSection(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index + 1 < endIndex && str.charAt(index) == 42 && str.charAt(index + 1) == 48) {
          indexTemp += 2; break;
        }
        int indexTemp2 = ParseOtherSections(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseSicSequence(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        for (int i = 0; i < 8; ++i) {
          int indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            if (i < 3) {
              index = indexStart;
            } break;
          }
        }
        if (index == indexStart) {
          break;
        }
        while (true) {
          int indexTemp2;
          int state2 = (tokener != null) ? tokener.GetState() : 0;
          indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 59)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseFWS(str, index, endIndex, tokener);
            for (int i2 = 0; i2 < 8; ++i2) {
              int indexTemp3 = ParsePsChar(str, index, endIndex, tokener);
              if (indexTemp3 != index) {
                index = indexTemp3;
              } else {
                if (i2 < 3) {
                  index = indexStart2;
                } break;
              }
            }
            if (index == indexStart2) {
              break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else if (tokener != null) {
            tokener.RestoreState(state2); break;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseSolicitationKeywords(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122))) {
          ++index;
        } else {
          break;
        }
        while (index < endIndex && ((str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 95) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 58))) {
          ++index;
        }
        while (true) {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            if (index + 1 < endIndex && (str.charAt(index) == 44) && ((str.charAt(index + 1) >= 65 && str.charAt(index + 1) <= 90) || (str.charAt(index + 1) >= 97 && str.charAt(index + 1) <= 122))) {
              index += 2;
            } else {
              break;
            }
            while (index < endIndex && ((str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) == 95) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 122) || (str.charAt(index) >= 48 && str.charAt(index) <= 58))) {
              ++index;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else {
            break;
          }
        }
        indexTemp = index;
      } while (false);
      return indexTemp;
    }

    public static int ParseStrictHeaderTo(String str, int index, int endIndex, ITokener tokener) {
      return ParseAddressList(str, index, endIndex, tokener);
    }

    public static int ParseText(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) >= 1 && str.charAt(index) <= 9) || (str.charAt(index) == 11) || (str.charAt(index) == 12) || (str.charAt(index) >= 14 && str.charAt(index) <= 127) || (str.charAt(index) >= 128 && str.charAt(index) <= 55295) || (str.charAt(index) >= 57344 && str.charAt(index) <= 65535))) {
          ++indexTemp; break;
        }
        if (index + 1 < endIndex && ((str.charAt(index) >= 55296 && str.charAt(index) <= 56319) && (str.charAt(index + 1) >= 56320 && str.charAt(index + 1) <= 57343))) {
          indexTemp += 2; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseTime(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int tx2 = ParseTimeOfDay(str, index, endIndex, tokener);
        if (tx2 == index) {
          break;
        } else {
          index = tx2;
        }
        tx2 = ParseZone(str, index, endIndex, tokener);
        if (tx2 == index) {
          index = indexStart; break;
        } else {
          index = tx2;
        }
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseTimeOfDay(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index < endIndex && (str.charAt(index) == 58)) {
          ++index;
        } else {
          index = indexStart; break;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        do {
          int indexTemp2 = index;
          do {
            int indexStart2 = index;
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index < endIndex && (str.charAt(index) == 58)) {
              ++index;
            } else {
              index = indexStart2; break;
            }
            index = ParseCFWS(str, index, endIndex, tokener);
            if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
              index += 2;
            } else {
              index = indexStart2; break;
            }
            indexTemp2 = index;
            index = indexStart2;
          } while (false);
          if (indexTemp2 != index) {
            index = indexTemp2;
          } else { break;
          }
        } while (false);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseTypeString(String str, int index, int endIndex, ITokener tokener) {
      int indexTemp = index;
      do {
        if (index + 2 < endIndex && (str.charAt(index) & ~32) == 65 && (str.charAt(index + 1) & ~32) == 76 && (str.charAt(index + 2) & ~32) == 76) {
          indexTemp += 3; break;
        }
        if (index + 3 < endIndex && (str.charAt(index) & ~32) == 76 && (str.charAt(index + 1) & ~32) == 73 && (str.charAt(index + 2) & ~32) == 83 && (str.charAt(index + 3) & ~32) == 84) {
          indexTemp += 4; break;
        }
        if (index + 10 < endIndex && (str.charAt(index) & ~32) == 84 && (str.charAt(index + 1) & ~32) == 82 && (str.charAt(index + 2) & ~32) == 65 && (str.charAt(index + 3) & ~32) == 78 && (str.charAt(index + 4) & ~32) == 83 && (str.charAt(index + 5) & ~32) == 65 && (str.charAt(index + 6) & ~32) == 67 && (str.charAt(index + 7) & ~32) == 84 && (str.charAt(index + 8) & ~32) == 73 && (str.charAt(index + 9) & ~32) == 79 && (str.charAt(index + 10) & ~32) == 78) {
          indexTemp += 11; break;
        }
      } while (false);
      return indexTemp;
    }

    public static int ParseValue(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        if (index < endIndex && ((str.charAt(index) == 33) || (str.charAt(index) >= 35 && str.charAt(index) <= 36) || (str.charAt(index) >= 45 && str.charAt(index) <= 46) || (str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index) >= 65 && str.charAt(index) <= 90) || (str.charAt(index) >= 94 && str.charAt(index) <= 126) || (str.charAt(index) >= 42 && str.charAt(index) <= 43) || (str.charAt(index) >= 38 && str.charAt(index) <= 39) || (str.charAt(index) == 63))) {
          ++indexTemp;
          while (indexTemp < endIndex && ((str.charAt(indexTemp) == 33) || (str.charAt(indexTemp) >= 35 && str.charAt(indexTemp) <= 36) || (str.charAt(indexTemp) >= 45 && str.charAt(indexTemp) <= 46) || (str.charAt(indexTemp) >= 48 && str.charAt(indexTemp) <= 57) || (str.charAt(indexTemp) >= 65 && str.charAt(indexTemp) <= 90) || (str.charAt(indexTemp) >= 94 && str.charAt(indexTemp) <= 126) || (str.charAt(indexTemp) >= 42 && str.charAt(indexTemp) <= 43) || (str.charAt(indexTemp) >= 38 && str.charAt(indexTemp) <= 39) || (str.charAt(indexTemp) == 63))) {
            ++indexTemp;
          }
          break;
        }
        int indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseWord(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2 = ParseAtom(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        indexTemp2 = ParseQuotedString(str, index, endIndex, tokener);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseYear(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        index = ParseCFWS(str, index, endIndex, tokener);
        if (index + 1 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57))) {
          index += 2;
        } else {
          index = indexStart; break;
        }
        while (index < endIndex && (str.charAt(index) >= 48 && str.charAt(index) <= 57)) {
          ++index;
        }
        index = ParseCFWS(str, index, endIndex, tokener);
        indexTemp = index;
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }

    public static int ParseZone(String str, int index, int endIndex, ITokener tokener) {
      int indexStart = index;
      int state = (tokener != null) ? tokener.GetState() : 0;
      int indexTemp = index;
      do {
        int indexTemp2;
        int state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          for (int i2 = 0;; ++i2) {
            int indexTemp3 = ParseFWS(str, index, endIndex, tokener);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              if (i2 < 1) {
                index = indexStart2;
              } break;
            }
          }
          if (index == indexStart2) {
            break;
          }
          if (index < endIndex && ((str.charAt(index) == 43) || (str.charAt(index) == 45))) {
            ++index;
          } else {
            index = indexStart2; break;
          }
          if (index + 3 < endIndex && ((str.charAt(index) >= 48 && str.charAt(index) <= 57) || (str.charAt(index + 1) >= 48 && str.charAt(index + 1) <= 57) || (str.charAt(index + 2) >= 48 && str.charAt(index + 2) <= 57) || (str.charAt(index + 3) >= 48 && str.charAt(index + 3) <= 57))) {
            index += 4;
          } else {
            index = indexStart2; break;
          }
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
        state2 = (tokener != null) ? tokener.GetState() : 0;
        indexTemp2 = index;
        do {
          int indexStart2 = index;
          index = ParseCFWS(str, index, endIndex, tokener);
          do {
            int indexTemp3 = index;
            do {
              if (index + 1 < endIndex && (str.charAt(index) & ~32) == 85 && (str.charAt(index + 1) & ~32) == 84) {
                indexTemp3 += 2; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 71 && (str.charAt(index + 1) & ~32) == 77 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 69 && (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 && (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 67 && (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 77 && (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 83 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index + 2 < endIndex && (str.charAt(index) & ~32) == 80 && (str.charAt(index + 1) & ~32) == 68 && (str.charAt(index + 2) & ~32) == 84) {
                indexTemp3 += 3; break;
              }
              if (index < endIndex && ((str.charAt(index) >= 65 && str.charAt(index) <= 73) || (str.charAt(index) >= 75 && str.charAt(index) <= 90) || (str.charAt(index) >= 97 && str.charAt(index) <= 105) || (str.charAt(index) >= 107 && str.charAt(index) <= 122))) {
                ++indexTemp3; break;
              }
            } while (false);
            if (indexTemp3 != index) {
              index = indexTemp3;
            } else {
              index = indexStart2; break;
            }
          } while (false);
          if (index == indexStart2) {
            break;
          }
          index = ParseCFWS(str, index, endIndex, tokener);
          indexTemp2 = index;
          index = indexStart2;
        } while (false);
        if (indexTemp2 != index) {
          indexTemp = indexTemp2; break;
        }
        if (tokener != null) {
          tokener.RestoreState(state2);
        }
      } while (false);
      if (tokener != null && indexTemp == indexStart) {
        tokener.RestoreState(state);
      }
      return indexTemp;
    }
  }
