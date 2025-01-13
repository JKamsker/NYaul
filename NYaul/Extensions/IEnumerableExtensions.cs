using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.Linq;

public static class EnumerableExtensions
{
    /// <summary>
    /// Skips elements until the predicate returns true
    /// </summary>
    /// <param name="source">The source enumerable</param>
    /// <param name="predicate">The predicate to skip until</param>
    /// <typeparam name="TSource">The type of the source enumerable</typeparam>
    /// <returns>The enumerable with elements skipped until the predicate returns true</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source or predicate is null</exception>
    public static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        var hasSkipped = false;
        foreach (var item in source)
        {
            if (hasSkipped || predicate(item))
            {
                hasSkipped = true;
                yield return item;
            }
        }
    }

    /// <summary>
    /// Skips elements until the predicate returns true, then skips the first element that satisfies the predicate
    /// </summary>
    /// <param name="source">The source enumerable</param>
    /// <param name="predicate">The predicate to skip until</param>
    /// <typeparam name="TSource">The type of the source enumerable</typeparam>
    /// <returns>The enumerable with elements skipped until the predicate returns true, then skips the first element that satisfies the predicate</returns>
    public static IEnumerable<TSource> SkipUntilAfter<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        => source.SkipUntil(predicate).Skip(1);
}