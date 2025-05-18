namespace Roblox.Sentinels;

using System;

/// <inheritdoc cref="ServiceSentinel"/>
public class TogglableServiceSentinel : ServiceSentinel
{
    /// <summary>
    /// Is the sentinel running?
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Constructs a new instance of <see cref="TogglableServiceSentinel"/>
    /// </summary>
    /// <param name="healthChecker">A function to determine if this sentinel is healthy</param>
    /// <param name="monitorIntervalGetter">The monitor interval getter</param>
    /// <param name="isHealthy">The initial sentinel state</param>
    public TogglableServiceSentinel(Func<bool> healthChecker, Func<TimeSpan> monitorIntervalGetter, bool isHealthy = true)
        : base(healthChecker, monitorIntervalGetter, isHealthy)
        => IsRunning = true;

    /// <summary>
    /// Stop the sentinel from running.
    /// </summary>
    public void StopSentinel()
    {
        if (IsRunning)
        {
            _MonitorTimer.Change(-1, -1);
            IsRunning = false;
        }
    }

    /// <summary>
    /// Start the sentinel.
    /// </summary>
    public void StartSentinel()
    {
        if (!IsRunning)
        {
            var monitorInterval = _MonitorIntervalGetter();
            _MonitorTimer.Change(monitorInterval, monitorInterval);
            IsRunning = true;
        }
    }
}
