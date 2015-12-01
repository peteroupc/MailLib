/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
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
      // if (tokenStack.Count != state) {
      // Console.WriteLine("Rolling back from " + tokenStack.Count + " to "
      // + state);
      // }
      while (state < this.tokenStack.Count) {
        this.tokenStack.RemoveAt(state);
      }
    }

    public void Commit(int token, int startIndex, int endIndex) {
      // Console.WriteLine("Committing token " + token + ", size now " +
      // (tokenStack.Count + 1));
      this.tokenStack.Add(new[] { token, startIndex, endIndex });
    }

    public IList<int[]> GetTokens() {
      this.tokenStack.Sort(this);
      return this.tokenStack;
    }

    /// <summary>Compares one integer array with another.</summary>
    /// <param name='x'>An integer array.</param>
    /// <param name='y'>An integer array. (2).</param>
    /// <returns>Zero if both values are equal; a negative number if
    /// <paramref name='x'/> is less than <paramref name='y'/>, or a
    /// positive number if <paramref name='x'/> is greater than <paramref
    /// name='y'/>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='x'/> or <paramref name='y'/> is null.</exception>
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
