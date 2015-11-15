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

  public final class WrappedStream implements ITransform {
    private InputStream stream;

    public WrappedStream (InputStream stream) {
      this.stream = stream;
    }

    public int read() {
      try {
        return this.stream.read();
      } catch (IOException ex) {
        throw new MessageDataException(ex.getMessage(), ex);
      }
    }
  }
