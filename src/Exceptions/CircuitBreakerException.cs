namespace Roblox.Sentinels;

using System;

/// <summary>
/// Represents a circuit breaker exception.
/// </summary>
public class CircuitBreakerException : Exception
{
    private readonly string _CircuitBreakerName;
    private readonly DateTime? _CircuitBreakerTripped;

    /// <inheritdoc cref="Exception"/>
    public override string Message
    {
        get
        {
            var now = DateTime.UtcNow;
            var trippedTime = now.Subtract(_CircuitBreakerTripped ?? now);

            return $"CircuitBreaker Error: {_CircuitBreakerName} has been tripped for {trippedTime.TotalSeconds} seconds.";
        }
    }

    /// <summary>
    /// Constructs a new instance of <see cref="CircuitBreakerException"/>
    /// </summary>
    /// <param name="circuitBreaker">The circuit breaker</param>
    public CircuitBreakerException(CircuitBreakerBase circuitBreaker)
        : this(circuitBreaker.Name, circuitBreaker.Tripped)
    {
    }

    /// <summary>
    /// Constructs a new instance of <see cref="CircuitBreakerException"/>
    /// </summary>
    /// <param name="circuitBreakerName">The name of the circuit breaker.</param>
    /// <param name="circuitBreakerTripped">The time that the circuit breaker was tripped</param>	
    public CircuitBreakerException(string circuitBreakerName, DateTime? circuitBreakerTripped)
    {
        _CircuitBreakerName = circuitBreakerName;
        _CircuitBreakerTripped = circuitBreakerTripped;
    }
}
