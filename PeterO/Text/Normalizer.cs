/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Text {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Text.Normalizer"]/*'/>
  [Obsolete(
  "Use NormalizingCharacterInput instead; that class is much more flexible than Normalizer.")]
  public sealed class Normalizer {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.Normalizer.Normalize(System.String,PeterO.Text.Normalization)"]/*'/>
    public static string Normalize(string str, Normalization form) {
      return NormalizingCharacterInput.Normalize(str, form);
    }

    private readonly NormalizingCharacterInput nci;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.Normalizer.#ctor(System.String,PeterO.Text.Normalization)"]/*'/>
    public Normalizer(string str, Normalization form) {
      this.nci = new NormalizingCharacterInput(str, form);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.Normalizer.IsNormalized(System.String,PeterO.Text.Normalization)"]/*'/>
    public static bool IsNormalized(string str, Normalization form) {
      return NormalizingCharacterInput.IsNormalized(str, form);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.Normalizer.ReadChar"]/*'/>
    public int ReadChar() {
      return this.nci.ReadChar();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Text.Normalizer.Read(System.Int32[],System.Int32,System.Int32)"]/*'/>
    public int Read(int[] chars, int index, int length) {
      return this.nci.Read(chars, index, length);
    }
  }
}
