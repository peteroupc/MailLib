package com.upokecenter.mail.transforms;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;

    /**
     * Normalizes bare CR and bare LF to CRLF.
     */
  public class LineBreakNormalizeTransform implements IByteReader
  {
    private InputStream stream;
    private int val;
    private boolean cr;
    private boolean supportBareLF;

    /**
     * Initializes a new instance of the LineBreakNormalizeTransform class.
     * @param stream A InputStream object.
     * @param supportBareLF A Boolean object.
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
          }
          if (ret == 0x0d && this.supportBareLF) {
            this.cr = true;
            this.val = 0x0a;
            return 0x0d;
          }
          return ret;
        }
      } catch (IOException ex) {
        throw new MessageDataException(ex.getMessage(), ex);
      }
    }
  }
