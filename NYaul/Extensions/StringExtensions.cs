using System;
using System.Collections.Generic;
using System.Text;

namespace System;

/// <summary>
/// Provides extension methods for string collections that facilitate joining operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Concatenates the elements of an <see cref="IEnumerable{String}"/> collection using the specified separator.
    /// </summary>
    /// <param name="values">An enumerable collection of strings to join.</param>
    /// <param name="separator">The string separator to use between elements.</param>
    /// <returns>A single string that consists of the elements of <paramref name="values"/> delimited by <paramref name="separator"/>.</returns>
    public static string Join(this IEnumerable<string> values, string separator)
    {
        return string.Join(separator, values);
    }

    /// <summary>
    /// Concatenates the elements of a string array using the specified character as a separator.
    /// </summary>
    /// <param name="values">An array of strings to join.</param>
    /// <param name="separator">The character separator to insert between elements.</param>
    /// <returns>A single string that consists of the array elements separated by <paramref name="separator"/>.</returns>
    public static string Join(this string[] values, char separator)
    {
        return StringPolyfill.Join(separator, values);
    }

    /// <summary>
    /// Concatenates the elements of an <see cref="IEnumerable{String}"/> collection using the specified character as a separator.
    /// </summary>
    /// <param name="values">An enumerable collection of strings to join.</param>
    /// <param name="separator">The character to use as a separator between elements.</param>
    /// <returns>A single string that consists of the collection elements separated by <paramref name="separator"/>.</returns>
    public static string Join(this IEnumerable<string> values, char separator)
    {
        return StringPolyfill.Join(separator, values);
    }
}
