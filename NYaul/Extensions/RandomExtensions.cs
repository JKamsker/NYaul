using System;
using System.Text;

//using Random = NYaul.Internals.RandomEx;

namespace System;

public static class RandomExtensions
{
    private const string DefaultCharset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // NextMilliseconds
    public static TimeSpan NextMilliseconds(this Random random, int min, int max)
    {
        return TimeSpan.FromMilliseconds(random.Next(min, max));
    }

    public static TimeSpan NextTimeSpan(this Random random, TimeSpan min, TimeSpan max)
    {
        return random.NextMilliseconds((int)min.TotalMilliseconds, (int)max.TotalMilliseconds);
    }

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