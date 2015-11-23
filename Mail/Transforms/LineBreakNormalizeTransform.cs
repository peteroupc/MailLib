/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.IO;
using PeterO;
using PeterO.Mail;

namespace PeterO.Mail.Transforms {
    /// <summary>Normalizes bare CR and bare LF to CRLF.</summary>
  internal class LineBreakNormalizeTransform : IByteReader
  {
    private readonly Stream stream;
    private int val;
    private bool cr;
    private readonly bool supportBareLF;

    /// <summary>Initializes a new instance of the
    /// LineBreakNormalizeTransform class.</summary>
    /// <param name='stream'>A Stream object.</param>
    /// <param name='supportBareLF'>A Boolean object.</param>
    public LineBreakNormalizeTransform(Stream stream, bool supportBareLF) {
      this.stream = stream;
      this.val = -1;
      this.supportBareLF = supportBareLF;
    }

    public int ReadByte() {
      try {
        if (this.val >= 0) {
          int ret = this.val;
          this.val = -1;
          return ret;
        } else {
          int ret = this.stream.ReadByte();
          if (this.cr && ret == 0x0a) {
            // Ignore LF if CR was just read
            ret = this.stream.ReadByte();
          }
          this.cr = ret == 0x0d;
          if (ret == 0x0a) {
            this.val = 0x0a;
            return 0x0d;
          }
          if (ret == 0x0d && this.supportBareLF) {
            this.cr = true;
            this.val = 0x0a;
            return 0x0d;
          }
          return ret;
        }
      } catch (IOException ex) {
        throw new MessageDataException(ex.Message, ex);
      }
    }
  }
}
