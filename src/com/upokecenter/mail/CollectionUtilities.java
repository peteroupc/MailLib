package com.upokecenter.mail;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

  final class CollectionUtilities {
private CollectionUtilities() {
}
    public static <TKey, TValue> boolean MapEquals(Map<TKey, TValue> mapA, Map<TKey, TValue> mapB) {
      if (mapA == null) {
        return mapB == null;
      }
      if (mapB == null) {
        return false;
      }
      if (mapA.size() != mapB.size()) {
        return false;
      }
      for(Map.Entry<TKey, TValue> kvp : mapA.entrySet()) {
        TValue valueB = null;
        boolean hasKey;
valueB=mapB.get(kvp.getKey());
hasKey=(valueB==null) ? mapB.containsKey(kvp.getKey()) : true;
        if (hasKey) {
          TValue valueA = kvp.getValue();
          if (!(((valueA)==null) ? ((valueB)==null) : (valueA).equals(valueB))) {
            return false;
          }
        } else {
          return false;
        }
      }
      return true;
    }
  }
