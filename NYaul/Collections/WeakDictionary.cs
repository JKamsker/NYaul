using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NYaul.Collections;

public class WeakDictionary<TKey, TValue>
    where TKey : class
    where TValue : class
{
    private readonly ConcurrentDictionary<TKey, WeakReference<WeakEntry>> _dictionary = new();

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_dictionary.TryGetValue(key, out var weakEntry))
        {
            if (weakEntry?.TryGetTarget(out var entry) == true)
            {
                value = entry.Value;
                return true;
            }
            else
            {
                _dictionary.TryRemove(key, out _);
            }
        }

        value = default;

        return false;
    }

    public void Add(TKey key, TValue value)
    {
        _dictionary.AddOrUpdate
        (
            key: key,
            addValue: new WeakReference<WeakEntry>
            (
                new WeakEntry(key, value, (k, v) => _dictionary.TryRemove(k, out _))
            ),
            updateValueFactory: (k, v) =>
            {
                v.SetTarget(new WeakEntry(key, value, (k, v) =>
                {
                    _dictionary.TryRemove(k, out _);
                }));
                return v;
            }
        );
    }

    private class WeakEntry : IDisposable
    {
        private readonly Action<TKey, TValue> _disposeAction;

        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public bool IsDisposed { get; private set; }

        public WeakEntry(TKey key, TValue value, Action<TKey, TValue> disposeAction)
        {
            Key = key;
            Value = value;
            _disposeAction = disposeAction;
        }

        // disposer and finalizer are used to clean up the dictionary when the value is no longer referenced
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;

            _disposeAction(Key, Value);

            Key = null;
            Value = null;
        }

        ~WeakEntry()
        {
            Dispose();
        }

        // equality is based on the key and value
        public override bool Equals(object obj)
        {
            if (obj is WeakEntry other)
            {
                return Key.Equals(other.Key) && Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Value.GetHashCode();
        }

        public static bool operator ==(WeakEntry left, WeakEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WeakEntry left, WeakEntry right)
        {
            return !(left == right);
        }
    }
}