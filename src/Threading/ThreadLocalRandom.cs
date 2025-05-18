namespace Roblox.Sentinels;

using System;
using System.Threading;

internal class ThreadLocalRandom
{
    private readonly ThreadLocal<Random> _Random;
    private int _Seed;

    public ThreadLocalRandom(int initialSeed)
    {
        _Seed = initialSeed;
        _Random = new(() => new(Interlocked.Increment(ref _Seed)));
    }
    public ThreadLocalRandom()
        : this(Environment.TickCount)
    { }

    public int Next() => _Random.Value.Next();
    public int Next(int maxValue) => _Random.Value.Next(maxValue);
    public int Next(int minValue, int maxValue) => _Random.Value.Next(minValue, maxValue);
    public double NextDouble() => _Random.Value.NextDouble();
}
