namespace Roblox.Sentinels;

using System;
using System.Threading;

/// <inheritdoc cref="ISentinel"/>
public class ServiceSentinel : ISentinel, IDisposable
{

    private readonly Func<bool> _HealthChecker;
    private bool _IsDisposed;

    /// <summary>
    /// The monitor interval getter.
    /// </summary>
    protected readonly Func<TimeSpan> _MonitorIntervalGetter;

    /// <summary>
    /// The monitor timer.
    /// </summary>
    protected Timer _MonitorTimer;

    /// <inheritdoc cref="ISentinel.IsHealthy"/>
    public bool IsHealthy { get; private set; }

    /// <summary>
    /// Constructs a new instance of <see cref="ServiceSentinel"/>
    /// </summary>
    /// <param name="healthChecker">A function to determine if this sentinel is healthy</param>
    /// <param name="monitorIntervalGetter">The monitor interval getter</param>
    /// <param name="isHealthy">The initial sentinel state</param>
    public ServiceSentinel(Func<bool> healthChecker, Func<TimeSpan> monitorIntervalGetter, bool isHealthy = true)
    {
        _HealthChecker = healthChecker;
        _MonitorIntervalGetter = monitorIntervalGetter;
        _MonitorTimer = new(OnTimerCallback);
        IsHealthy = isHealthy;

        var monitorInterval = monitorIntervalGetter();
        _MonitorTimer.Change(monitorInterval, monitorInterval);
    }

    private void OnTimerCallback(object state)
    {
        if (_IsDisposed) return;

        var timer = (Timer)state;
        try
        {
            timer.Change(-1, -1);
            IsHealthy = _HealthChecker();
        }
        catch (Exception) { IsHealthy = false; }
        finally
        {
            try
            {
                var monitorInterval = _MonitorIntervalGetter();
                timer.Change(monitorInterval, monitorInterval);
            }
            catch { }
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    protected virtual void Dispose(bool disposing)
    {
        if (_IsDisposed) return;

        if (disposing)
        {
            _MonitorTimer.CheckAndDispose();
            _MonitorTimer = null;
        }

        _IsDisposed = true;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
