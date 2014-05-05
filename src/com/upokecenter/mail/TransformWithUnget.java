package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;
import java.io.*;

  final class TransformWithUnget implements ITransform {
    private ITransform stream;
    private int lastByte;
    private boolean unget;

    public TransformWithUnget (ITransform stream) {
      this.lastByte = -1;
      this.stream = stream;
    }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
    public int read() {
      if (this.unget) {
        this.unget = false;
      } else {
        this.lastByte = this.stream.read();
      }
      return this.lastByte;
    }

    /**
     * Not documented yet.
     */
    public void Unget() {
      this.unget = true;
    }
  }
