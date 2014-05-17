package com.upokecenter.util;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System.Reflection;

    /**
     * Description of DebugUtility.
     */
  class DebugUtility
  {
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(String str) {
      Type type = Type.GetType("System.Console");
      type.GetMethod("WriteLine", new Type[] { String.class }).Invoke(type, new Object[] { str });
    }
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Log(String format, Object... args) {
      Log(String.Format(format, args));
    }
  }
