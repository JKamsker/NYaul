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

    /// <summary>
    /// Calculates the wait time for a specific retry attempt using exponential backoff.
    /// </summary>
    /// <param name="retryCount">The current retry attempt number (0-based index).</param>
    /// <param name="maxRetryOverride">Optional override for the maximum number of retries. If provided, this value will be used instead of the instance's MaxRetries value.</param>
    /// <returns>A TimeSpan representing the calculated wait time, or null if ReturnNullOnMaxRetries is true and the retry count exceeds the maximum retries.</returns>
    /// <remarks>
    /// The wait time is calculated using an exponential backoff strategy, where each subsequent retry wait time increases exponentially,
    /// but is capped at the specified MaxWaitTime. The growth rate is calculated to reach MaxWaitTime after the specified number of retries.
    /// </remarks>
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

    /// <summary>
    /// Generates a sequence of wait times for all possible retry attempts.
    /// </summary>
    /// <returns>An enumerable sequence of TimeSpan values representing the wait time for each retry attempt. The sequence ends when ReturnNullOnMaxRetries is true and MaxRetries is exceeded.</returns>
    /// <remarks>
    /// This method provides a convenient way to preview all wait times that would be used in a retry sequence.
    /// The sequence will continue indefinitely if ReturnNullOnMaxRetries is false.
    /// </remarks>
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

    /// <summary>
    /// Gets a builder instance for creating a new WaitTimeGenerator with a fluent API.
    /// </summary>
    /// <returns>A new instance of WaitTimeGeneratorBuilder.</returns>
    public static WaitTimeGeneratorBuilder Create => new WaitTimeGeneratorBuilder();
}

/// <summary>
/// A builder class for creating instances of WaitTimeGenerator with a fluent API.
/// </summary>
/// <remarks>
/// This builder provides a convenient way to configure and create WaitTimeGenerator instances
/// with mandatory parameters and optional settings.
/// </remarks>
public class WaitTimeGeneratorBuilder
{
    private int? _maxRetries;
    private TimeSpan? _baseTime;
    private TimeSpan? _maxWaitTime;
    private bool? _returnNullOnMaxRetries;

    /// <summary>
    /// Gets a new instance of the WaitTimeGeneratorBuilder.
    /// </summary>
    /// <returns>A new instance of WaitTimeGeneratorBuilder.</returns>
    public static WaitTimeGeneratorBuilder Create => new WaitTimeGeneratorBuilder();

    /// <summary>
    /// Sets the maximum number of retry attempts and enables returning null after max retries.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <returns>The current builder instance for method chaining.</returns>
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

    /// <summary>
    /// Sets the base wait time for the first retry attempt.
    /// </summary>
    /// <param name="baseTime">The initial wait time before exponential increase.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public WaitTimeGeneratorBuilder WithBaseTime(TimeSpan baseTime)
    {
        _baseTime = baseTime;
        return this;
    }

    /// <summary>
    /// Sets the maximum wait time between retries.
    /// </summary>
    /// <param name="maxWaitTime">The maximum allowed wait time between retries.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public WaitTimeGeneratorBuilder WithMaxWaitTime(TimeSpan maxWaitTime)
    {
        _maxWaitTime = maxWaitTime;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of retries and configures the generator to continue using max wait time after reaching the limit.
    /// </summary>
    /// <param name="maxRetries">The number of retries after which the maximum wait time will be used.</param>
    /// <returns>The current builder instance for method chaining.</returns>
    public WaitTimeGeneratorBuilder MaxWaitTimeAfter(int maxRetries)
    {
        _maxRetries = maxRetries;
        _returnNullOnMaxRetries = false;
        return this;
    }

    /// <summary>
    /// Builds and returns a new WaitTimeGenerator instance with the configured settings.
    /// </summary>
    /// <returns>A new WaitTimeGenerator instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required parameters (MaxRetries, BaseTime, or MaxWaitTime) are not set.</exception>
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
