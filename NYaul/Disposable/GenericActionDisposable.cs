using System;

namespace NYaul.Disposable;

/// <summary>
/// Represents a disposable action that invokes a specified callback when disposed.
/// </summary>
public class ActionDisposable : IDisposable
{
    private readonly Action _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDisposable"/> class with the specified dispose action.
    /// </summary>
    /// <param name="action">The action to be called upon disposal.</param>
    public ActionDisposable(Action action)
    {
        _action = action;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDisposable"/> class using an initialization action and a dispose action.
    /// </summary>
    /// <param name="initAction">The action to perform immediately upon construction.</param>
    /// <param name="action">The action to be called upon disposal.</param>
    public ActionDisposable(Action initAction, Action action)
    {
        initAction();
        _action = action;
    }

    /// <summary>
    /// Invokes the disposal action.
    /// </summary>
    public void Dispose()
    {
        _action();
    }

    /// <summary>
    /// Creates a new <see cref="IDisposable"/> that executes the specified action upon disposal.
    /// </summary>
    /// <param name="action">The action to execute when disposed.</param>
    /// <returns>An <see cref="IDisposable"/> object.</returns>
    public static IDisposable Create(Action action)
    {
        return new ActionDisposable(action);
    }

    /// <summary>
    /// Creates a new <see cref="IDisposable"/> that executes an initialization action immediately and a disposal action when disposed.
    /// </summary>
    /// <param name="initAction">The action to execute immediately upon creation.</param>
    /// <param name="action">The action to execute when disposed.</param>
    /// <returns>An <see cref="IDisposable"/> object.</returns>
    public static IDisposable Create(Action initAction, Action action)
    {
        return new ActionDisposable(initAction, action);
    }

    /// <summary>
    /// Creates a new <see cref="IDisposable"/> that performs an action with a starting value immediately,
    /// and then performs the action with an ending value upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the action.</typeparam>
    /// <param name="start">The initial value passed to the action immediately.</param>
    /// <param name="end">The final value passed to the action upon disposal.</param>
    /// <param name="action">The action to perform.</param>
    /// <returns>An <see cref="IDisposable"/> object.</returns>
    public static IDisposable Create<T>(T start, T end, Action<T> action)
    {
        action(start);
        return new GenericActionDisposable<T>(end, action);
    }

    /// <summary>
    /// Represents a generic disposable action that invokes a parameterized action upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of value passed to the action upon disposal.</typeparam>
    private class GenericActionDisposable<T> : IDisposable
    {
        private readonly Action<T> _action;
        private readonly T _end;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericActionDisposable{T}"/> class.
        /// </summary>
        /// <param name="end">The value to pass to the action upon disposal.</param>
        /// <param name="action">The action to be invoked upon disposal.</param>
        public GenericActionDisposable(T end, Action<T> action)
        {
            _action = action;
            _end = end;
        }

        /// <summary>
        /// Invokes the action with the specified ending value.
        /// </summary>
        public void Dispose()
        {
            _action(_end);
        }

        /// <summary>
        /// Creates a new <see cref="IDisposable"/> that performs an action with a starting value immediately,
        /// and then performs the action with an ending value upon disposal.
        /// </summary>
        /// <param name="start">The value to pass to the action immediately.</param>
        /// <param name="end">The value to pass to the action upon disposal.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>An <see cref="IDisposable"/> object.</returns>
        public static IDisposable Create(T start, T end, Action<T> action)
        {
            action(start);
            return new GenericActionDisposable<T>(end, action);
        }
    }
}
