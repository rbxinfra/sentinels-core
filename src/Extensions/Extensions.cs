namespace Roblox.Sentinels;

using System;

internal static class Extensions
{
    internal static void CheckAndDispose(this IDisposable disposable) => disposable?.Dispose();
}
