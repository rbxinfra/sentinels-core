namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;

/// <inheritdoc cref="ICircuitBreakerPolicy{TExecutionContext}"/>
public abstract class CircuitBreakerPolicyBase<TExecutionContext> : ICircuitBreakerPolicy<TExecutionContext>, IDisposable
{
    private bool _Disposed;
    private readonly ITripReasonAuthority<TExecutionContext> _TripReasonAuthority;

    /// <summary>
    /// Constructs a new instance of <see cref="CircuitBreakerPolicyBase{TExecutionContext}"/>.
    /// </summary>
    /// <param name="tripReasonAuthority">The trip reason authority used to determine if the circuit breaker should be tripped</param>
    /// <exception cref="ArgumentNullException">tripReasonAuthority is null.</exception>
    protected CircuitBreakerPolicyBase(ITripReasonAuthority<TExecutionContext> tripReasonAuthority)
        => _TripReasonAuthority = tripReasonAuthority ?? throw new ArgumentNullException(nameof(tripReasonAuthority));


    /// <inheritdoc cref="ICircuitBreakerPolicy{TExecutionContext}.CircuitBreakerTerminatingRequest"/>
    public event Action CircuitBreakerTerminatingRequest;

    /// <inheritdoc cref="ICircuitBreakerPolicy{TExecutionContext}.RequestIntendingToOpenCircuitBreaker"/>
    public event Action RequestIntendingToOpenCircuitBreaker;

    /// <inheritdoc cref="ICircuitBreakerPolicy{TExecutionContext}.NotifyRequestFinished(TExecutionContext, Exception)"/>
    public void NotifyRequestFinished(TExecutionContext executionContext, Exception exception)
    {
        try
        {
            if (_TripReasonAuthority.IsReasonForTrip(executionContext, exception))
            {
                RequestIntendingToOpenCircuitBreaker?.Invoke();
                TryToTripCircuitBreaker(executionContext);
            }
            else OnSuccessfulRequest(executionContext);
        }
        finally { OnNotified(executionContext); }
    }

    /// <inheritdoc cref="ICircuitBreakerPolicy{TExecutionContext}.ThrowIfTripped(TExecutionContext)"/>
    public void ThrowIfTripped(TExecutionContext executionContext)
    {
        if (!IsCircuitBreakerOpened(executionContext, out CircuitBreakerException ex)) return;

        CircuitBreakerTerminatingRequest?.Invoke();
        throw ex;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Determines if the circuit breaker is opened.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    /// <param name="exception">The potential exception</param>
    /// <returns>Is the circuit breaker opened.</returns>
    protected abstract bool IsCircuitBreakerOpened(TExecutionContext executionContext, out CircuitBreakerException exception);

    /// <summary>
    /// Tries to trip the circuit breaker.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    /// <returns>Did the circuit breaker trip</returns>
    protected abstract bool TryToTripCircuitBreaker(TExecutionContext executionContext);

    /// <summary>
    /// Invoked if the trip reason authority does not have a reason to trip.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    protected abstract void OnSuccessfulRequest(TExecutionContext executionContext);

    /// <summary>
    /// Invoked when the execution ends.
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    protected abstract void OnNotified(TExecutionContext executionContext);


    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed) return;

        if (disposing && _TripReasonAuthority is IDisposable disposableAuthority)
            disposableAuthority.Dispose();

        _Disposed = true;
    }
}
