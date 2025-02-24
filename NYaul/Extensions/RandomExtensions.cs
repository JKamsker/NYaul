﻿using System;
using System.Text;

//using Random = NYaul.Internals.RandomEx;

namespace System;

/// <summary>
/// Provides extension methods for the Random class to generate various types of random values.
/// </summary>
public static class RandomExtensions
{
    private const string DefaultCharset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Generates a random TimeSpan between specified millisecond values.
    /// </summary>
    /// <param name="random">The Random instance.</param>
    /// <param name="min">The minimum number of milliseconds.</param>
    /// <param name="max">The maximum number of milliseconds.</param>
    /// <returns>A TimeSpan with a random duration between min and max milliseconds.</returns>
    public static TimeSpan NextMilliseconds(this Random random, int min, int max)
    {
        return TimeSpan.FromMilliseconds(random.Next(min, max));
    }

    /// <summary>
    /// Generates a random TimeSpan between two specified TimeSpan values.
    /// </summary>
    /// <param name="random">The Random instance.</param>
    /// <param name="min">The minimum TimeSpan value.</param>
    /// <param name="max">The maximum TimeSpan value.</param>
    /// <returns>A TimeSpan with a random duration between min and max.</returns>
    /// <remarks>
    /// The resolution is in milliseconds, and the values are internally converted to milliseconds for the random calculation.
    /// </remarks>
    public static TimeSpan NextTimeSpan(this Random random, TimeSpan min, TimeSpan max)
    {
        return random.NextMilliseconds((int)min.TotalMilliseconds, (int)max.TotalMilliseconds);
    }

    /// <summary>
    /// Generates a random string of specified length using the provided character set.
    /// </summary>
    /// <param name="random">The Random instance.</param>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="charset">The set of characters to use for generation. Defaults to alphanumeric characters.</param>
    /// <returns>A randomly generated string of the specified length.</returns>
    /// <remarks>
    /// The default character set includes lowercase letters, uppercase letters, and digits (a-z, A-Z, 0-9).
    /// </remarks>
    public static string NextString(this Random random, int length, string charset = DefaultCharset)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(charset[random.Next(charset.Length)]);
        }
        return sb.ToString();
    }
}
