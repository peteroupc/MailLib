package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */

  interface IHeaderFieldParser {
    boolean IsStructured();

    int Parse(String str, int index, int endIndex, ITokener tokener);

    String DowngradeHeaderField(String name, String str);

    String DecodeEncodedWords(String str);
  }
