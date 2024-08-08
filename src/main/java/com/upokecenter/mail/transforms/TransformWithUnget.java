package com.upokecenter.mail.transforms;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import com.upokecenter.util.*;
import com.upokecenter.mail.*;

  // TODO: Refactor to eliminate this class
  public final class TransformWithUnget implements IByteReader {
    private final IByteReader transform;
    private int lastByte;
    private boolean unget;

    public TransformWithUnget(IByteReader stream) {
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
