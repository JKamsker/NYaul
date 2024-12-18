using System;

namespace NYaul.Disposable;

public class ActionDisposable : IDisposable
{
    private readonly Action _action;

    public ActionDisposable(Action action)
    {
        _action = action;
    }

    public ActionDisposable(Action initAction, Action action)
    {
        initAction();
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }

    public static IDisposable Create(Action action)
    {
        return new ActionDisposable(action);
    }

    public static IDisposable Create(Action initAction, Action action)
    {
        return new ActionDisposable(initAction, action);
    }

    public static IDisposable Create<T>(T start, T end, Action<T> action)
    {
        action(start);
        return new GenericActionDisposable<T>(end, action);
    }

    private class GenericActionDisposable<T> : IDisposable
    {
        private readonly Action<T> _action;

        private readonly T _end;

        public GenericActionDisposable(T end, Action<T> action)
        {
            _action = action;
            _end = end;
        }

        public void Dispose()
        {
            _action(_end);
        }

        public static IDisposable Create(T start, T end, Action<T> action)
        {
            return new ActionDisposable<T>(start, end, action);
        }
    }
}