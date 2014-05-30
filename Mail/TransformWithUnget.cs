/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;

namespace PeterO.Mail {
  internal sealed class TransformWithUnget : ITransform {
    private ITransform transform;
    private int lastByte;
    private bool unget;

    public TransformWithUnget(ITransform stream) {
      this.lastByte = -1;
      this.transform = stream;
    }

    public int ReadByte() {
      if (this.unget) {
        this.unget = false;
      } else {
        this.lastByte = this.transform.ReadByte();
      }
      return this.lastByte;
    }

    public void Unget() {
      this.unget = true;
    }
  }
}
