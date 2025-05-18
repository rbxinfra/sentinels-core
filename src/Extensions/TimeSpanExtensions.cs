namespace Roblox.Sentinels;

using System;

internal static class TimeSpanExtensions
{
    public static TimeSpan Multiply(this TimeSpan multiplicand, double multiplier) => TimeSpan.FromTicks((long)(multiplicand.Ticks * multiplier));
}
