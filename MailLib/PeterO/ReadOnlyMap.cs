/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;

namespace PeterO {
  internal sealed class ReadOnlyMap<TKey, TValue> :
    IDictionary<TKey, TValue> {
    private readonly IDictionary<TKey, TValue> wrapped;

    public ReadOnlyMap(IDictionary<TKey, TValue> wrapped) {
      this.wrapped = wrapped;
    }

    public TValue this[TKey key] {
      get {
        return this.wrapped[key];
      }

      set {
        throw new NotSupportedException();
      }
    }

    public ICollection<TKey> Keys {
      get {
        return this.wrapped.Keys;
      }
    }

    public ICollection<TValue> Values {
      get {
        return this.wrapped.Values;
      }
    }

    public int Count {
      get {
        return this.wrapped.Count;
      }
    }

    public bool IsReadOnly {
      get {
        return true;
      }
    }

    public bool ContainsKey(TKey key) {
      return this.wrapped.ContainsKey(key);
    }

    public void Add(TKey key, TValue value) {
      throw new NotSupportedException();
    }

    public bool Remove(TKey key) {
      throw new NotSupportedException();
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return this.wrapped.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    public void Clear() {
      throw new NotSupportedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return this.wrapped.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      this.wrapped.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
      throw new NotSupportedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return this.wrapped.GetEnumerator();
    }
    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
      return this.wrapped.GetEnumerator();
    }
  }
}
