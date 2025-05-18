namespace Roblox.Sentinels;

/// <inheritdoc cref="CircuitBreakerBase"/>
public class CircuitBreaker : CircuitBreakerBase
{
    /// <inheritdoc cref="CircuitBreakerBase.Name"/>
    protected internal override string Name { get; }

    /// <summary>
    /// Creates a new instance of <see cref="CircuitBreaker"/>
    /// </summary>
    /// <param name="name">The name of this circuit breaker</param>
    public CircuitBreaker(string name) => Name = name;
}
