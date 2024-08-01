using System;
using System.Threading.Tasks;

namespace NYaul.Disposable;
#if NETCOREAPP3_0_OR_GREATER
public class AsyncActionDisposable : IAsyncDisposable
{
    private readonly Func<Task> _action;

    public AsyncActionDisposable(Func<Task> action)
    {
        _action = action;
    }

    public async ValueTask DisposeAsync()
    {
        await _action();
    }

    public static IAsyncDisposable Create(Func<Task> action)
    {
        return new AsyncActionDisposable(action);
    }
}

#endif