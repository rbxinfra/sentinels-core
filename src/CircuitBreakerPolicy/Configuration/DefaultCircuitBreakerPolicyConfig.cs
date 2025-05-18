namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;

/// <inheritdoc cref="IDefaultCircuitBreakerPolicyConfig"/>
public class DefaultCircuitBreakerPolicyConfig : IDefaultCircuitBreakerPolicyConfig
{
    /// <inheritdoc cref="IDefaultCircuitBreakerPolicyConfig.RetryInterval"/>
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromMilliseconds(250);

    /// <inheritdoc cref="IDefaultCircuitBreakerPolicyConfig.FailuresAllowedBeforeTrip"/>
    public int FailuresAllowedBeforeTrip
    {
        get => _FailuresAllowedBeforeTrip;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(FailuresAllowedBeforeTrip), "Has to be bigger than or equal to zero.");
            _FailuresAllowedBeforeTrip = value;
        }
    }

    private int _FailuresAllowedBeforeTrip;
}
