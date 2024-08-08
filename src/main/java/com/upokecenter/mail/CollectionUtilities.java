package com.upokecenter.mail;
/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */

import java.util.*;

  final class CollectionUtilities {
private CollectionUtilities() {
}
    public static <TKey, TValue> boolean MapEquals(
      Map<TKey, TValue> mapA,
      Map<TKey, TValue> mapB) {
      if (mapA == null) {
        return mapB == null;
      }
      if (mapB == null) {
        return false;
      }
      if (mapA.size() != mapB.size()) {
        return false;
      }
      for (Map.Entry<TKey, TValue> kvp : mapA.entrySet()) {
        TValue valueB = null;
        boolean hasKey;
valueB = mapB.get(kvp.getKey());
hasKey=(valueB == null) ? mapB.containsKey(kvp.getKey()) : true;
        if (hasKey) {
          TValue valueA = kvp.getValue();
          if (!(((valueA) == null) ? ((valueB) == null) : (valueA).equals(valueB))) {
            return false;
          }
        } else {
          return false;
        }
      }
      return true;
    }

    public static <T> boolean ListEquals(List<T> listA, List<T> listB) {
      if (listA == null) {
        return listB == null;
      }
      if (listA.size() != listB.size()) {
        return false;
      }
      for (int i = 0; i < listA.size(); ++i) {
        if (!(((listA.get(i)) == null) ? ((listB.get(i)) == null) : (listA.get(i)).equals(listB.get(i)))) {
          return false;
        }
      }
      return true;
    }
  }
