namespace Roblox.Sentinels;

using System;


/// <summary>
/// A simple class to implement an exponential backoff
/// </summary>
public static class ExponentialBackoff
{
    private const byte _CeilingForMaxAttempts = 10;
    private static readonly ThreadLocalRandom _Random = new();

    /// <summary>
    /// Calculates the timespan for the backoff.
    /// </summary>
    /// <param name="attempt">The current attempt</param>
    /// <param name="maxAttempts">The maximum amount of attempts</param>
    /// <param name="baseDelay">The base delay to start at</param>
    /// <param name="maxDelay">The maximum delay to end at</param>
    /// <param name="jitter">A randomness to add to the delay</param>
    /// <returns>The calculated backoff time</returns>
    public static TimeSpan CalculateBackoff(byte attempt, byte maxAttempts, TimeSpan baseDelay, TimeSpan maxDelay, Jitter jitter = Jitter.None)
        => CalculateBackoff(attempt, maxAttempts, baseDelay, maxDelay, jitter, () => _Random.NextDouble());

    internal static TimeSpan CalculateBackoff(byte attempt, byte maxAttempts, TimeSpan baseDelay, TimeSpan maxDelay, Jitter jitter, Func<double> nextRandomDouble)
    {
        if (maxAttempts > _CeilingForMaxAttempts)
            throw new ArgumentOutOfRangeException(string.Format("{0} must be less than or equal to {1}", maxAttempts, _CeilingForMaxAttempts));
        if (attempt > maxAttempts) attempt = maxAttempts;

        var delay = baseDelay.Multiply(Math.Pow(2, attempt - 1));
        if (delay > maxDelay || delay < TimeSpan.Zero)
            delay = maxDelay;

        var nextRandom = nextRandomDouble();
        switch (jitter)
        {
            case Jitter.Full:
                delay = delay.Multiply(nextRandom);
                break;
            case Jitter.Equal:
                delay = delay.Multiply(0.5 + nextRandom * 0.5);
                break;
        }

        return delay;
    }
}
