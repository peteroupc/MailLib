package com.upokecenter.text;

    /**
     * An interface for reading Unicode characters.
     */
  public interface ICharacterInput {
    int ReadChar();

    int Read(int[] chars, int index, int length);
  }
