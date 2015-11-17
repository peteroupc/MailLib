package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */

import java.util.*;

  final class Tokener implements ITokener, Comparator<int[]> {
    private ArrayList<int[]> tokenStack = new ArrayList<int[]>();

    public int GetState() {
      return this.tokenStack.size();
    }

    public void RestoreState(int state) {
      // if (tokenStack.size() != state) {
      // System.out.println("Rolling back from " + tokenStack.size() + " to "
      // + state);
      // }
      while (state < this.tokenStack.size()) {
        this.tokenStack.remove(state);
      }
    }

    public void Commit(int token, int startIndex, int endIndex) {
      // System.out.println("Committing token " + token + ", size now " +
      // (tokenStack.size() + 1));
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
     */
    public int compare(int[] x, int[] y) {
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
