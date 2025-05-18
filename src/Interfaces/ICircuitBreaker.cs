namespace Roblox.Sentinels;

/// <summary>
/// Represents a circuit breaker.
/// </summary>
public interface ICircuitBreaker
{
    /// <summary>
    /// Is this circuit breaker tripped?
    /// </summary>
    bool IsTripped { get; }

    /// <summary>
    /// Reset the circuit breaker.
    /// </summary>
    /// <returns>If the reset was successful</returns>
    bool Reset();

    /// <summary>
    /// Test if the circuit breaker is tripped.
    /// </summary>
    void Test();

    /// <summary>
    /// Trip the circuit breaker.
    /// </summary>
    /// <returns>If the trip was successful</returns>
    bool Trip();
}
