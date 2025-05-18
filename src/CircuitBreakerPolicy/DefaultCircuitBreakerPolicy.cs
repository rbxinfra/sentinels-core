namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;
using System.Threading;

/// <inheritdoc cref="CircuitBreakerPolicyBase{TExecutionContext}"/>
public class DefaultCircuitBreakerPolicy<TExecutionContext> : CircuitBreakerPolicyBase<TExecutionContext>
{
    private readonly CircuitBreaker _CircuitBreaker;
    private int _ShouldRetrySignal;
    private DateTime _NextRetry = DateTime.MinValue;
    private int _ConsecutiveErrorsCount;

    /// <summary>
    /// The circuit breaker policy config.
    /// </summary>
    protected readonly IDefaultCircuitBreakerPolicyConfig Config;

    /// <summary>
    /// Constructs a new instance of <see cref="DefaultCircuitBreakerPolicy{TExecutionContext}"/>.
    /// </summary>
    /// <param name="circuitBreakerIdentifier">An identifier for this policy</param>
    /// <param name="circuitBreakerPolicyConfig">The configuration for this policy</param>
    /// <param name="tripReasonAuthority">The trip reason authority for this policy</param>
    /// <exception cref="ArgumentException">The parameter cannot be null or empty</exception>
    /// <exception cref="ArgumentNullException">The parameter cannot be null</exception>
    public DefaultCircuitBreakerPolicy(string circuitBreakerIdentifier, IDefaultCircuitBreakerPolicyConfig circuitBreakerPolicyConfig, ITripReasonAuthority<TExecutionContext> tripReasonAuthority)
        : base(tripReasonAuthority)
    {
        if (string.IsNullOrWhiteSpace(circuitBreakerIdentifier)) throw new ArgumentException("Has to be a non-empty string.", nameof(circuitBreakerIdentifier));

        Config = circuitBreakerPolicyConfig ?? throw new ArgumentNullException(nameof(circuitBreakerPolicyConfig));
        if (Config.FailuresAllowedBeforeTrip < 0) throw new ArgumentException("FailuresAllowedBeforeTrip cannot be negative.", nameof(circuitBreakerPolicyConfig));

        _CircuitBreaker = new CircuitBreaker(circuitBreakerIdentifier);
    }

    /// <inheritdoc cref="CircuitBreakerPolicyBase{TExecutionContext}.IsCircuitBreakerOpened(TExecutionContext, out CircuitBreakerException)"/>
    protected override bool IsCircuitBreakerOpened(TExecutionContext executionContext, out CircuitBreakerException exception)
    {
        exception = null;

        if (!_CircuitBreaker.IsTripped) return false;
        if (_NextRetry <= DateTime.UtcNow && Interlocked.CompareExchange(ref _ShouldRetrySignal, 1, 0) == 0) return false;

        exception = new CircuitBreakerException(_CircuitBreaker);
        return true;
    }

    /// <inheritdoc cref="CircuitBreakerPolicyBase{TExecutionContext}.OnSuccessfulRequest(TExecutionContext)"/>
    protected override void OnSuccessfulRequest(TExecutionContext executionContext)
    {
        Interlocked.Exchange(ref _ConsecutiveErrorsCount, 0);
        _CircuitBreaker.Reset();
    }

    /// <inheritdoc cref="CircuitBreakerPolicyBase{TExecutionContext}.OnNotified(TExecutionContext)"/>
    protected override void OnNotified(TExecutionContext executionContext) => Interlocked.Exchange(ref _ShouldRetrySignal, 0);

    /// <inheritdoc cref="CircuitBreakerPolicyBase{TExecutionContext}.TryToTripCircuitBreaker(TExecutionContext)"/>
    protected override bool TryToTripCircuitBreaker(TExecutionContext executionContext)
    {
        Interlocked.Increment(ref _ConsecutiveErrorsCount);
        if (_ConsecutiveErrorsCount <= Config.FailuresAllowedBeforeTrip) return false;

        _NextRetry = DateTime.UtcNow.Add(Config.RetryInterval);
        _CircuitBreaker.Trip();
        return true;
    }
}
