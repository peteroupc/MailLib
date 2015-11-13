package com.upokecenter.util;

import java.io.*;
import com.upokecenter.util.*;
import com.upokecenter.mail.*;
import com.upokecenter.text.*;

/// <summary>
/// </summary>
public interface ICharacterEncoder {
  /**
   * Not documented yet.
   */
  int Encode(
   ICharacterReader reader,
   InputStream output,
   boolean replace);
