package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.io.*;

    /**
     * Normalizes bare CR and bare LF to CRLF.
     */
  class LineBreakNormalizeTransform implements ITransform
  {
    private InputStream stream;
    private int val;
    private boolean cr;
    private boolean supportBareLF;

    /**
     * Initializes a new instance of the LineBreakNormalizeTransform
     * class.
     */
    public LineBreakNormalizeTransform (InputStream stream, boolean supportBareLF) {
      this.stream = stream;
      this.val = -1;
      this.supportBareLF = supportBareLF;
    }

public int read() {
      try {
        if (this.val >= 0) {
          int ret = this.val;
          this.val = -1;
          return ret;
        } else {
          int ret = this.stream.read();
          if (this.cr && ret == 0x0a) {
            // Ignore LF if CR was just read
            ret = this.stream.read();
          }
          this.cr = ret == 0x0d;
          if (ret == 0x0a) {
            this.val = 0x0a;
            return 0x0d;
          } else if (ret == 0x0d && this.supportBareLF) {
            this.cr = true;
            this.val = 0x0a;
            return 0x0d;
          } else {
            return ret;
          }
        }
      } catch (IOException ex) {
        throw new MessageDataException(ex.getMessage(), ex);
      }
    }
  }
