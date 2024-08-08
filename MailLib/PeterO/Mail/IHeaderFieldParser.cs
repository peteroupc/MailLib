/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;

namespace PeterO.Mail {
  internal interface IHeaderFieldParser {
    bool IsStructured();

    int Parse(string str, int index, int endIndex, ITokener tokener);

    string DowngradeHeaderField(string name, string str);

    string DecodeEncodedWords(string str);
  }
}
