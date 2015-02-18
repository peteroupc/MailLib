package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

  final class TransformWithUnget implements ITransform {
    private ITransform transform;
    private int lastByte;
    private boolean unget;

    public TransformWithUnget (ITransform stream) {
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
