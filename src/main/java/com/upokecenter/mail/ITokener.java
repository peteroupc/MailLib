package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

  interface ITokener {
    int GetState();

    void RestoreState(int state);

    void Commit(int token, int startIndex, int endIndex);
  }
