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

    public static IDisposable Create(Action initAction, Action action)
    {
        return new ActionDisposable(initAction, action);
    }
}