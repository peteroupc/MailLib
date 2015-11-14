package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

  class EncodingUtf16BE implements ICharacterEncoding {
   public ICharacterDecoder GetDecoder() {
      return new EncodingUtf16.Decoder(true);
    }

    public ICharacterEncoder GetEncoder() {
      return new EncodingUtf16.Encoder(true);
    }
  }
