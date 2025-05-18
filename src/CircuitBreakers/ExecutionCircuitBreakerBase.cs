namespace Roblox.Sentinels;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <inheritdoc cref="CircuitBreakerBase"/>
public abstract class ExecutionCircuitBreakerBase : CircuitBreakerBase
{
    private DateTime _NextRetry = DateTime.MinValue;
    private int _ShouldRetrySignal;

    private bool IsTimeForRetry => Now >= _NextRetry;

    /// <summary>
    /// The retry interval for this circuit breaker.
    /// </summary>
    protected abstract TimeSpan RetryInterval { get; }

    private bool ShouldRetry() => Interlocked.CompareExchange(ref _ShouldRetrySignal, 1, 0) == 0;
    private void AttemptToProceed()
    {
        try { Test(); }
        catch (CircuitBreakerException) { if (!IsTimeForRetry || !ShouldRetry()) throw; }
    }

    /// <summary>
    /// Should this circuit breaker trip?
    /// </summary>
    /// <param name="ex">The exception to check</param>
    /// <returns>Should this circuit breaker trip?</returns> 
    protected abstract bool ShouldTrip(Exception ex);

    /// <summary>
    /// Execute the circuit action.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Execute(Action action)
    {
        AttemptToProceed();

        try { action(); }
        catch (Exception ex)
        {
            if (ShouldTrip(ex))
            {
                _NextRetry = Now.Add(RetryInterval);
                Trip();
            }
            throw;
        }
        finally { Interlocked.Exchange(ref _ShouldRetrySignal, 0); }

        Reset();
    }

    /// <summary>
    /// Execute the circuit breaker but async.
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task to be awaited</returns>
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default(CancellationToken))
    {
        AttemptToProceed();

        try { await action(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            if (ShouldTrip(ex))
            {
                _NextRetry = Now.Add(RetryInterval);
                Trip();
            }
            throw;
        }
        finally { Interlocked.Exchange(ref _ShouldRetrySignal, 0); }

        Reset();
    }

    /// <inheritdoc cref="CircuitBreakerBase.Reset"/>
    public override bool Reset()
    {
        _NextRetry = DateTime.MinValue;
        return base.Reset();
    }

}
