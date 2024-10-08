package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
