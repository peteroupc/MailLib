package com.upokecenter.text;

    /**
     * An interface for reading Unicode characters.
     */
  public interface ICharacterInput {
    /**
     * Reads a Unicode character from a data source.
     * @return The Unicode character read, from U + 0000 to U + 10FFFF. Returns
     * -1 if the end of the source is reached.
     */
    int ReadChar();

    /**
     * Reads a sequence of Unicode characters from a data source.
     */
    int Read(int[] chars, int index, int length);
  }
