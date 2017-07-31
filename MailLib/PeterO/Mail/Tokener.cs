/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
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
        throw new ArgumentException("state (" + state + ") is more than " +
          this.tokenStack.Count);
      }
      if (state < 0) {
      throw new ArgumentException("state (" + state + ") is less than " +
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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.Tokener.Compare(System.Int32[],System.Int32[])"]/*'/>
    public int Compare(int[] x, int[] y) {
      if (x == null) {
  throw new ArgumentNullException("x");
}
      if (y == null) {
  throw new ArgumentNullException("y");
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
