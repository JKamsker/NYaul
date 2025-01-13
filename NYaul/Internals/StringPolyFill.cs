using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NYaul.Internals;
internal static class StringPolyfill
{
    /// <summary>
    /// Gets a substring between two strings. The first on is at the start and the second one is at the end.
    /// </summary>
    public static bool TryTrimStartAndEnd(this string input, string start, string end, out string result)
    {
        if (string.IsNullOrEmpty(input))
        {
            result = string.Empty;
            return false;
        }

        if (input.StartsWith(start) && input.EndsWith(end))
        {
            // result = input.Substring(start.Length, input.Length - start.Length - end.Length);
            result = input[start.Length..^end.Length];
            return true;
        }

        result = string.Empty;
        return false;
    }

#if NET48 || NETSTANDARD2_0

    public static bool Contains(this string source, string value, StringComparison comparisonType)
    {
        return source.IndexOf(value, comparisonType) >= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith(this string source, char value)
    {
        return source[^1] == value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith(this string source, char value)
    {
        return source[0] == value;
    }



    public static string Join(char separator, string[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        if (values.Length == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder();
        result.Append(values[0]);
        for (int i = 1; i < values.Length; i++)
        {
            result.Append(separator);
            result.Append(values[i]);
        }
        return result.ToString();
    }

    public static string Join(char separator, IEnumerable<string> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        var result = new StringBuilder();
        using (var enumerator = values.GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                result.Append(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    result.Append(separator);
                    result.Append(enumerator.Current);
                }
            }
        }

        return result.ToString();
    }
    

#else
    public static string Join(char separator, string[] values)
    {
        return string.Join(separator, values);
    }
    
    public static string Join(char separator, IEnumerable<string> values)
    {
        return string.Join(separator, values);
    }
#endif
}