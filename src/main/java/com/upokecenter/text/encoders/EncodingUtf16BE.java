package com.upokecenter.text.encoders;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

  public class EncodingUtf16BE implements ICharacterEncoding {
   public ICharacterDecoder GetDecoder() {
      return new EncodingUtf16.Decoder(true);
    }

    public ICharacterEncoder GetEncoder() {
      return new EncodingUtf16.Encoder(true);
    }
  }
