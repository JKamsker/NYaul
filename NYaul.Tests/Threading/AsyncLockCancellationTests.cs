using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NYaul.Threading;

namespace NYaul.Tests.Threading
{
    public class AsyncLockCancellationTests
    {
        [Fact]
        public async Task LockAsync_WithCancellation_CancelsWhenRequested()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var cts = new CancellationTokenSource();
            
            // Acquire the lock first so next call will wait
            using (await asyncLock.LockAsync())
            {
                // Act & Assert
                var lockTask = asyncLock.LockAsync(cts.Token);
                
                // Cancel the token after a short delay
                await Task.Delay(50);
                cts.Cancel();
                
                // Verify the task was canceled
                await Assert.ThrowsAsync<OperationCanceledException>(() => lockTask);
            }
        }
        
        [Fact]
        public async Task LockAsync_WithAlreadyCancelledToken_ThrowsImmediately()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(50); 
            
            asyncLock.LockAsync();
            
            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => 
                await asyncLock.LockAsync(cts.Token));
        }
        
        [Fact]
        public async Task LockAsync_CancellationDoesNotAffectOtherWaiters()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var cts = new CancellationTokenSource();
            var lockAcquired = false;
            
            // First, acquire the lock
            using (await asyncLock.LockAsync())
            {
                // Try to acquire with a token that will be cancelled
                var cancelTask = Task.Run(async () => {
                    try {
                        using (await asyncLock.LockAsync(cts.Token)) {
                            // This should not execute
                            Assert.True(false, "Cancelled task should not acquire lock");
                        }
                    }
                    catch (OperationCanceledException) {
                        // Expected
                    }
                });
                
                // Try to acquire without a cancellation token
                var successTask = Task.Run(async () => {
                    using (await asyncLock.LockAsync()) {
                        lockAcquired = true;
                    }
                });
                
                // Wait a bit and then cancel the token
                await Task.Delay(50);
                cts.Cancel();
                
                // Wait a bit more to let other tasks run
                await Task.Delay(100);
            }
            
            // Give the successTask time to complete
            await Task.Delay(100);
            
            // Assert
            Assert.True(lockAcquired, "Lock should have been acquired by the non-cancelled task");
        }
    }
}
