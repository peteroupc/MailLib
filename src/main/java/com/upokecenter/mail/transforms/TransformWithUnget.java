package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  public final class TransformWithUnget implements IByteReader {
    private IByteReader transform;
    private int lastByte;
    private boolean unget;

    public TransformWithUnget (IByteReader stream) {
      this.lastByte = -1;
      this.transform = stream;
    }

    public int read() {
      if (this.unget) {
        this.unget = false;
      } else {
        this.lastByte = this.transform.read();
      }
      return this.lastByte;
    }

    public void Unget() {
      this.unget = true;
    }
  }