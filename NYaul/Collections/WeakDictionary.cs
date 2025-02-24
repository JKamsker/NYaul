using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace NYaul.Collections;

/// <summary>
/// Represents a thread-safe dictionary that holds weak references to its values.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary. Must be a reference type.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary. Must be a reference type.</typeparam>
/// <remarks>
/// This dictionary automatically removes entries when their values are garbage collected.
/// It uses weak references to allow values to be collected when they are no longer referenced elsewhere in the application.
/// This is useful for implementing caches or maintaining lookup tables that shouldn't prevent garbage collection.
/// </remarks>
public class WeakDictionary<TKey, TValue>
    where TKey : class
    where TValue : class
{
    private readonly ConcurrentDictionary<TKey, WeakReference<WeakEntry>> _dictionary = new();

    /// <summary>
    /// Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, contains the default value for TValue.</param>
    /// <returns>true if the dictionary contains an element with the specified key and the referenced value still exists; otherwise, false.</returns>
    /// <remarks>
    /// If the key exists but its associated value has been garbage collected, the key will be removed from the dictionary
    /// and the method will return false.
    /// </remarks>
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

    /// <summary>
    /// Adds or updates a key-value pair in the dictionary.
    /// </summary>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <remarks>
    /// If the key already exists, its value will be updated.
    /// The value is stored as a weak reference, allowing it to be garbage collected when no other references exist.
    /// The dictionary maintains internal cleanup logic to remove entries when their values are collected.
    /// </remarks>
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

    /// <summary>
    /// Represents an entry in the weak dictionary that can be garbage collected.
    /// </summary>
    /// <remarks>
    /// This class implements IDisposable and includes a finalizer to ensure proper cleanup
    /// of dictionary entries when they are no longer needed or their values are collected.
    /// </remarks>
    private class WeakEntry : IDisposable
    {
        private readonly Action<TKey, TValue> _disposeAction;

        /// <summary>
        /// Gets the key associated with this entry.
        /// </summary>
        public TKey Key { get; private set; }
        /// <summary>
        /// Gets the value associated with this entry.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this entry has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the WeakEntry class.
        /// </summary>
        /// <param name="key">The key for this entry.</param>
        /// <param name="value">The value for this entry.</param>
        /// <param name="disposeAction">The action to perform when this entry is disposed.</param>
        public WeakEntry(TKey key, TValue value, Action<TKey, TValue> disposeAction)
        {
            Key = key;
            Value = value;
            _disposeAction = disposeAction;
        }

        // disposer and finalizer are used to clean up the dictionary when the value is no longer referenced
        /// <summary>
        /// Performs cleanup of the entry and removes it from the parent dictionary.
        /// </summary>
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
        public override bool Equals(object? obj)
        {
            if (obj is WeakEntry other)
            {
                return Key.Equals(other.Key) && Value.Equals(other.Value);
            }
            return false;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
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
