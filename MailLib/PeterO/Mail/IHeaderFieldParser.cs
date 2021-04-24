/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
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
