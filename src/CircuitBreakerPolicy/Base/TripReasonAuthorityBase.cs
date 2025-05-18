namespace Roblox.Sentinels.CircuitBreakerPolicy;

using System;

/// <inheritdoc cref="ITripReasonAuthority{TExecutionContext}"/>
public abstract class TripReasonAuthorityBase<TExecutionContext> : ITripReasonAuthority<TExecutionContext>
{
    /// <inheritdoc cref="ITripReasonAuthority{TExecutionContext}.IsReasonForTrip(TExecutionContext, Exception)"/>
    public abstract bool IsReasonForTrip(TExecutionContext executionContext, Exception exception);

    /// <summary>
    /// Try get an inner exception of type.
    /// </summary>
    /// <typeparam name="T">The typeof the exception</typeparam>
    /// <param name="exception">The base exception</param>
    /// <param name="inner">The out inner exception</param>
    /// <returns>If there is an exception of this type</returns>
    protected static bool TryGetInnerExceptionOfType<T>(Exception exception, out T inner)
        where T : Exception
    {
        inner = default;

        while (exception.InnerException != null)
        {
            inner = exception.InnerException as T;
            if (inner != null)
                return true;
            exception = exception.InnerException;
        }

        return false;
    }
}
