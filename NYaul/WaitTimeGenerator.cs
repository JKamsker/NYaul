using System;
using System.Collections.Generic;

namespace NYaul;

public class WaitTimeGenerator
{
    public int MaxRetries { get; }
    public TimeSpan BaseTime { get; }
    public TimeSpan MaxWaitTime { get; }
    public bool ReturnNullOnMaxRetries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitTimeGenerator"/> class with specified settings for retry logic.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts. Specifies how many times a retry should be attempted before giving up. Must be a non-negative integer.</param>
    /// <param name="baseTime">The base wait time before the first retry attempt. This serves as the initial delay and influences the exponential backoff calculation for subsequent retries. The wait time grows exponentially based on this value but will not exceed <paramref name="maxWaitTime"/>.</param>
    /// <param name="maxWaitTime">The maximum wait time between retries. This value caps the wait time, ensuring that the delay between retry attempts does not become excessively long, regardless of the number of attempts.</param>
    /// <param name="returnNullOnMaxRetries">Determines the behavior once the maximum number of retries (<paramref name="maxRetries"/>) is reached. If set to <c>true</c>, <see cref="GetWaitTime"/> will return <c>null</c> for any retry attempt beyond the maximum, indicating that no further retries should be attempted. If <c>false</c>, it will continue to return the <paramref name="maxWaitTime"/> for any count beyond <paramref name="maxRetries"/>, potentially allowing retries to continue indefinitely at the maximum interval. Defaults to <c>true</c>.</param>
    /// <remarks>
    /// The <see cref="WaitTimeGenerator"/> class provides a flexible way to manage retry intervals in applications, using an exponential backoff strategy. It is designed to help handle transient failures by intelligently spacing out retry attempts, with the ability to configure the initial delay, the growth rate of the delay, and the maximum delay, as well as the overall behavior after the maximum number of retries is exceeded.
    /// </remarks>
    public WaitTimeGenerator(int maxRetries, TimeSpan baseTime, TimeSpan maxWaitTime, bool returnNullOnMaxRetries = true)
    {
        MaxRetries = maxRetries;
        BaseTime = baseTime;
        MaxWaitTime = maxWaitTime;
        ReturnNullOnMaxRetries = returnNullOnMaxRetries;
    }

    public TimeSpan? GetWaitTime(int retryCount, int? maxRetryOverride = null)
    {
        var maxRetries = maxRetryOverride ?? MaxRetries;
        if (ReturnNullOnMaxRetries && retryCount > maxRetries)
        {
            return null;
        }

        double factor = Math.Pow(MaxWaitTime.TotalSeconds / BaseTime.TotalSeconds, 1.0 / maxRetries);
        double waitTimeInSeconds = BaseTime.TotalSeconds * Math.Pow(factor, retryCount);
        waitTimeInSeconds = Math.Min(waitTimeInSeconds, MaxWaitTime.TotalSeconds);
        return TimeSpan.FromSeconds(waitTimeInSeconds);
    }

    public IEnumerable<TimeSpan?> GetWaitTimes()
    {
        for (int i = 0; ; i++)
        {
            var time = GetWaitTime(i);
            if (time == null)
            {
                yield break;
            }

            yield return time;
        }
    }

    public static WaitTimeGeneratorBuilder Create => new WaitTimeGeneratorBuilder();
}

public class WaitTimeGeneratorBuilder
{
    private int? _maxRetries;
    private TimeSpan? _baseTime;
    private TimeSpan? _maxWaitTime;
    private bool? _returnNullOnMaxRetries;

    public static WaitTimeGeneratorBuilder Create => new WaitTimeGeneratorBuilder();

    public WaitTimeGeneratorBuilder WithMaxRetries(int maxRetries)
    {
        _maxRetries = maxRetries;
        _returnNullOnMaxRetries = true;
        return this;
    }

    /// <summary>
    /// Alias of <see cref="WithBaseTime"/>.
    /// </summary>
    public WaitTimeGeneratorBuilder WithMinWaitTime(TimeSpan baseTime)
        => WithBaseTime(baseTime);

    public WaitTimeGeneratorBuilder WithBaseTime(TimeSpan baseTime)
    {
        _baseTime = baseTime;
        return this;
    }

    public WaitTimeGeneratorBuilder WithMaxWaitTime(TimeSpan maxWaitTime)
    {
        _maxWaitTime = maxWaitTime;
        return this;
    }

    public WaitTimeGeneratorBuilder MaxWaitTimeAfter(int maxRetries)
    {
        _maxRetries = maxRetries;
        _returnNullOnMaxRetries = false;
        return this;
    }

    public WaitTimeGenerator Build()
    {
        if (_maxRetries == null)
        {
            throw new InvalidOperationException("MaxRetries must be set.");
        }

        if (_baseTime == null)
        {
            throw new InvalidOperationException("BaseTime must be set.");
        }

        if (_maxWaitTime == null)
        {
            throw new InvalidOperationException("MaxWaitTime must be set.");
        }

        return new WaitTimeGenerator(_maxRetries.Value, _baseTime.Value, _maxWaitTime.Value, _returnNullOnMaxRetries ?? false);
    }
}