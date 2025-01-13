using System;
using System.Collections.Generic;
using System.Text;

namespace System;

public static class StringExtensions
{
    public static string Join(this IEnumerable<string> values, string separator)
    {
        return string.Join(separator, values);
    }

    public static string Join(this string[] values, char separator)
    {
        return StringPolyfill.Join(separator, values);
    }

    public static string Join(this IEnumerable<string> values, char separator)
    {
        return StringPolyfill.Join(separator, values);
    }
}