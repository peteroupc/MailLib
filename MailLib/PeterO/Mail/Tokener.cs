/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;

namespace PeterO.Mail {
  internal sealed class Tokener : ITokener, IComparer<int[]> {
    private readonly List<int[]> tokenStack = new List<int[]>();

    public int GetState() {
      return this.tokenStack.Count;
    }

    public void RestoreState(int state) {
      #if DEBUG
      if (state > this.tokenStack.Count) {
        throw new ArgumentException("state(" + state + ") is more than " +
          this.tokenStack.Count);
      }
      if (state < 0) {
        throw new ArgumentException("state(" + state + ") is less than " +
          "0");
      }
      #endif
      while (state < this.tokenStack.Count) {
        this.tokenStack.RemoveAt(state);
      }
    }

    public void Commit(int token, int startIndex, int endIndex) {
      this.tokenStack.Add(new[] { token, startIndex, endIndex });
    }

    public IList<int[]> GetTokens() {
      this.tokenStack.Sort(this);
      return this.tokenStack;
    }

    public int Compare(int[] x, int[] y) {
      if (x == null) {
        throw new ArgumentNullException(nameof(x));
      }
      if (y == null) {
        throw new ArgumentNullException(nameof(y));
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
}
