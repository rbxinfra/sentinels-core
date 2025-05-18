namespace Roblox.Sentinels;

/// <summary>
/// Represents a sentinel.
/// </summary>
public interface ISentinel
{
    /// <summary>
    /// Is the sentinel healthy?
    /// </summary>
    bool IsHealthy { get; }
}
