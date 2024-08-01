using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NYaul.Extensions;

public static class TimeSpanExtensions
{
    // Awaiter
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
    {
        return Task.Delay(timeSpan).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this TimeSpan? timeSpan)
    {
        return timeSpan?.GetAwaiter() ?? Task.CompletedTask.GetAwaiter();
    }
}