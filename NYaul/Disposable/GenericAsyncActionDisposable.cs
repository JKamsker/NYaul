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

    public static async Task<IAsyncDisposable> CreateAsync(Func<Task> initAction, Func<Task> action)
    {
        await initAction();
        return new AsyncActionDisposable(action);
    }

    public static async Task<IAsyncDisposable> CreateAsync<T>(T start, T end, Func<T, Task> action)
    {
        await action(start);
        return new GenericAsyncActionDisposable<T>(start, end, action);
    }

    private class GenericAsyncActionDisposable<T> : IAsyncDisposable
    {
        private readonly Func<T, Task> _action;
        private readonly T _end;

        public GenericAsyncActionDisposable(T start, T end, Func<T, Task> action)
        {
            _action = action;
            _end = end;
        }

        public async ValueTask DisposeAsync()
        {
            await _action(_end);
        }
    }
}
#endif