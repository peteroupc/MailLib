/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;

namespace PeterO {
    /// <summary>Description of CollectionUtilities.</summary>
  internal static class CollectionUtilities
  {
    public static bool MapEquals<TKey, TValue>(IDictionary<TKey, TValue> mapA, IDictionary<TKey, TValue> mapB) {
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
  }
}
