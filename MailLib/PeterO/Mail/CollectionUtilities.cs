/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;

namespace PeterO.Mail {
  internal static class CollectionUtilities {
    public static bool MapEquals<TKey, TValue>(
      IDictionary<TKey, TValue> mapA,
      IDictionary<TKey, TValue> mapB) {
      if (mapA == null) {
        return mapB == null;
      }
      if (mapB == null) {
        return false;
      }
      if (mapA.Count != mapB.Count) {
        return false;
      }
      foreach (KeyValuePair<TKey, TValue> kvp in mapA) {
        TValue valueB = default(TValue);
        bool hasKey = mapB.TryGetValue(kvp.Key, out valueB);
        if (hasKey) {
          TValue valueA = kvp.Value;
          if (!Object.Equals(valueA, valueB)) {
            return false;
          }
        } else {
          return false;
        }
      }
      return true;
    }

    public static bool ListEquals<T>(IList<T> listA, IList<T> listB) {
      if (listA == null) {
        return listB == null;
      }
      if (listA.Count != listB.Count) {
        return false;
      }
      for (var i = 0; i < listA.Count; ++i) {
        if (!Object.Equals(listA[i], listB[i])) {
          return false;
        }
      }
      return true;
    }
  }
}
