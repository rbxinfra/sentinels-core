namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;


/// <summary>
/// An authority to determine if a trip should be enabled.
/// </summary>
/// <typeparam name="TExecutionContext">The execution context</typeparam>
public interface ITripReasonAuthority<in TExecutionContext>
{
    /// <summary>
    /// Is this a reason for a trip?
    /// </summary>
    /// <param name="executionContext">The execution context</param>
    /// <param name="exception">The exception</param>
    /// <returns>Is this a reason for a trip?</returns>
    bool IsReasonForTrip(TExecutionContext executionContext, Exception exception);
}
