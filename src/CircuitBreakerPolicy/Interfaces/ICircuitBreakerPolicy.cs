namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;

/// <summary>
/// Represents the policy to implement for a circuit breaker.
/// </summary>
/// <typeparam name="TExecutionContext">The execution context</typeparam>
public interface ICircuitBreakerPolicy<in TExecutionContext> : IDisposable
{
    /// <summary>
    /// Invoked when the circuit breaker terminates the request.
    /// </summary>
    event Action CircuitBreakerTerminatingRequest;

    /// <summary>
    /// Invoked when the request wants to open the circuit breaker.
    /// </summary>
    event Action RequestIntendingToOpenCircuitBreaker;


    /// <summary>
    /// Notify the circuit breaker that the request has ended.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    /// <param name="exception">The potential exception brought forward.</param>
    void NotifyRequestFinished(TExecutionContext executionContext, Exception exception = null);

    /// <summary>
    /// Throws an exception if the policy allows for it.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    void ThrowIfTripped(TExecutionContext executionContext);
}
