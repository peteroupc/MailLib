package com.upokecenter.text;

    /**
     * An interface for reading Unicode characters.
     */
  public interface ICharacterInput {
    /**
     * Reads a Unicode character from a data source.
     */
    int ReadChar();

    /**
     * Reads a sequence of Unicode characters from a data source.
     */
    int Read(int[] chars, int index, int length);
  }
