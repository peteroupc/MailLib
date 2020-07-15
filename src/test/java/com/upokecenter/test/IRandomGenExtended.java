package com.upokecenter.test; import com.upokecenter.util.*;

  public interface IRandomGenExtended extends IRandomGen {
    int GetInt32(int maxExclusive);

    long GetInt64(long maxExclusive);
  }
