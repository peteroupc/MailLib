package com.upokecenter.text;

import java.io.*;
import com.upokecenter.util.*;

import com.upokecenter.text.*;

  class EncodingGBK implements ICharacterEncoding {
    private ICharacterEncoder enc = EncodingGB18030.GetEncoder2(true);

    public ICharacterDecoder GetDecoder() {
      return EncodingGB18030.GetDecoder2();
    }

    public ICharacterEncoder GetEncoder() {
      return this.enc;
    }
  }
