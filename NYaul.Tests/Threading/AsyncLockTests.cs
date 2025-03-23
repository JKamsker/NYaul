using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using NYaul.Threading;

namespace NYaul.Tests.Threading
{
    public class AsyncLockTests
    {
        [Fact]
        public async Task LockAsync_EnsuresExclusiveAccess()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var sharedResource = 0;
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (await asyncLock.LockAsync())
                    {
                        // Simulate some work that reads and updates the shared resource
                        var temp = sharedResource;
                        await Task.Delay(10); // Simulate some processing time
                        sharedResource = temp + 1;
                    }
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(10, sharedResource);
        }

        [Fact]
        public async Task LockAsync_PreventsConcurrentAccess()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var isLocked = false;
            var concurrentAccess = false;

            // Act
            var task1 = Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    isLocked = true;
                    await Task.Delay(100); // Hold the lock for 100ms
                    isLocked = false;
                }
            });

            // Give task1 time to acquire the lock
            await Task.Delay(20);

            var task2 = Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    // If we get here and task1 still has the lock set to true,
                    // then we have concurrent access which is a problem
                    if (isLocked)
                    {
                        concurrentAccess = true;
                    }
                }
            });

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.False(concurrentAccess);
        }

        [Fact]
        public async Task LockAsync_PermitsSequentialAccess()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var accessCount = 0;
            var sequenceCheck = new List<int>();

            // Act
            var task1 = Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    accessCount++;
                    sequenceCheck.Add(1);
                    await Task.Delay(50);
                }
            });

            var task2 = Task.Run(async () =>
            {
                using (await asyncLock.LockAsync())
                {
                    accessCount++;
                    sequenceCheck.Add(2);
                    await Task.Delay(50);
                }
            });

            await Task.WhenAll(task1, task2);

            // Assert
            Assert.Equal(2, accessCount);
            Assert.Equal(2, sequenceCheck.Count);
        }

        [Fact]
        public void Dispose_ReleasesResources()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            
            // Act
            asyncLock.Dispose();
            
            // Assert - verify that attempting to use the lock after disposal throws
            Assert.ThrowsAsync<ObjectDisposedException>(async () => await asyncLock.LockAsync());
        }

        [Fact]
        public async Task MultipleLockReleases_WorksCorrectly()
        {
            // Arrange
            var asyncLock = new AsyncLock();
            var tasks = new List<Task>();

            // Act & Assert
            // Acquire and release the lock multiple times sequentially
            for (int i = 0; i < 5; i++)
            {
                using (await asyncLock.LockAsync())
                {
                    // Just acquiring and releasing should work without errors
                }
            }

            // Now try concurrent lock requests
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (await asyncLock.LockAsync())
                    {
                        await Task.Delay(10);
                    }
                }));
            }

            // If we can complete all tasks without deadlock, the test passes
            var completedTask = await Task.WhenAny(
                Task.WhenAll(tasks),
                Task.Delay(TimeSpan.FromSeconds(5))
            );

            Assert.Equal(tasks.Count, tasks.Count(t => t.Status == TaskStatus.RanToCompletion));
        }
    }
}
