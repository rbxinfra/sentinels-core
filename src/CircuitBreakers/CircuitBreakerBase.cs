namespace Roblox.Sentinels;

using System;
using System.Threading;

/// <inheritdoc cref="ICircuitBreaker"/>
public abstract class CircuitBreakerBase : ICircuitBreaker
{
    private int _IsTripped;

    /// <summary>
    /// The name of this circuit breaker.
    /// </summary>
    protected internal abstract string Name { get; }

    /// <summary>
    /// The date when this circuit breaker was tripped.
    /// </summary>
    protected internal DateTime? Tripped { get; private set; }

    /// <summary>
    /// Gets date time now.
    /// </summary>
    protected virtual DateTime Now => DateTime.UtcNow;

    /// <inheritdoc cref="ICircuitBreaker.IsTripped"/>
    public bool IsTripped => _IsTripped == 1;

    /// <inheritdoc cref="ICircuitBreaker.Reset"/>
    public virtual bool Reset()
    {
        var tripped = Interlocked.Exchange(ref _IsTripped, 0) == 1;
        if (tripped) Tripped = null;

        return tripped;
    }

    /// <inheritdoc cref="ICircuitBreaker.Test"/>
    public virtual void Test()
    {
        if (IsTripped) throw new CircuitBreakerException(this);
    }

    /// <inheritdoc cref="ICircuitBreaker.Trip"/>
    public virtual bool Trip()
    {
        var tripped = Interlocked.Exchange(ref _IsTripped, 1) == 1;
        if (!tripped) Tripped = Now;

        return tripped;
    }

}
