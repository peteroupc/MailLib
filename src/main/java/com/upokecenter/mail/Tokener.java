package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

import java.util.*;

  final class Tokener implements ITokener, Comparator<int[]> {
    private final ArrayList<int[]> tokenStack = new ArrayList<int[]>();

    public int GetState() {
      return this.tokenStack.size();
    }

    public void RestoreState(int state) {
      while (state < this.tokenStack.size()) {
        this.tokenStack.remove(state);
      }
    }

    public void Commit(int token, int startIndex, int endIndex) {
      this.tokenStack.add(new int[] { token, startIndex, endIndex });
    }

    public List<int[]> GetTokens() {
      java.util.Collections.sort(this.tokenStack, this);
      return this.tokenStack;
    }

    /**
     * Compares one integer array with another.
     * @param x An integer array.
     * @param y An integer array. (2).
     * @return Zero if both values are equal; a negative number if {@code x} is
     * less than {@code y}, or a positive number if {@code x} is greater
     * than {@code y}.
     * @throws NullPointerException The parameter {@code x} or {@code y} is null.
     */
    public int compare(int[] x, int[] y) {
      if (x == null) {
  throw new NullPointerException("x");
}
      if (y == null) {
  throw new NullPointerException("y");
}
      // Sort by their start indexes
      if (x[1] == y[1]) {
        // Sort by their token numbers
        // NOTE: Some parsers rely on the ordering
        // of token numbers, particularly if one token
        // contains another. In this case, the containing
        // token has a lower number than the contained
        // token.
        return (x[0] == y[0]) ? 0 : ((x[0] < y[0]) ? -1 : 1);
      }
      return (x[1] < y[1]) ? -1 : 1;
    }
  }
