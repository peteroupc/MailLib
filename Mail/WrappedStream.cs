/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.IO;

namespace PeterO.Mail {
  internal sealed class WrappedStream : ITransform {
    private Stream stream;

    public WrappedStream(Stream stream) {
      this.stream = stream;
    }

    public int ReadByte() {
      try {
        return this.stream.ReadByte();
      } catch (IOException ex) {
        throw new MessageDataException(ex.Message, ex);
      }
    }
  }
}
