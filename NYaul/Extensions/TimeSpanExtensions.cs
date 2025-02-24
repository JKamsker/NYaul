﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System;

/// <summary>
/// Provides extension methods for the TimeSpan structure to enable async/await functionality.
/// </summary>
public static class TimeSpanExtensions
{
    // Awaiter
    /// <summary>
    /// Enables awaiting on a TimeSpan value to create a delay.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan value to wait for.</param>
    /// <returns>A TaskAwaiter that can be used to await the specified duration.</returns>
    /// <remarks>
    /// This extension method allows using a TimeSpan directly in an await expression.
    /// Example: await TimeSpan.FromSeconds(5);
    /// </remarks>
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
    {
        return Task.Delay(timeSpan).GetAwaiter();
    }

    /// <summary>
    /// Enables awaiting on a nullable TimeSpan value to create an optional delay.
    /// </summary>
    /// <param name="timeSpan">The nullable TimeSpan value to wait for.</param>
    /// <returns>A TaskAwaiter that can be used to await the specified duration, or completes immediately if the TimeSpan is null.</returns>
    /// <remarks>
    /// This extension method allows using a nullable TimeSpan directly in an await expression.
    /// If the TimeSpan is null, it completes immediately using Task.CompletedTask.
    /// Example: await (TimeSpan?)null; // Completes immediately
    /// </remarks>
    public static TaskAwaiter GetAwaiter(this TimeSpan? timeSpan)
    {
        return timeSpan?.GetAwaiter() ?? Task.CompletedTask.GetAwaiter();
    }
}
