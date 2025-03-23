using System;
using System.Threading;
using System.Threading.Tasks;

namespace NYaul.Threading;

public class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    public async Task<IDisposable> LockAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(AsyncLock));
        }
        
        await _semaphore.WaitAsync().ConfigureAwait(false);
        return new Releaser(this);
    }

    public async Task<IDisposable> LockAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(AsyncLock));
        }
        
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Releaser(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _semaphore.Dispose();
        }

        _disposed = true;
    }

    private class Releaser : IDisposable
    {
        private readonly AsyncLock _asyncLock;
        private bool _disposed;

        public Releaser(AsyncLock asyncLock)
        {
            _asyncLock = asyncLock;
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _asyncLock._semaphore.Release();
            _disposed = true;
        }
    }
}
