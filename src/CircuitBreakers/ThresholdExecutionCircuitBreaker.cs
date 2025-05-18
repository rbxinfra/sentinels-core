namespace Roblox.Sentinels;

using System;
using System.Threading;

/// <inheritdoc cref="ExecutionCircuitBreakerBase"/>
public class ThresholdExecutionCircuitBreaker : ExecutionCircuitBreakerBase
{
    private readonly Func<Exception, bool> _FailureDetector;
    private readonly Func<TimeSpan> _RetryIntervalCalculator;
    private readonly Func<int> _ExceptionCountForTripping;
    private readonly Func<TimeSpan> _ExceptionIntervalForTripping;
    private readonly Func<DateTime> _UtcNowGetter;
    private DateTime _ExceptionCountIntervalEnd = DateTime.MinValue;
    private int _ExceptionCount;

    /// <inheritdoc cref="CircuitBreakerBase.Name"/>
    protected internal override string Name { get; }

    /// <inheritdoc cref="ExecutionCircuitBreakerBase.RetryInterval"/>
    protected override TimeSpan RetryInterval => _RetryIntervalCalculator();

    /// <inheritdoc cref="CircuitBreakerBase.Now"/>
    protected override DateTime Now => _UtcNowGetter();

    /// <summary>
    /// Creates a new instance of <see cref="ThresholdExecutionCircuitBreaker"/>
    /// </summary>
    /// <param name="name">The name of this circuit breaker</param>
    /// <param name="failureDetector">The function to determine a failure</param>
    /// <param name="retryIntervalCalculator">The function to calculate a retry</param>
    /// <param name="exceptionCountForTripping">The function to get the max amount of exceptions before a trip</param>
    /// <param name="exceptionIntervalForTripping">The interval getter for a trip</param>
    public ThresholdExecutionCircuitBreaker(string name, Func<Exception, bool> failureDetector, Func<TimeSpan> retryIntervalCalculator, Func<int> exceptionCountForTripping, Func<TimeSpan> exceptionIntervalForTripping)
            : this(name, failureDetector, retryIntervalCalculator, exceptionCountForTripping, exceptionIntervalForTripping, () => DateTime.UtcNow)
    {
    }
    internal ThresholdExecutionCircuitBreaker(string name, Func<Exception, bool> failureDetector, Func<TimeSpan> retryIntervalCalculator, Func<int> exceptionCountForTripping, Func<TimeSpan> exceptionIntervalForTripping, Func<DateTime> utcNowGetter)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Cannot be null, empty or whitespace", nameof(name));
        _FailureDetector = failureDetector ?? throw new ArgumentNullException(nameof(failureDetector));
        _RetryIntervalCalculator = retryIntervalCalculator ?? throw new ArgumentNullException(nameof(retryIntervalCalculator));
        _ExceptionCountForTripping = exceptionCountForTripping ?? throw new ArgumentNullException(nameof(exceptionCountForTripping));
        _ExceptionIntervalForTripping = exceptionIntervalForTripping ?? throw new ArgumentNullException(nameof(exceptionIntervalForTripping));
        _UtcNowGetter = utcNowGetter ?? throw new ArgumentNullException(nameof(utcNowGetter));

        Name = name;
    }

    private void ResetExceptionCount()
    {
        Interlocked.Exchange(ref _ExceptionCount, 0);
        _ExceptionCountIntervalEnd = Now.Add(_ExceptionIntervalForTripping());
    }

    /// <inheritdoc cref="ExecutionCircuitBreakerBase.ShouldTrip(Exception)"/>
    protected override bool ShouldTrip(Exception ex)
    {
        if (ex == null) throw new ArgumentNullException(nameof(ex));

        if (_FailureDetector(ex))
        {
            if (_ExceptionCountIntervalEnd < Now) ResetExceptionCount();

            Interlocked.Increment(ref _ExceptionCount);
            if (_ExceptionCount > _ExceptionCountForTripping()) return true;
        }
        return false;
    }
}
