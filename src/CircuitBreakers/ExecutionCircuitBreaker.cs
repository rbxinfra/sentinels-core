namespace Roblox.Sentinels;

using System;

/// <inheritdoc cref="ExecutionCircuitBreakerBase"/>
public class ExecutionCircuitBreaker : ExecutionCircuitBreakerBase
{
    private readonly Func<Exception, bool> _FailureDetector;
    private readonly Func<TimeSpan> _RetryIntervalCalculator;

    /// <inheritdoc cref="CircuitBreakerBase.Name"/>
    protected internal override string Name { get; }

    /// <inheritdoc cref="ExecutionCircuitBreakerBase.RetryInterval"/>
    protected override TimeSpan RetryInterval => _RetryIntervalCalculator();

    /// <summary>
    /// Create a new instance of <see cref="ExecutionCircuitBreaker"/>.
    /// </summary>
    /// <param name="name">The name of this circuit breaker.</param>
    /// <param name="failureDetector">The method to determine if a execption is a failure</param>
    /// <param name="retryIntervalCalculator">The retry interval calculator</param>
    /// <exception cref="ArgumentException">Argument cannot be null, empty or whitespace</exception>
    /// <exception cref="ArgumentNullException">Argument cannot be null</exception>
    public ExecutionCircuitBreaker(string name, Func<Exception, bool> failureDetector, Func<TimeSpan> retryIntervalCalculator)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Cannot be null, empty or whitespace", nameof(name));
        _FailureDetector = failureDetector ?? throw new ArgumentNullException(nameof(failureDetector));
        _RetryIntervalCalculator = retryIntervalCalculator ?? throw new ArgumentNullException(nameof(retryIntervalCalculator));

        Name = name;
    }

    /// <inheritdoc cref="ExecutionCircuitBreakerBase.ShouldTrip(Exception)"/>
    protected override bool ShouldTrip(Exception ex) => _FailureDetector(ex);
}
