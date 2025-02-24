﻿using System;
using System.Threading.Tasks;

namespace NYaul.Disposable;

#if NETCOREAPP3_0_OR_GREATER
/// <summary>
/// Represents an asynchronous disposable action that executes a specified asynchronous function when disposed.
/// </summary>
public class AsyncActionDisposable : IAsyncDisposable
{
    private readonly Func<Task> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncActionDisposable"/> class with the specified asynchronous dispose action.
    /// </summary>
    /// <param name="action">A function that returns a <see cref="Task"/> to be executed upon disposal.</param>
    public AsyncActionDisposable(Func<Task> action)
    {
        _action = action;
    }

    /// <summary>
    /// Asynchronously performs the disposal action.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _action();
    }

    /// <summary>
    /// Creates a new <see cref="IAsyncDisposable"/> instance that executes the specified asynchronous action upon disposal.
    /// </summary>
    /// <param name="action">A function that returns a <see cref="Task"/> to be executed upon disposal.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> instance.</returns>
    public static IAsyncDisposable Create(Func<Task> action)
    {
        return new AsyncActionDisposable(action);
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="IAsyncDisposable"/> instance after executing an initialization action.
    /// </summary>
    /// <param name="initAction">A function that returns a <see cref="Task"/> to perform initialization.</param>
    /// <param name="action">A function that returns a <see cref="Task"/> to be executed upon disposal.</param>
    /// <returns>A task that represents the asynchronous creation operation, containing an <see cref="IAsyncDisposable"/> instance.</returns>
    public static async Task<IAsyncDisposable> CreateAsync(Func<Task> initAction, Func<Task> action)
    {
        await initAction();
        return new AsyncActionDisposable(action);
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="IAsyncDisposable"/> instance that executes an action with a starting value immediately,
    /// and then executes the action with an ending value upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of the parameter for the action.</typeparam>
    /// <param name="start">The value to be passed to the action immediately.</param>
    /// <param name="end">The value to be passed to the action upon disposal.</param>
    /// <param name="action">A function that accepts a value of type <typeparamref name="T"/> and returns a <see cref="Task"/>.</param>
    /// <returns>A task that represents the asynchronous creation operation, containing an <see cref="IAsyncDisposable"/> instance.</returns>
    public static async Task<IAsyncDisposable> CreateAsync<T>(T start, T end, Func<T, Task> action)
    {
        await action(start);
        return new GenericAsyncActionDisposable<T>(start, end, action);
    }

    /// <summary>
    /// Represents a generic asynchronous disposable action that invokes a parameterized asynchronous function upon disposal.
    /// </summary>
    /// <typeparam name="T">The type of the parameter passed to the dispose action.</typeparam>
    private class GenericAsyncActionDisposable<T> : IAsyncDisposable
    {
        private readonly Func<T, Task> _action;
        private readonly T _end;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAsyncActionDisposable{T}"/> class.
        /// </summary>
        /// <param name="start">The value to pass immediately (unused further).</param>
        /// <param name="end">The value to pass to the asynchronous action upon disposal.</param>
        /// <param name="action">A function that accepts a value of type <typeparamref name="T"/> and returns a <see cref="Task"/>.</param>
        public GenericAsyncActionDisposable(T start, T end, Func<T, Task> action)
        {
            _action = action;
            _end = end;
        }

        /// <summary>
        /// Asynchronously performs the disposal action with the specified ending value.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _action(_end);
        }
    }
}
#endif
