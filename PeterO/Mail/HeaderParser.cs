using System;

namespace PeterO.Mail {
internal static class HeaderParser {
public static int ParseAddrSpec(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseLocalPart(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 64)) {
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

public static int ParseAddress(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
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

public static int ParseAddressList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, indexTemp4,
  state, state2, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
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
 tx2 = ParseAddress(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
do {
  indexTemp3 = index;
 do {
  indexTemp4 = ParseAddress(str, index, endIndex, tokener);
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

public static int ParseAngleAddr(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseObsRoute(str, index, endIndex, tokener);
 tx2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 62)) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseAtext(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 33) || (str[index] == 35) || (str[index] == 36)||
  (str[index] == 37) || (str[index] == 38) || (str[index] == 39) ||
  (str[index] == 42) || (str[index] == 43) || (str[index] == 45) ||
  (str[index] == 47) || (str[index] == 61) || (str[index] == 63) ||
  (str[index] == 94) || (str[index] == 95) || (str[index] == 96) ||
  (str[index] == 123) || (str[index] == 124) || (str[index] == 125) ||
  (str[index] == 126) || (str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp += 2; break;
}
 } while (false);
 return indexTemp;
}

public static int ParseAtom(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 for (i = 0;; ++i) {
  indexTemp2 = ParseAtext(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseAuthresVersion(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
 while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseAuthservId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseValue(str, index, endIndex, tokener);
}

public static int ParseCFWS(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexStart3, indexTemp, indexTemp2,
  indexTemp3, state, state2, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = index;
 do {
 indexStart3 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 tx4 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
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
  if (i2< 1) {
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
for (i = 0; true; ++i) {
  indexTemp2 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp2 == index) { if (i< 1) {
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

public static int ParseCertifierList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseDomainName(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 break;
}
 tx3 = ParseDomainName(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseCharset(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] == 45) || (str[index] >= 48 && str[index] <= 57) ||
  (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 && str[index]
  <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
   (str[index] == 45) || (str[index] >= 48 && str[index] <= 57) ||
   (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 && str[index]
   <= 126))) {
 ++index;
}
} else {
 break;
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseDate(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseDay(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
if (index + 2 < endIndex && (((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 70&&
  (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 66) ||
  ((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index +
  2] & ~32) == 82) || ((str[index] & ~32) == 65 && (str[index + 1] & ~32) ==
  80 && (str[index + 2] & ~32) == 82) || ((str[index] & ~32) == 77 &&
  (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 89) ||
  ((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 85 && (str[index +
  2] & ~32) == 78) || ((str[index] & ~32) == 74 && (str[index + 1] & ~32) ==
  85 && (str[index + 2] & ~32) == 76) || ((str[index] & ~32) == 65 &&
  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 71) ||
  ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 80) || ((str[index] & ~32) == 79 && (str[index + 1] & ~32) ==
  67 && (str[index + 2] & ~32) == 84) || ((str[index] & ~32) == 78 &&
  (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 86) ||
  ((str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 67))) {
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

public static int ParseDateTime(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseDayOfWeek(str, index, endIndex, tokener);
 if (tx3 == index) {
 break;
} else {
 index = tx3;
}
if (index < endIndex && (str[index] == 44)) {
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
 tx2 = ParseDate(str, index, endIndex, tokener);
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

public static int ParseDay(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
for (i = 0; i < 2; ++i) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
  ++index;
 } else if (i< 1) {
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

public static int ParseDayOfWeek(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && (((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 84&&
  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 69) ||
  ((str[index] & ~32) == 87 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 68) || ((str[index] & ~32) == 84 && (str[index + 1] & ~32) ==
  72 && (str[index + 2] & ~32) == 85) || ((str[index] & ~32) == 70 &&
  (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73) ||
  ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 65 && (str[index +
  2] & ~32) == 84) || ((str[index] & ~32) == 83 && (str[index + 1] & ~32) ==
  85 && (str[index + 2] & ~32) == 78))) {
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

public static int ParseDesignator(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMilitaryString(str, index, endIndex, tokener);
}

public static int ParseDiagDeprecated(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexStart3, indexStart4, indexTemp,
  indexTemp2, indexTemp3, indexTemp4, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 33)) {
 ++index;
} else {
 break;
}
do {
  indexTemp2 = index;
 do {
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index + 1 < endIndex && str[index] == 50 && str[index + 1] == 53) {
 index += 2;
} else {
 break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 53)) {
 ++index;
} else {
 index = indexStart3; break;
}
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index + 1 < endIndex && (str[index] == 50) && (str[index + 1] >= 48 &&
  str[index + 1] <= 52)) {
 index += 2;
} else {
 break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
} else {
 index = indexStart3; break;
}
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
if (index + 2 < endIndex && ((str[index] == 49) && ((str[index + 1] >= 48 &&
  str[index + 1] <= 57) || (str[index + 2] >= 48 && str[index + 2] <= 57)))) {
 indexTemp2 += 3; break;
}
if (index + 1 < endIndex && ((str[index] >= 49 && str[index] <= 57) &&
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 indexTemp2 += 2; break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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
 for (i = 0; i < 3; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
do {
  indexTemp3 = index;
 do {
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index + 1 < endIndex && str[index] == 50 && str[index + 1] == 53) {
 index += 2;
} else {
 break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 53)) {
 ++index;
} else {
 index = indexStart4; break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index + 1 < endIndex && (str[index] == 50) && (str[index + 1] >= 48 &&
  str[index + 1] <= 52)) {
 index += 2;
} else {
 break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
} else {
 index = indexStart4; break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
if (index + 2 < endIndex && ((str[index] == 49) && ((str[index + 1] >= 48 &&
  str[index + 1] <= 57) || (str[index + 2] >= 48 && str[index + 2] <= 57)))) {
 indexTemp3 += 3; break;
}
if (index + 1 < endIndex && ((str[index] >= 49 && str[index] <= 57) &&
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 indexTemp3 += 2; break;
}
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 3) {
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

public static int ParseDiagIdentity(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, i4, indexStart, indexStart2, indexStart3, indexStart4, indexTemp,
  indexTemp2, indexTemp3, indexTemp4, indexTemp5, state, state2, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = index;
 do {
 indexStart3 = index;
 tx4 = ParseLabel(str, index, endIndex, tokener);
 if (tx4 == index) {
 break;
} else {
 index = tx4;
}
if (index < endIndex && (str[index] == 46)) {
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
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
do {
  indexTemp3 = index;
 do {
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
  break;
}
 for (i4 = 0;; ++i4) {
  indexTemp5 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp5 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
 ++indexTemp5; break;
}
 } while (false);
  if (indexTemp5 != index) {
 index = indexTemp5;
} else {
  if (i4< 1) {
   index = indexStart4;
  } break;
 }
 }
 if (index == indexStart4) {
 break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index] == 45))) {
 ++index;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
 index = indexStart4; break;
}
 while (true) {
  indexTemp5 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp5 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
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
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 if (tokener != null) {
 tokener.RestoreState(state2);
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45) || (str[index] == 95))) {
 ++indexTemp;
 while (indexTemp < endIndex && ((str[indexTemp] >= 65 && str[indexTemp] <=
   90) || (str[indexTemp] >= 97 && str[indexTemp] <= 122) || (str[indexTemp]
   >= 48 && str[indexTemp] <= 57) || (str[indexTemp] == 45) ||
   (str[indexTemp] == 95))) {
indexTemp++;
}
 break;
}
 // Unlimited production in choice
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDiagOther(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 1 < endIndex && str[index] == 33 && str[index + 1] == 46) {
 index += 2;
} else {
 break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122))) {
 ++index;
}
} else {
 index = indexStart; break;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
 tx3 = ParseDiagIdentity(str, index, endIndex, tokener);
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
 index = ParseFWS(str, index, endIndex, tokener);
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseDispNotParam(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
   (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
   <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
   str[index] <= 126))) {
 ++index;
}
} else {
 index = indexStart; break;
}
if (index + 8 < endIndex && (str[index] == 61) && (((str[index + 1] & ~32)
  == 82 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) == 81 &&
  (str[index + 4] & ~32) == 85 && (str[index + 5] & ~32) == 73 && (str[index+
  6] & ~32) == 82 && (str[index + 7] & ~32) == 69 && (str[index + 8] & ~32)
    == 68) || ((str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) ==
  80 && (str[index + 3] & ~32) == 84 && (str[index + 4] & ~32) == 73 &&
  (str[index + 5] & ~32) == 79 && (str[index + 6] & ~32) == 78 && (str[index+
  7] & ~32) == 65 && (str[index + 8] & ~32) == 76))) {
 index += 9;
} else {
 index = indexStart; break;
}
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 index = indexStart; break;
}
 tx2 = ParseValue(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 tx3 = ParseValue(str, index, endIndex, tokener);
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

public static int ParseDisplayName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParsePhrase(str, index, endIndex, tokener);
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseDistName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++indexTemp; break;
}
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while ((index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 43) || (str[index] == 45) || (str[index] == 95)))
) {
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

public static int ParseDomain(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, state2, state3, tx3, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseDomainLiteral(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseAtom(str, index, endIndex, tokener);
 if (tx3 == index) {
 break;
} else {
 index = tx3;
}
 while (true) {
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
 tx4 = ParseAtom(str, index, endIndex, tokener);
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

public static int ParseDomainLiteral(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 91)) {
 ++index;
} else {
 index = indexStart; break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseDtext(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 93)) {
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

public static int ParseDomainName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
 ++index;
} else {
  break;
}
 index = ParseLdhStr(str, index, endIndex, tokener);
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index + 1 < endIndex && (str[index] == 46) && ((str[index + 1] >= 65 &&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <= 122)||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
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
  if (i< 1) {
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

public static int ParseDomainNoCfws(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, indexTemp4,
  state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 91)) {
 ++index;
} else {
 break;
}
 while (true) {
  indexTemp3 = index;
 do {
if (index < endIndex && ((str[index] >= 33 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126))) {
 ++indexTemp3; break;
}
 indexTemp4 = index;
 do {
if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp4; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
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
if (index < endIndex && (str[index] == 93)) {
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

public static int ParseDotAtom(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseDotAtomText(str, index, endIndex, tokener);
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

public static int ParseDotAtomText(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = ParseAtext(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseAtext(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
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

public static int ParseDtext(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 33 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 1 && str[index]
  <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 &&
  str[index] <= 31) || (str[index] == 127))) {
 ++indexTemp; break;
}
  indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp += 2; break;
}
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseEncodingCount(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
 while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseEncodingKeyword(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
 index = indexStart; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45))) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseFWS(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index + 1 < endIndex && str[index] == 13 && str[index + 1] == 10) {
 index += 2;
}
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
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
  if (i< 1) {
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

public static int ParseGeneralKeyword(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45))) {
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

public static int ParseGroup(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseDisplayName(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseGroupList(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
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

public static int ParseGroupList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexStart3, indexTemp, indexTemp2,
  indexTemp3, state, state2, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
  indexTemp2 = ParseObsGroupList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = index;
 do {
 indexStart3 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 tx4 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
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
  if (i2< 1) {
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
for (i = 0; true; ++i) {
  indexTemp2 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp2 == index) { if (i< 1) {
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

public static int ParseHeaderAcceptLanguage(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, state2, state3, tx3, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseLanguageQ(str, index, endIndex, tokener);
 if (tx3 == index) {
index = indexStart2; break;
} else {
 index = tx3;
}
 while (true) {
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx4 = ParseLanguageQ(str, index, endIndex, tokener);
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

public static int ParseHeaderAlternateRecipient(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderArchive(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
if (index + 1 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79) {
 indexTemp2 += 2; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 89 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 83) {
 indexTemp2 += 3; break;
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
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseParameter(str, index, endIndex, tokener);
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

public static int ParseHeaderArchivedAt(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart; break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
  (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
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
if (index < endIndex && (str[index] == 62)) {
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

public static int ParseHeaderAuthenticationResults(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, state,
  tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseAuthservId(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseCFWS(str, index, endIndex, tokener);
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
  indexTemp2 = index;
 do {
  indexTemp3 = ParseNoResult(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
 indexStart2 = index;
for (i2 = 0; true; ++i2) {
  indexTemp3 = ParseResinfo(str, index, endIndex, tokener);
  if (indexTemp3 == index) { if (i2< 1) {
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

public static int ParseHeaderAutoSubmitted(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
 ++index;
}
} else {
 index = indexStart; break;
}
 while (true) {
  indexTemp2 = ParseOptParameterList(str, index, endIndex, tokener);
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

public static int ParseHeaderAutoforwarded(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderBcc(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
  indexTemp3 = ParseAddressList(str, index, endIndex, tokener);
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

public static int ParseHeaderContentBase(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
  (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
   (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
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

public static int ParseHeaderContentDisposition(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
 ++index;
}
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 tx3 = ParseParameter(str, index, endIndex, tokener);
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

public static int ParseHeaderContentDuration(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
for (i = 0; i < 10; ++i) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
  ++index;
 } else if (i< 1) {
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

public static int ParseHeaderContentId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHeaderContentLanguage(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseLanguageList(str, index, endIndex, tokener);
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

public static int ParseHeaderContentLocation(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
  (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
   (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
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

public static int ParseHeaderContentMd5(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 47 && str[index]
  <= 57) || (str[index] == 43) || (str[index] == 61))) {
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

public static int ParseHeaderContentTransferEncoding(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45))) {
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

public static int ParseHeaderContentType(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseRestrictedName(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 47)) {
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
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseParameter(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseHeaderControl(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state2;
indexStart = index;
 indexTemp = index;
 do {
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
 ++index;
}
} else {
 index = indexStart; break;
}
 while (true) {
  state2 = indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
 while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
} else {
 break;
}
if (index < endIndex && (str[index] >= 33 && str[index] <= 126)) {
 ++index;
 while (index < endIndex && (str[index] >= 33 && str[index] <= 126)) {
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
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseHeaderConversion(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderConversionWithLoss(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderDate(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderDeferredDelivery(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderDeliveryDate(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseDateTime(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseDayOfWeek(str, index, endIndex, tokener);
 if (tx3 == index) {
index = indexStart2; break;
} else {
 index = tx3;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && (((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 70&&
  (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 66) ||
  ((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index +
  2] & ~32) == 82) || ((str[index] & ~32) == 65 && (str[index + 1] & ~32) ==
  80 && (str[index + 2] & ~32) == 82) || ((str[index] & ~32) == 77 &&
  (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 89) ||
  ((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 85 && (str[index +
  2] & ~32) == 78) || ((str[index] & ~32) == 74 && (str[index + 1] & ~32) ==
  85 && (str[index + 2] & ~32) == 76) || ((str[index] & ~32) == 65 &&
  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 71) ||
  ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 80) || ((str[index] & ~32) == 79 && (str[index + 1] & ~32) ==
  67 && (str[index + 2] & ~32) == 84) || ((str[index] & ~32) == 78 &&
  (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 86) ||
  ((str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 67))) {
 index += 3;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
for (i2 = 0; i2 < 2; ++i2) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
  ++index;
 } else if (i2< 1) {
index = indexStart2; break;
 } else {
 break;
}
}
if (index == indexStart2) {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57)) && (str[index + 2] == 58)) {
 index += 3;
} else {
 index = indexStart2; break;
}
if (index + 2 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57)) && (str[index + 2] == 58)) {
 index += 3;
} else {
 index = indexStart2; break;
}
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
for (i2 = 0;; ++i2) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
  ++index;
 } else if (i2< 4) {
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

public static int ParseHeaderDiscloseRecipients(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderDispositionNotificationOptions(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseDispNotParam(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 tx3 = ParseDispNotParam(str, index, endIndex, tokener);
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

public static int ParseHeaderDispositionNotificationTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMailboxList(str, index, endIndex, tokener);
}

public static int ParseHeaderDistribution(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
 tx2 = ParseDistName(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseDistName(str, index, endIndex, tokener);
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
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderDkimSignature(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseNoEncodedWords(str, index, endIndex, tokener);
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseHeaderEdiintFeatures(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index]
  <= 122) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
   (str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index]
   <= 122) || (str[index] == 45))) {
 ++index;
}
} else {
 index = indexStart; break;
}
 while (true) {
  state2 = indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 index = indexStart2; break;
}
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index]
  <= 122) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
   (str[index] >= 65 && str[index] <= 90) || (str[index] >= 97 && str[index]
   <= 122) || (str[index] == 45))) {
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

public static int ParseHeaderEesstVersion(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && str[index] == 49 && str[index + 1] == 46 &&
  str[index + 2] == 48) {
 index += 3;
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

public static int ParseHeaderEncoding(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseEncodingCount(str, index, endIndex, tokener);
 if (tx3 == index) {
 break;
} else {
 index = tx3;
}
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseEncodingKeyword(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
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
 for (i = 0;; ++i) {
  indexTemp2 = ParseEncodingKeyword(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseHeaderEncrypted(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseWord(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 tx3 = ParseWord(str, index, endIndex, tokener);
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

public static int ParseHeaderExpandedDate(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i3, indexStart, indexStart2, indexStart3, indexTemp, indexTemp2,
  indexTemp3, indexTemp4, state, state3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && (((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 84&&
  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 69) ||
  ((str[index] & ~32) == 87 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 68) || ((str[index] & ~32) == 84 && (str[index + 1] & ~32) ==
  72 && (str[index + 2] & ~32) == 85) || ((str[index] & ~32) == 70 &&
  (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73) ||
  ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 65 && (str[index +
  2] & ~32) == 84) || ((str[index] & ~32) == 83 && (str[index + 1] & ~32) ==
  85 && (str[index + 2] & ~32) == 78))) {
 index += 3;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
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
 index = ParseCFWS(str, index, endIndex, tokener);
for (i = 0; i < 2; ++i) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
  ++index;
 } else if (i< 1) {
index = indexStart; break;
 } else {
 break;
}
}
if (index == indexStart) {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 2 < endIndex && (((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 78) || ((str[index] & ~32) == 70&&
  (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 66) ||
  ((str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index +
  2] & ~32) == 82) || ((str[index] & ~32) == 65 && (str[index + 1] & ~32) ==
  80 && (str[index + 2] & ~32) == 82) || ((str[index] & ~32) == 77 &&
  (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 89) ||
  ((str[index] & ~32) == 74 && (str[index + 1] & ~32) == 85 && (str[index +
  2] & ~32) == 78) || ((str[index] & ~32) == 74 && (str[index + 1] & ~32) ==
  85 && (str[index + 2] & ~32) == 76) || ((str[index] & ~32) == 65 &&
  (str[index + 1] & ~32) == 85 && (str[index + 2] & ~32) == 71) ||
  ((str[index] & ~32) == 83 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 80) || ((str[index] & ~32) == 79 && (str[index + 1] & ~32) ==
  67 && (str[index + 2] & ~32) == 84) || ((str[index] & ~32) == 78 &&
  (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 86) ||
  ((str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index +
  2] & ~32) == 67))) {
 index += 3;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
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
do {
  indexTemp2 = index;
 do {
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
 for (i3 = 0;; ++i3) {
  indexTemp4 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp4 != index) {
 index = indexTemp4;
} else {
  if (i3< 1) {
   index = indexStart3;
  } break;
 }
 }
 if (index == indexStart3) {
 break;
}
if (index < endIndex && ((str[index] == 43) || (str[index] == 45))) {
 ++index;
} else {
 index = indexStart3; break;
}
if (index + 3 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57) || (str[index + 2] >= 48 &&
  str[index + 2] <= 57) || (str[index + 3] >= 48 && str[index + 3] <= 57))) {
 index += 4;
} else {
 index = indexStart3; break;
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
 indexStart3 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp4 = index;
 do {
if (index + 1 < endIndex && (str[index] & ~32) == 85 && (str[index + 1] & ~32) == 84) {
 indexTemp4 += 2; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 71 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp4 += 3; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 73) ||
  (str[index] >= 75 && str[index] <= 90) || (str[index] >= 97 && str[index]
  <= 105) || (str[index] >= 107 && str[index] <= 122))) {
 ++indexTemp4; break;
}
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
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderFollowupTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseNewsgroupList(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 indexTemp2 = index;
 do {
 indexStart2 = index;
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
if (index + 5 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82) {
 index += 6;
} else {
 index = indexStart2; break;
}
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
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

public static int ParseHeaderFrom(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseMailboxList(str, index, endIndex, tokener);
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

public static int ParseHeaderGenerateDeliveryReport(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseFWS(str, index, endIndex, tokener);
}

public static int ParseHeaderImportance(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderInReplyTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, indexTemp3, state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
  indexTemp3 = ParsePhrase(str, index, endIndex, tokener);
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

public static int ParseHeaderIncompleteCopy(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseFWS(str, index, endIndex, tokener);
}

public static int ParseHeaderInjectionDate(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderInjectionInfo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParsePathIdentity(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseParameter(str, index, endIndex, tokener);
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

public static int ParseHeaderJabberId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 32)) {
 ++index;
} else {
 break;
}
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
 tx2 = ParsePathxmpp(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderKeywords(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParsePhrase(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 tx3 = ParsePhrase(str, index, endIndex, tokener);
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

public static int ParseHeaderLanguage(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index + 1 < endIndex && (((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122)) && ((str[index + 1] >= 65 &&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122)))) {
 index += 2;
} else {
  break;
}
while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
}
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 40)) {
 ++index;
} else {
 break;
}
 tx4 = ParseLanguageDescription(str, index, endIndex, tokener);
 if (tx4 == index) {
index = indexStart3; break;
} else {
 index = tx4;
}
if (index < endIndex && (str[index] == 41)) {
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

public static int ParseHeaderLatestDeliveryTime(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseDateTime(str, index, endIndex, tokener);
}

public static int ParseHeaderListId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
  indexTemp3 = ParsePhrase(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart; break;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseAtext(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseDotAtomText(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 62)) {
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

public static int ParseHeaderMessageContext(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45))) {
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

public static int ParseHeaderMessageId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMsgId(str, index, endIndex, tokener);
}

public static int ParseHeaderMimeVersion(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
 while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
}
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
 while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseHeaderMmhsAcp127MessageIdentifier(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 for (i = 0; i < 69; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseHeaderMmhsAuthorizingUsers(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMailboxList(str, index, endIndex, tokener);
}

public static int ParseHeaderMmhsCodressMessageIndicator(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseNonnegInteger(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsCopyPrecedence(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParsePrecedence(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsExemptedAddress(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseAddressList(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsExtendedAuthorisationInfo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseDateTime(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsHandlingInstructions(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseMilitaryStringSequence(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsMessageInstructions(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseMilitaryStringSequence(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsMessageType(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
if (index < endIndex && (str[index] == 48)) {
 ++indexTemp2; break;
}
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
}
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 88 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) ==
  82&&
  (str[index + 4] & ~32) == 67 && (str[index + 5] & ~32) == 73 &&
  (str[index + 6] & ~32) == 83 && (str[index + 7] & ~32) == 69) {
 indexTemp2 += 8; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 79 && (str[index + 1] & ~32) == 80 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) ==
  82&&
  (str[index + 4] & ~32) == 65 && (str[index + 5] & ~32) == 84 &&
  (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 79 && (str[index+
  8] & ~32) == 78) {
 indexTemp2 += 9; break;
}
if (index + 6 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 79 && (str[index + 3] & ~32) ==
  74&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 67 &&
  (str[index + 6] & ~32) == 84) {
 indexTemp2 += 7; break;
}
if (index + 4 < endIndex && (str[index] & ~32) == 68 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) ==
  76&&
  (str[index + 4] & ~32) == 76) {
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
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseMessageTypeParam(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsOriginatorPlad(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 for (i = 0; i < 69; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseHeaderMmhsOriginatorReference(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 for (i = 0; i < 69; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseHeaderMmhsOtherRecipientsIndicatorCc(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseDesignator(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseDesignator(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsOtherRecipientsIndicatorTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseDesignator(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseDesignator(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsPrimaryPrecedence(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParsePrecedence(str, index, endIndex, tokener);
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

public static int ParseHeaderMmhsSubjectIndicatorCodes(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseSicSequence(str, index, endIndex, tokener);
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

public static int ParseHeaderMtPriority(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 45)) {
 ++index;
}
if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
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
 indexStart2 = index;
if (index < endIndex && (str[index] == 48)) {
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

public static int ParseHeaderNewsgroups(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
while (index < endIndex && (str[index] == 32)) {
 ++index;
}
 tx2 = ParseNewsgroupList(str, index, endIndex, tokener);
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

public static int ParseHeaderNntpPostingHost(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
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

public static int ParseHeaderObsoletes(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseMsgId(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 tx3 = ParseMsgId(str, index, endIndex, tokener);
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

public static int ParseHeaderOriginalRecipient(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseAtom(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart; break;
}
 while (true) {
  indexTemp2 = ParseText(str, index, endIndex, tokener);
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

public static int ParseHeaderPath(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
 while (true) {
  indexTemp2 = ParsePathList(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
index = indexTemp2;
} else {
 break;
}
 }
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45) || (str[index] == 95))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45) || (str[index] == 95))) {
 ++index;
}
} else {
 index = indexStart; break;
}
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseHeaderPreventNondeliveryReport(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseFWS(str, index, endIndex, tokener);
}

public static int ParseHeaderPriority(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderReceived(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
 indexTemp3 = index;
 indexStart2 = index;
for (i2 = 0; true; ++i2) {
  indexTemp3 = ParseReceivedToken(str, index, endIndex, tokener);
  if (indexTemp3 == index) { if (i2< 1) {
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
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart; break;
}
 tx2 = ParseDateTime(str, index, endIndex, tokener);
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

public static int ParseHeaderReceivedSpf(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
if (index + 3 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) ==
  83) {
 indexTemp2 += 4; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) ==
  76) {
 indexTemp2 += 4; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 65 &&
  (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 76) {
 indexTemp2 += 8; break;
}
if (index + 6 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 65 &&
  (str[index + 6] & ~32) == 76) {
 indexTemp2 += 7; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) ==
  69) {
 indexTemp2 += 4; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 84 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) ==
  80&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index+
  8] & ~32) == 82) {
 indexTemp2 += 9; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 82 && (str[index + 3] & ~32) ==
  77&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index+
  8] & ~32) == 82) {
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
 for (i = 0;; ++i) {
  indexTemp2 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = HeaderParserUtility.ParseCommentLax(str, index, endIndex, tokener);
 if (tx3 == index) {
 break;
} else {
 index = tx3;
}
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
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

public static int ParseHeaderRequireRecipientValidSince(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseAddrSpec(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart; break;
}
 tx2 = ParseDateTime(str, index, endIndex, tokener);
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

public static int ParseHeaderResentTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseStrictHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderReturnPath(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParsePath(str, index, endIndex, tokener);
}

public static int ParseHeaderSender(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseMailbox(str, index, endIndex, tokener);
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

public static int ParseHeaderSensitivity(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderSioLabel(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseSioLabelParmSeq(str, index, endIndex, tokener);
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

public static int ParseHeaderSolicitation(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseSolicitationKeywords(str, index, endIndex, tokener);
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

public static int ParseHeaderSupersedes(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexTemp, indexTemp2, state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart2; break;
}
 tx3 = ParseIdLeft(str, index, endIndex, tokener);
 if (tx3 == index) {
index = indexStart2; break;
} else {
 index = tx3;
}
if (index < endIndex && (str[index] == 64)) {
 ++index;
} else {
 index = indexStart2; break;
}
 tx3 = ParseIdRight(str, index, endIndex, tokener);
 if (tx3 == index) {
index = indexStart2; break;
} else {
 index = tx3;
}
if (index < endIndex && (str[index] == 62)) {
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
} else {
  if (i< 1) {
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

public static int ParseHeaderTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseLaxHeaderTo(str, index, endIndex, tokener);
}

public static int ParseHeaderUserAgent(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexStart3, indexTemp, indexTemp2,
  indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
 ++index;
}
} else {
 index = indexStart2; break;
}
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 47)) {
 ++index;
} else {
 index = indexStart3; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index]
   <= 90) || (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 &&
   str[index] <= 43) || (str[index] >= 38 && str[index] <= 39) ||
   (str[index] == 63))) {
 ++index;
}
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
  if (i< 1) {
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

public static int ParseHeaderVbrInfo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  indexTemp4, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
do {
  indexTemp3 = index;
 do {
  indexTemp4 = ParseMdElement(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 59)) {
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
  if (i< 1) {
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

public static int ParseHeaderX400ContentIdentifier(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseNoEncodedWords(str, index, endIndex, tokener);
}

public static int ParseHeaderX400ContentReturn(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMixerKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderX400MtsIdentifier(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseNoEncodedWords(str, index, endIndex, tokener);
}

public static int ParseHeaderX400Originator(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMailbox(str, index, endIndex, tokener);
}

public static int ParseHeaderX400Received(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseNoEncodedWords(str, index, endIndex, tokener);
}

public static int ParseHeaderX400Recipients(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseMailboxList(str, index, endIndex, tokener);
}

public static int ParseHeaderXArchivedAt(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
  (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] >= 33 && str[index] <= 59) ||
   (str[index] == 61) || (str[index] >= 63 && str[index] <= 126))) {
 ++index;
}
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

public static int ParseHeaderXRicevuta(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseGeneralKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderXTiporicevuta(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseGeneralKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderXTrasporto(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseGeneralKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderXVerificasicurezza(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseGeneralKeyword(str, index, endIndex, tokener);
}

public static int ParseHeaderXref(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
 tx2 = ParsePathIdentity(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
 tx3 = ParseNewsgroupName(str, index, endIndex, tokener);
 if (tx3 == index) {
index = indexStart2; break;
} else {
 index = tx3;
}
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart2; break;
}
if (index < endIndex && ((str[index] >= 33 && str[index] <= 39) ||
  (str[index] >= 41 && str[index] <= 58) || (str[index] >= 60 && str[index]
  <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] >= 33 && str[index] <= 39) ||
   (str[index] >= 41 && str[index] <= 58) || (str[index] >= 60 && str[index]
   <= 126))) {
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
  if (i< 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseIdLeft(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseLocalPart(str, index, endIndex, tokener);
}

public static int ParseIdRight(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseDomain(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseKey(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
  break;
}
while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 95) || (str[index] >= 45 && str[index] <= 46))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseKeyValueList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseKeyValuePair(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseKeyValuePair(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 59)) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseKeyValuePair(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, indexTemp3, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseKey(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
 ++index;
} else {
 index = indexStart; break;
}
do {
  indexTemp2 = index;
 do {
  indexTemp3 = ParseDotAtom(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseLabel(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
 ++index;
} else {
  break;
}
 while (true) {
  indexTemp2 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp2 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
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

public static int ParseLanguageDescription(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParsePrintablestring(str, index, endIndex, tokener);
}

public static int ParseLanguageList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseLanguageTag(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseLanguageTag(str, index, endIndex, tokener);
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

public static int ParseLanguageQ(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseLanguageRange(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && (str[index] & ~32) == 81 && str[index + 1] == 61) {
 index += 2;
} else {
 index = indexStart2; break;
}
 tx3 = ParseQvalue(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseLanguageRange(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 indexTemp2 = index;
 do {
 indexStart2 = index;
for (i2 = 0; i2 < 8; ++i2) {
 if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122))) {
  ++index;
 } else if (i2< 1) {
index = indexStart2; break;
 } else {
 break;
}
}
if (index == indexStart2) {
 break;
}
while ((index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >=
  65 && str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57))))) {
 index += 2;
}
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
if (index < endIndex && (str[index] == 42)) {
 ++indexTemp; break;
}
 } while (false);
 return indexTemp;
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseLanguageTag(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
  break;
}
while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseLaxHeaderTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
  indexTemp3 = ParseAddressList(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseLdhStr(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp2 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
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

public static int ParseLocalPart(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseWord(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
 tx3 = ParseWord(str, index, endIndex, tokener);
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

public static int ParseLocalPartNoCfws(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart4, indexTemp, indexTemp2, indexTemp3,
  indexTemp4, indexTemp5, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseDotAtomText(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 34)) {
 ++index;
} else {
 break;
}
 while (true) {
  indexTemp3 = index;
 do {
if (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++indexTemp3; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 91) || (str[index] >= 93 && str[index] <= 126))) {
 ++indexTemp3; break;
}
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && (str[index] == 92)) {
 ++index;
}
do {
  indexTemp5 = index;
 do {
if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp5; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
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
if (index + 1 < endIndex && ((str[index] == 92) && ((str[index + 1] >= 32&&
  str[index + 1] <= 126) || (str[index + 1] == 9)))) {
 indexTemp3 += 2; break;
}
 } while (false);
  if (indexTemp3 != index) {
index = indexTemp3;
} else {
 break;
}
 }
if (index < endIndex && (str[index] == 34)) {
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

public static int ParseMailbox(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseNameAddr(str, index, endIndex, tokener);
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

public static int ParseMailboxList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, indexTemp4,
  state, state2, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
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
 tx2 = ParseMailbox(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
do {
  indexTemp3 = index;
 do {
  indexTemp4 = ParseMailbox(str, index, endIndex, tokener);
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

public static int ParseMcElement(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 67 && str[index + 2] == 61) {
 index += 3;
} else {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseTypeString(str, index, endIndex, tokener);
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

public static int ParseMdElement(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 68 && str[index + 2] == 61) {
 index += 3;
} else {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseDomainName(str, index, endIndex, tokener);
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

public static int ParseMessageTypeParam(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 9 < endIndex && (str[index] & ~32) == 73 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) ==
  78&&
  (str[index + 4] & ~32) == 84 && (str[index + 5] & ~32) == 73 &&
  (str[index + 6] & ~32) == 70 && (str[index + 7] & ~32) == 73 && (str[index+
  8] & ~32) == 69 && (str[index + 9] & ~32) == 82) {
 index += 10;
} else {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseQuotedMilitaryString(str, index, endIndex, tokener);
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

public static int ParseMethod(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = ParseLdhStr(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
index = indexTemp2;
} else {
 break;
}
 }
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 47)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseMethodVersion(str, index, endIndex, tokener);
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

public static int ParseMethodVersion(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
 while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseMethodspec(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseMethod(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseMilitaryString(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 for (i = 0; i < 69; ++i) {
  indexTemp2 = index;
 do {
if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
 ++indexTemp2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] == 32) ||
  (str[index] == 39) || (str[index] >= 43 && str[index] <= 58) ||
  (str[index] == 61) || (str[index] == 63))) {
 ++indexTemp2; break;
}
 } while (false);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
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

public static int ParseMilitaryStringSequence(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0; i < 69; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 for (i2 = 0; i2 < 69; ++i2) {
  indexTemp3 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
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

public static int ParseMixerKeyword(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 45))) {
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

public static int ParseMsgId(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart; break;
}
 tx2 = ParseIdLeft(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 64)) {
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
if (index < endIndex && (str[index] == 62)) {
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

public static int ParseMvElement(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 86 && str[index + 2] == 61) {
 index += 3;
} else {
 break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx2 = ParseCertifierList(str, index, endIndex, tokener);
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

public static int ParseNameAddr(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseDisplayName(str, index, endIndex, tokener);
 tx2 = ParseAngleAddr(str, index, endIndex, tokener);
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

public static int ParseNewsgroupList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
 tx2 = ParseNewsgroupName(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseNewsgroupName(str, index, endIndex, tokener);
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
while (index < endIndex && ((str[index] == 32) || (str[index] == 9))) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseNewsgroupName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 43) || (str[index] == 45) || (str[index] == 95))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 43) || (str[index] == 45) ||
   (str[index] == 95))) {
 ++index;
}
} else {
 break;
}
 while (true) {
  state2 = indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 43) || (str[index] == 45) || (str[index] == 95))) {
 ++index;
 while (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 43) || (str[index] == 45) ||
   (str[index] == 95))) {
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

public static int ParseNoEncodedWords(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseObsUnstruct(str, index, endIndex, tokener);
}

public static int ParseNoFoldLiteral(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 91)) {
 ++index;
} else {
 break;
}
 while (true) {
  indexTemp2 = ParseDtext(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
index = indexTemp2;
} else {
 break;
}
 }
if (index < endIndex && (str[index] == 93)) {
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

public static int ParseNoResult(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) ==
  69) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseNodeid(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 45 && str[index] <= 46) || (str[index] == 95) ||
  (str[index] == 126))) {
 ++indexTemp2; break;
}
if (index + 2 < endIndex && (((str[index] == 37) && (((str[index + 1] >= 48&&
  str[index + 1] <= 57) || (str[index + 1] >= 65 && str[index + 1] <= 70)||
  (str[index + 1] >= 97 && str[index + 1] <= 102)) && ((str[index + 2] >=
  48 && str[index + 2] <= 57) || (str[index + 2] >= 65 && str[index + 2] <=
  70) || (str[index + 2] >= 97 && str[index + 2] <= 102)))))) {
 indexTemp2 += 3; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] == 36) ||
  (str[index] >= 40 && str[index] <= 44) || (str[index] == 59) ||
  (str[index] == 61))) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseNonnegInteger(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 48)) {
 ++indexTemp; break;
}
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseObsAcceptLanguage(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
 tx2 = ParseObsLanguageQ(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseObsLanguageQ(str, index, endIndex, tokener);
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

public static int ParseObsDomainList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3,
  state, state2, tx2, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
  indexTemp3 = ParseCFWS(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
if (index < endIndex && (str[index] == 44)) {
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
if (index < endIndex && (str[index] == 64)) {
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
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 44)) {
 ++index;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 64)) {
 ++index;
} else {
 break;
}
 tx4 = ParseDomain(str, index, endIndex, tokener);
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

public static int ParseObsGroupList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexStart2, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 44)) {
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
  if (i< 1) {
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

public static int ParseObsLanguageQ(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseLanguageRange(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && ((str[index] == 81) || (str[index] == 113))) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
 ++index;
} else {
 index = indexStart2; break;
}
 tx3 = ParseQvalue(str, index, endIndex, tokener);
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

public static int ParseObsRoute(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseObsDomainList(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
if (index < endIndex && (str[index] == 58)) {
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

public static int ParseObsUnstruct(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i3, i4, indexStart, indexStart3, indexStart4, indexTemp, indexTemp2,
  indexTemp3, indexTemp4, indexTemp5, state, state2, state3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
while (index < endIndex && (str[index] == 13)) {
 ++index;
}
 for (i3 = 0;; ++i3) {
  indexTemp4 = index;
 do {
 indexTemp5 = index;
 do {
if (index < endIndex && ((str[index] == 0) || (str[index] >= 1 && str[index]
  <= 8) || (str[index] >= 11 && str[index] <= 12) || (str[index] >= 14 &&
  str[index] <= 31) || (str[index] == 127) || (str[index] >= 33 &&
  str[index] <= 126) || (str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp5; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp5 += 2; break;
}
 } while (false);
 if (indexTemp5 != index) {
 indexTemp4 = indexTemp5; break;
}
 indexStart4 = index;
for (i4 = 0; true; ++i4) {
  indexTemp5 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp5 == index) { if (i4< 1) {
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
  if (i3< 1) {
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
if (index < endIndex && (str[index] == 10)) {
 ++indexTemp2;
 while (indexTemp2 < endIndex && (str[indexTemp2] == 10)) {
indexTemp2++;
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
while (index < endIndex && (str[index] == 13)) {
 ++index;
}
  indexTemp = index;
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseOptParameterList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParseParameter(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseOtherSections(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index + 1 < endIndex && (str[index] == 42) && (str[index + 1] >= 49 &&
  str[index + 1] <= 57)) {
 index += 2;
} else {
 break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}

public static int ParseParameter(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart3, indexStart4, indexTemp, indexTemp2, indexTemp3,
  indexTemp4, indexTemp5, state, state3, tx5;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
  indexTemp3 = ParseRegularParameter(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
do {
  indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
   (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
   <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
   str[index] <= 126))) {
 ++index;
}
} else {
 break;
}
if (index + 1 < endIndex && str[index] == 42 && str[index + 1] == 48) {
 index += 2;
}
if (index < endIndex && (str[index] == 42)) {
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
if (index < endIndex && (str[index] == 61)) {
 ++index;
} else {
 index = indexStart3; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp4 = index;
 do {
 indexStart4 = index;
 index = ParseCharset(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 39)) {
 ++index;
} else {
 index = indexStart4; break;
}
 index = ParseLanguageTag(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 39)) {
 ++index;
} else {
 index = indexStart4; break;
}
 while (true) {
  indexTemp5 = index;
 do {
if (index + 2 < endIndex && (((str[index] == 37) && (((str[index + 1] >= 48&&
  str[index + 1] <= 57) || (str[index + 1] >= 65 && str[index + 1] <= 70)||
  (str[index + 1] >= 97 && str[index + 1] <= 102)) && ((str[index + 2] >=
  48 && str[index + 2] <= 57) || (str[index + 2] >= 65 && str[index + 2] <=
  70) || (str[index + 2] >= 97 && str[index + 2] <= 102)))))) {
 indexTemp5 += 3; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
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
 indexStart3 = index;
do {
  indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
   (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
   <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
   str[index] <= 126))) {
 ++index;
}
} else {
 break;
}
 tx5 = ParseOtherSections(str, index, endIndex, tokener);
 if (tx5 == index) {
index = indexStart4; break;
} else {
 index = tx5;
}
if (index < endIndex && (str[index] == 42)) {
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
if (index < endIndex && (str[index] == 61)) {
 ++index;
} else {
 index = indexStart3; break;
}
 while (true) {
  indexTemp4 = index;
 do {
if (index + 2 < endIndex && (((str[index] == 37) && (((str[index + 1] >= 48&&
  str[index + 1] <= 57) || (str[index + 1] >= 65 && str[index + 1] <= 70)||
  (str[index + 1] >= 97 && str[index + 1] <= 102)) && ((str[index + 2] >=
  48 && str[index + 2] <= 57) || (str[index + 2] >= 65 && str[index + 2] <=
  70) || (str[index + 2] >= 97 && str[index + 2] <= 102)))))) {
 indexTemp4 += 3; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
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

public static int ParsePath(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 60)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 62)) {
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

public static int ParsePathIdentity(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, i4, indexStart, indexStart2, indexStart3, indexStart4, indexTemp,
  indexTemp2, indexTemp3, indexTemp4, indexTemp5, state, state2, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = index;
 do {
 indexStart3 = index;
 tx4 = ParseLabel(str, index, endIndex, tokener);
 if (tx4 == index) {
 break;
} else {
 index = tx4;
}
if (index < endIndex && (str[index] == 46)) {
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
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
do {
  indexTemp3 = index;
 do {
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
  break;
}
 for (i4 = 0;; ++i4) {
  indexTemp5 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp5 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
 ++indexTemp5; break;
}
 } while (false);
  if (indexTemp5 != index) {
 index = indexTemp5;
} else {
  if (i4< 1) {
   index = indexStart4;
  } break;
 }
 }
 if (index == indexStart4) {
 break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
 indexTemp4 = index;
 do {
 indexStart4 = index;
if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index] == 45))) {
 ++index;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
 index = indexStart4; break;
}
 while (true) {
  indexTemp5 = index;
 do {
if (index + 1 < endIndex && ((str[index] == 45) && ((str[index + 1] >= 65&&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <=
  122) || (str[index + 1] >= 48 && str[index + 1] <= 57)))) {
 indexTemp5 += 2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
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
  indexTemp2 = index;
  index = indexStart2;
 } while (false);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 if (tokener != null) {
 tokener.RestoreState(state2);
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] == 45) || (str[index] == 95))) {
 ++indexTemp;
 while (indexTemp < endIndex && ((str[indexTemp] >= 65 && str[indexTemp] <=
   90) || (str[indexTemp] >= 97 && str[indexTemp] <= 122) || (str[indexTemp]
   >= 48 && str[indexTemp] <= 57) || (str[indexTemp] == 45) ||
   (str[indexTemp] == 95))) {
indexTemp++;
}
 break;
}
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParsePathList(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexStart4, indexTemp, indexTemp2, indexTemp3,
  indexTemp4, state, state2, state4, tx3, tx5;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParsePathIdentity(str, index, endIndex, tokener);
 if (tx3 == index) {
 break;
} else {
 index = tx3;
}
 index = ParseFWS(str, index, endIndex, tokener);
do {
  indexTemp3 = index;
 do {
  state4 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp4 = index;
 do {
 indexStart4 = index;
 tx5 = ParseDiagOther(str, index, endIndex, tokener);
 if (tx5 == index) {
 break;
} else {
 index = tx5;
}
if (index < endIndex && (str[index] == 33)) {
 ++index;
} else {
 index = indexStart4; break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
 if (tokener != null) {
 tokener.RestoreState(state4);
}
  state4 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp4 = index;
 do {
 indexStart4 = index;
 tx5 = ParseDiagDeprecated(str, index, endIndex, tokener);
 if (tx5 == index) {
 break;
} else {
 index = tx5;
}
if (index < endIndex && (str[index] == 33)) {
 ++index;
} else {
 index = indexStart4; break;
}
  indexTemp4 = index;
  index = indexStart4;
 } while (false);
 if (indexTemp4 != index) {
 indexTemp3 = indexTemp4; break;
}
 if (tokener != null) {
 tokener.RestoreState(state4);
}
if (index + 1 < endIndex && str[index] == 33 && str[index + 1] == 33) {
 indexTemp3 += 2; break;
}
if (index < endIndex && (str[index] == 33)) {
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

public static int ParsePathxmpp(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 while (true) {
  indexTemp3 = ParseNodeid(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
index = indexTemp3;
} else {
 break;
}
 }
if (index < endIndex && (str[index] == 64)) {
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
  indexTemp2 = ParseRegName(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
index = indexTemp2;
} else {
 break;
}
 }
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 47)) {
 ++index;
} else {
 break;
}
 while (true) {
  indexTemp3 = ParseResid(str, index, endIndex, tokener);
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

public static int ParsePhrase(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParsePhraseWord(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 while (true) {
  indexTemp2 = ParsePhraseWordOrDot(str, index, endIndex, tokener);
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

public static int ParsePhraseAtom(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0;; ++i) {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++indexTemp2; break;
}
if (index < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index] == 33) || (str[index] == 35) || (str[index] == 36) ||
  (str[index] == 37) || (str[index] == 38) || (str[index] == 39) ||
  (str[index] == 42) || (str[index] == 43) || (str[index] == 45) ||
  (str[index] == 47) || (str[index] == 61) || (str[index] == 63) ||
  (str[index] == 94) || (str[index] == 95) || (str[index] == 96) ||
  (str[index] == 123) || (str[index] == 124) || (str[index] == 125) ||
  (str[index] == 126))) {
 ++indexTemp2; break;
}
 indexTemp3 = index;
 do {
if (index < endIndex && ((str[index] >= 128 && str[index] <= 55295) ||
  (str[index] >= 57344 && str[index] <= 65535))) {
 ++indexTemp3; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
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
  if (i< 1) {
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

public static int ParsePhraseAtomOrDot(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 indexTemp2 = index;
for (i = 0; true; ++i) {
  indexTemp2 = ParseAtext(str, index, endIndex, tokener);
  if (indexTemp2 == index) { if (i< 1) {
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
if (index < endIndex && (str[index] == 46)) {
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

public static int ParsePhraseWord(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParsePhraseAtom(str, index, endIndex, tokener);
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

public static int ParsePhraseWordOrDot(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
 tx3 = ParsePhraseAtomOrDot(str, index, endIndex, tokener);
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

public static int ParsePrecedence(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart3, indexTemp, indexTemp2, indexTemp3, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
do {
  indexTemp2 = index;
 do {
if (index < endIndex && (str[index] == 48)) {
 ++indexTemp2; break;
}
 indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] >= 49 && str[index] <= 57)) {
 ++index;
} else {
 break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
 ++index;
}
  indexTemp3 = index;
  index = indexStart3;
 } while (false);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 68 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) ==
  69&&
  (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 69 && (str[index + 7] & ~32) == 68) {
 indexTemp2 += 8; break;
}
if (index + 6 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 73 && (str[index + 5] & ~32) == 78 &&
  (str[index + 6] & ~32) == 69) {
 indexTemp2 += 7; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) ==
  79&&
  (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 73 &&
  (str[index + 6] & ~32) == 84 && (str[index + 7] & ~32) == 89) {
 indexTemp2 += 8; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 73 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) ==
  69&&
  (str[index + 4] & ~32) == 68 && (str[index + 5] & ~32) == 73 &&
  (str[index + 6] & ~32) == 65 && (str[index + 7] & ~32) == 84 && (str[index+
  8] & ~32) == 69) {
 indexTemp2 += 9; break;
}
if (index + 4 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 76 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) ==
  83&&
  (str[index + 4] & ~32) == 72) {
 indexTemp2 += 5; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 79 && (str[index + 1] & ~32) == 86 && (str[index + 2] & ~32) == 69 && (str[index + 3] & ~32) ==
  82&&
  (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 73 &&
  (str[index + 6] & ~32) == 68 && (str[index + 7] & ~32) == 69) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParsePrintablestring(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = index;
 do {
if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
 ++indexTemp2; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] == 32) ||
  (str[index] == 39) || (str[index] >= 43 && str[index] <= 58) ||
  (str[index] == 61) || (str[index] == 63))) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseProperty(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
if (index + 7 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) ==
  76&&
  (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 79 && (str[index + 7] & ~32) == 77) {
 indexTemp += 8; break;
}
if (index + 5 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 67 && (str[index + 2] & ~32) == 80 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 84 && (str[index + 5] & ~32) == 79) {
 indexTemp += 6; break;
}
 indexTemp2 = index;
 // Unlimited production in choice
 } while (false);
 return indexTemp;
}

public static int ParsePropspec(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParsePtype(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 46)) {
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
if (index < endIndex && (str[index] == 61)) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParsePsChar(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] >= 40 && str[index] <= 41)) {
 ++indexTemp; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] == 32) ||
  (str[index] == 39) || (str[index] >= 43 && str[index] <= 58) ||
  (str[index] == 61) || (str[index] == 63))) {
 ++indexTemp; break;
}
 } while (false);
 return indexTemp;
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParsePtype(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index + 3 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 84 && (str[index + 3] & ~32) ==
  80) {
 indexTemp += 4; break;
}
if (index + 5 < endIndex && (str[index] & ~32) == 72 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) ==
  68&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82) {
 indexTemp += 6; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 66 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 68 && (str[index + 3] & ~32) ==
  89) {
 indexTemp += 4; break;
}
if (index + 5 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 76 && (str[index + 3] & ~32) ==
  73&&
  (str[index + 4] & ~32) == 67 && (str[index + 5] & ~32) == 89) {
 indexTemp += 6; break;
}
 } while (false);
 return indexTemp;
}

public static int ParsePvalue(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart3, indexStart4, indexTemp, indexTemp2, indexTemp3,
  indexTemp4, state, state3, tx4;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++indexTemp2;
 while (indexTemp2 < endIndex && ((str[indexTemp2] == 33) ||
   (str[indexTemp2] >= 35 && str[indexTemp2] <= 36) || (str[indexTemp2] >=
   45 && str[indexTemp2] <= 46) || (str[indexTemp2] >= 48 && str[indexTemp2]
   <= 57) || (str[indexTemp2] >= 65 && str[indexTemp2] <= 90) ||
   (str[indexTemp2] >= 94 && str[indexTemp2] <= 126) || (str[indexTemp2] >=
   42 && str[indexTemp2] <= 43) || (str[indexTemp2] >= 38 && str[indexTemp2]
   <= 39) || (str[indexTemp2] == 63))) {
indexTemp2++;
}
 break;
}
  indexTemp3 = ParseQuotedString(str, index, endIndex, tokener);
 if (indexTemp3 != index) {
 indexTemp2 = indexTemp3; break;
}
  state3 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp3 = index;
 do {
 indexStart3 = index;
do {
  indexTemp4 = index;
 do {
 indexStart4 = index;
 index = ParseLocalPart(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 64)) {
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
 tx4 = ParseDomainName(str, index, endIndex, tokener);
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

public static int ParseQcontent(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 91) || (str[index] >= 93 && str[index] <= 126) ||
  (str[index] >= 1 && str[index] <= 8) || (str[index] >= 11 && str[index] <=
  12) || (str[index] >= 14 && str[index] <= 31) || (str[index] == 127) ||
  (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 &&
  str[index] <= 65535))) {
 ++indexTemp; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp += 2; break;
}
  indexTemp2 = ParseQuotedPair(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseQuotedMilitaryString(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 34)) {
 ++index;
} else {
 break;
}
 for (i = 0; i < 69; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 1) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
if (index < endIndex && (str[index] == 34)) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseQuotedPair(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && (str[index] == 92)) {
 ++index;
} else {
 break;
}
do {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] >= 33 && str[index] <= 126) ||
  (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 &&
  str[index] <= 65535))) {
 ++indexTemp2; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp2 += 2; break;
}
if (index < endIndex && ((str[index] == 32) || (str[index] == 9) ||
  (str[index] == 0) || (str[index] >= 1 && str[index] <= 8) || (str[index]
  >= 11 && str[index] <= 12) || (str[index] >= 14 && str[index] <= 31) ||
  (str[index] == 127) || (str[index] == 10) || (str[index] == 13))) {
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

public static int ParseQuotedString(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 34)) {
 ++index;
} else {
 index = indexStart; break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseQcontent(str, index, endIndex, tokener);
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
if (index < endIndex && (str[index] == 34)) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseQvalue(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i3, indexStart, indexStart2, indexStart3, indexTemp, indexTemp2, indexTemp3;
indexStart = index;
 indexTemp = index;
 do {
 indexTemp2 = index;
 do {
 indexStart2 = index;
if (index < endIndex && (str[index] == 48)) {
 ++index;
} else {
 break;
}
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
for (i3 = 0; i3 < 3; ++i3) {
 if (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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
 indexStart2 = index;
if (index < endIndex && (str[index] == 49)) {
 ++index;
} else {
 break;
}
do {
  indexTemp3 = index;
 do {
 indexStart3 = index;
if (index < endIndex && (str[index] == 46)) {
 ++index;
} else {
 break;
}
for (i3 = 0; i3 < 3; ++i3) {
 if (index < endIndex && (str[index] == 48)) {
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

public static int ParseReasonspec(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 5 < endIndex && (str[index] & ~32) == 82 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) ==
  83&&
  (str[index + 4] & ~32) == 79 && (str[index + 5] & ~32) == 78) {
 index += 6;
} else {
 break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
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

public static int ParseReceivedToken(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  indexTemp2 = ParseAngleAddr(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseRegName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 45 && str[index] <= 46) || (str[index] == 95) ||
  (str[index] == 126))) {
 ++indexTemp2; break;
}
if (index + 2 < endIndex && (((str[index] == 37) && (((str[index + 1] >= 48&&
  str[index + 1] <= 57) || (str[index + 1] >= 65 && str[index + 1] <= 70)||
  (str[index + 1] >= 97 && str[index + 1] <= 102)) && ((str[index + 2] >=
  48 && str[index + 2] <= 57) || (str[index + 2] >= 65 && str[index + 2] <=
  70) || (str[index + 2] >= 97 && str[index + 2] <= 102)))))) {
 indexTemp2 += 3; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] == 36) ||
  (str[index] >= 38 && str[index] <= 44) || (str[index] == 59) ||
  (str[index] == 61))) {
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

public static int ParseRegularParameter(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseRegularParameterName(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 61)) {
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

public static int ParseRegularParameterName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
  (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
  str[index] <= 126))) {
 ++index;
 while (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] == 43) ||
   (str[index] >= 45 && str[index] <= 46) || (str[index] >= 48 && str[index]
   <= 57) || (str[index] >= 65 && str[index] <= 90) || (str[index] >= 94 &&
   str[index] <= 126))) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseResid(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2;
indexStart = index;
 indexTemp = index;
 do {
 while (true) {
  indexTemp2 = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57) || (str[index] >= 45 && str[index] <= 46) || (str[index] == 95) ||
  (str[index] == 126))) {
 ++indexTemp2; break;
}
if (index < endIndex && ((str[index] == 33) || (str[index] == 36) ||
  (str[index] >= 38 && str[index] <= 44) || (str[index] >= 58 && str[index]
  <= 59) || (str[index] == 61))) {
 ++indexTemp2; break;
}
if (index + 2 < endIndex && (((str[index] == 37) && (((str[index + 1] >= 48&&
  str[index + 1] <= 57) || (str[index + 1] >= 65 && str[index + 1] <= 70)||
  (str[index + 1] >= 97 && str[index + 1] <= 102)) && ((str[index + 2] >=
  48 && str[index + 2] <= 57) || (str[index + 2] >= 65 && str[index + 2] <=
  70) || (str[index + 2] >= 97 && str[index + 2] <= 102)))))) {
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

public static int ParseResinfo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, state2, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart; break;
}
 tx2 = ParseMethodspec(str, index, endIndex, tokener);
 if (tx2 == index) {
index = indexStart; break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseCFWS(str, index, endIndex, tokener);
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
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 tx3 = ParseCFWS(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseRestrictedName(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 57))) {
 ++index;
} else {
  break;
}
for (i = 0; i < 126; ++i) {
 if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
   (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 &&
   str[index] <= 57) || (str[index] == 33) || (str[index] >= 35 &&
   str[index] <= 36) || (str[index] == 38) || (str[index] >= 94 &&
   str[index] <= 95) || (str[index] >= 45 && str[index] <= 46) ||
   (str[index] == 43))) {
  ++index;
 } else {
 break;
}
}
  indexTemp = index;
 } while (false);
 return indexTemp;
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseResult(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index + 3 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) ==
  83) {
 indexTemp += 4; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 70 && (str[index + 1] & ~32) == 65 && (str[index + 2] & ~32) == 73 && (str[index + 3] & ~32) ==
  76) {
 indexTemp += 4; break;
}
if (index + 7 < endIndex && (str[index] & ~32) == 83 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 70 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 70 && (str[index + 5] & ~32) == 65 &&
  (str[index + 6] & ~32) == 73 && (str[index + 7] & ~32) == 76) {
 indexTemp += 8; break;
}
if (index + 6 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 85 && (str[index + 3] & ~32) ==
  84&&
  (str[index + 4] & ~32) == 82 && (str[index + 5] & ~32) == 65 &&
  (str[index + 6] & ~32) == 76) {
 indexTemp += 7; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 78 && (str[index + 1] & ~32) == 79 && (str[index + 2] & ~32) == 78 && (str[index + 3] & ~32) ==
  69) {
 indexTemp += 4; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 84 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 77 && (str[index + 3] & ~32) ==
  80&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index+
  8] & ~32) == 82) {
 indexTemp += 9; break;
}
if (index + 8 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 69 && (str[index + 2] & ~32) == 82 && (str[index + 3] & ~32) ==
  77&&
  (str[index + 4] & ~32) == 69 && (str[index + 5] & ~32) == 82 &&
  (str[index + 6] & ~32) == 82 && (str[index + 7] & ~32) == 79 && (str[index+
  8] & ~32) == 82) {
 indexTemp += 9; break;
}
 } while (false);
 return indexTemp;
}

public static int ParseSection(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index + 1 < endIndex && str[index] == 42 && str[index + 1] == 48) {
 indexTemp += 2; break;
}
  indexTemp2 = ParseOtherSections(str, index, endIndex, tokener);
 if (indexTemp2 != index) {
 indexTemp = indexTemp2; break;
}
 } while (false);
 if (tokener != null && indexTemp == indexStart) {
 tokener.RestoreState(state);
}
 return indexTemp;
}

public static int ParseSicSequence(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i, i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3,
  state, state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 for (i = 0; i < 8; ++i) {
  indexTemp2 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp2 != index) {
 index = indexTemp2;
} else {
  if (i< 3) {
   index = indexStart;
  } break;
 }
 }
 if (index == indexStart) {
 break;
}
 while (true) {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 for (i2 = 0; i2 < 8; ++i2) {
  indexTemp3 = ParsePsChar(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 3) {
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

public static int ParseSioLabelParmSeq(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state, tx2, tx3;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseParameter(str, index, endIndex, tokener);
 if (tx2 == index) {
 break;
} else {
 index = tx2;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 59)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseFWS(str, index, endIndex, tokener);
 tx3 = ParseSioLabelParmSeq(str, index, endIndex, tokener);
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseSolicitationKeywords(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state2;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122))) {
 ++index;
} else {
  break;
}
while (index < endIndex && ((str[index] >= 45 && str[index] <= 46) ||
  (str[index] == 95) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 58))) {
 ++index;
}
 while (true) {
  state2 = indexTemp2 = index;
 do {
 indexStart2 = index;
if (index + 1 < endIndex && (str[index] == 44) && ((str[index + 1] >= 65 &&
  str[index + 1] <= 90) || (str[index + 1] >= 97 && str[index + 1] <= 122))) {
 index += 2;
} else {
 break;
}
while (index < endIndex && ((str[index] >= 45 && str[index] <= 46) ||
  (str[index] == 95) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 97 && str[index] <= 122) || (str[index] >= 48 && str[index]
  <= 58))) {
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

public static int ParseStrictHeaderTo(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
 return ParseAddressList(str, index, endIndex, tokener);
}
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseText(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] >= 1 && str[index] <= 9) || (str[index]
  == 11) || (str[index] == 12) || (str[index] >= 14 && str[index] <= 127) ||
  (str[index] >= 128 && str[index] <= 55295) || (str[index] >= 57344 &&
  str[index] <= 65535))) {
 ++indexTemp; break;
}
if (index + 1 < endIndex && ((str[index] >= 55296 && str[index] <= 56319) &&
  (str[index + 1] >= 56320 && str[index + 1] <= 57343))) {
 indexTemp += 2; break;
}
 } while (false);
 return indexTemp;
}

public static int ParseTime(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state, tx2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 tx2 = ParseTimeOfDay(str, index, endIndex, tokener);
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

public static int ParseTimeOfDay(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexStart2, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
do {
  indexTemp2 = index;
 do {
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
if (index < endIndex && (str[index] == 58)) {
 ++index;
} else {
 index = indexStart2; break;
}
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
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
#if CODE_ANALYSIS
[System.Diagnostics.CodeAnalysis.SuppressMessage(
  "Microsoft.Usage",
  "CA1801",
  Justification = "Tokener argument appears for consistency with other Parse* methods defined here.")]
#endif
public static int ParseTypeString(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp;
indexStart = index;
 indexTemp = index;
 do {
if (index + 2 < endIndex && (str[index] & ~32) == 65 && (str[index + 1] & ~32) == 76 && (str[index + 2] & ~32) == 76) {
 indexTemp += 3; break;
}
if (index + 3 < endIndex && (str[index] & ~32) == 76 && (str[index + 1] & ~32) == 73 && (str[index + 2] & ~32) == 83 && (str[index + 3] & ~32) ==
  84) {
 indexTemp += 4; break;
}
if (index + 10 < endIndex && (str[index] & ~32) == 84 && (str[index + 1] & ~32) == 82 && (str[index + 2] & ~32) == 65 && (str[index + 3] & ~32) ==
  78&&
  (str[index + 4] & ~32) == 83 && (str[index + 5] & ~32) == 65 &&
  (str[index + 6] & ~32) == 67 && (str[index + 7] & ~32) == 84 && (str[index+
  8] & ~32) == 73 && (str[index + 9] & ~32) == 79 && (str[index + 10] & ~32)
    == 78) {
 indexTemp += 11; break;
}
 } while (false);
 return indexTemp;
}

public static int ParseValue(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
if (index < endIndex && ((str[index] == 33) || (str[index] >= 35 &&
  str[index] <= 36) || (str[index] >= 45 && str[index] <= 46) || (str[index]
  >= 48 && str[index] <= 57) || (str[index] >= 65 && str[index] <= 90) ||
  (str[index] >= 94 && str[index] <= 126) || (str[index] >= 42 && str[index]
  <= 43) || (str[index] >= 38 && str[index] <= 39) || (str[index] == 63))) {
 ++indexTemp;
 while (indexTemp < endIndex && ((str[indexTemp] == 33) || (str[indexTemp]
   >= 35 && str[indexTemp] <= 36) || (str[indexTemp] >= 45 && str[indexTemp]
   <= 46) || (str[indexTemp] >= 48 && str[indexTemp] <= 57) ||
   (str[indexTemp] >= 65 && str[indexTemp] <= 90) || (str[indexTemp] >= 94&&
   str[indexTemp] <= 126) || (str[indexTemp] >= 42 && str[indexTemp] <=
   43) || (str[indexTemp] >= 38 && str[indexTemp] <= 39) || (str[indexTemp]
   == 63))) {
indexTemp++;
}
 break;
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

public static int ParseWord(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, indexTemp2, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
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

public static int ParseYear(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int indexStart, indexTemp, state;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
 index = ParseCFWS(str, index, endIndex, tokener);
if (index + 1 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57))) {
 index += 2;
} else {
 index = indexStart; break;
}
while (index < endIndex && (str[index] >= 48 && str[index] <= 57)) {
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

public static int ParseZone(
  string str,
  int index,
  int endIndex,
  ITokener tokener) {
int i2, indexStart, indexStart2, indexTemp, indexTemp2, indexTemp3, state,
  state2;
indexStart = index;
 state = (tokener != null) ? tokener.GetState() : 0;
 indexTemp = index;
 do {
  state2 = (tokener != null) ? tokener.GetState() : 0;
 indexTemp2 = index;
 do {
 indexStart2 = index;
 for (i2 = 0;; ++i2) {
  indexTemp3 = ParseFWS(str, index, endIndex, tokener);
  if (indexTemp3 != index) {
 index = indexTemp3;
} else {
  if (i2< 1) {
   index = indexStart2;
  } break;
 }
 }
 if (index == indexStart2) {
 break;
}
if (index < endIndex && ((str[index] == 43) || (str[index] == 45))) {
 ++index;
} else {
 index = indexStart2; break;
}
if (index + 3 < endIndex && ((str[index] >= 48 && str[index] <= 57) ||
  (str[index + 1] >= 48 && str[index + 1] <= 57) || (str[index + 2] >= 48 &&
  str[index + 2] <= 57) || (str[index + 3] >= 48 && str[index + 3] <= 57))) {
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
 indexStart2 = index;
 index = ParseCFWS(str, index, endIndex, tokener);
do {
  indexTemp3 = index;
 do {
if (index + 1 < endIndex && (str[index] & ~32) == 85 && (str[index + 1] & ~32) == 84) {
 indexTemp3 += 2; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 71 && (str[index + 1] & ~32) == 77 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 69 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 67 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 77 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 83 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index + 2 < endIndex && (str[index] & ~32) == 80 && (str[index + 1] & ~32) == 68 && (str[index + 2] & ~32) == 84) {
 indexTemp3 += 3; break;
}
if (index < endIndex && ((str[index] >= 65 && str[index] <= 73) ||
  (str[index] >= 75 && str[index] <= 90) || (str[index] >= 97 && str[index]
  <= 105) || (str[index] >= 107 && str[index] <= 122))) {
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
}
