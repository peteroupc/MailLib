package com.upokecenter.util;

import java.io.*;

import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

class EncoderHelper {
 private static class ReaderToInputClass implements ICharacterInput {
   ICharacterReader reader;
   public ReaderToInputClass (ICharacterReader reader) {
     this.reader = reader;
   }
   public int ReadChar() {
     return this.reader.ReadChar();
   }
   public int Read(int[] buffer, int offset, int length) {
    if ((buffer) == null) {
  throw new NullPointerException("buffer");
}
if (offset < 0) {
  throw new IllegalArgumentException("offset (" + offset +
    ") is less than " + 0);
}
if (offset > buffer.length) {
  throw new IllegalArgumentException("offset (" + offset + ") is more than "+
    buffer.length);
}
if (length < 0) {
  throw new IllegalArgumentException("length (" + length +
    ") is less than " + 0);
}
if (length > buffer.length) {
  throw new IllegalArgumentException("length (" + length + ") is more than "+
    buffer.length);
}
if (buffer.length-offset < length) {
  throw new IllegalArgumentException("buffer's length minus " + offset + " (" +
    (buffer.length-offset) + ") is less than " + length);
}
    int count = 0;
    for (int i = 0; i < length; ++i) {
     int c = this.reader.ReadChar();
     if (c< 0) {
 break;
}
      buffer[offset]=c;
     ++count;
     ++offset;
    }
    return count;
   }
 }
 public static ICharacterInput ReaderToInput(ICharacterReader reader) {
   return new ReaderToInputClass(reader);
 }
 public static String ReaderToString(ICharacterReader reader) {
  StringBuilder builder = new StringBuilder();
  while (true) {
    int c = reader.ReadChar();
    if (c< 0) {
 break;
}
    if (c <= 0xffff) { builder.append((char)(c));
  } else if (c <= 0x10ffff) {
builder.append((char)((((c-0x10000) >> 10) & 0x3ff)+0xd800));
builder.append((char)((((c-0x10000)) & 0x3ff)+0xdc00));
}
  }
  return builder.toString();
 }
 public static String InputToString(ICharacterInput reader) {
  StringBuilder builder = new StringBuilder();
  while (true) {
    int c = reader.ReadChar();
    if (c< 0) {
 break;
}
    if (c <= 0xffff) { builder.append((char)(c));
  } else if (c <= 0x10ffff) {
builder.append((char)((((c-0x10000) >> 10) & 0x3ff)+0xd800));
builder.append((char)((((c-0x10000)) & 0x3ff)+0xdc00));
}
  }
  return builder.toString();
 }
 public static int Error(int codePoint, InputStream output, boolean replace) {
  if (!replace) {
   throw new IllegalArgumentException("can't encode code point "+codePoint);
  } else {
   output.write((byte)'&');
   output.write((byte)'#');
   int count = 2;
   if (codePoint == 0) {
    output.write((byte)'0');
     ++count;
   } else {
    while (codePoint>0) {
        output.write((byte)(0x30+(codePoint%10)));
      ++count;
        codePoint/=10;
    }
   }
   output.write((byte)';');
   ++count;
   return count;
  }
 }
