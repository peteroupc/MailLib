package com.upokecenter.text.encoders;

import java.io.*;

import com.upokecenter.util.*;

import com.upokecenter.text.*;

  public class EncoderHelper {
    public static String InputToString(ICharacterInput reader) {
      StringBuilder builder = new StringBuilder();
      while (true) {
        int c = reader.ReadChar();
        if (c < 0) {
          break;
        }
        if (c <= 0xffff) {
          builder.append((char)c);
        } else if (c <= 0x10ffff) {
          builder.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          builder.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
      return builder.toString();
    }
  }
