namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;


/// <summary>
/// Represents a default policy configuration for a circuit breaker.
/// </summary>
public interface IDefaultCircuitBreakerPolicyConfig
{
    /// <summary>
    /// The retry interval.
    /// </summary>
    TimeSpan RetryInterval { get; }

    /// <summary>
    /// The max amount of failures before a trip.
    /// </summary>
    int FailuresAllowedBeforeTrip { get; }
}
