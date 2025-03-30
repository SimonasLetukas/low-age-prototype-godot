using System;

/// <summary>
/// <see cref="Random"/> with synchronized seed between server and all clients.
/// </summary>
public static class SharedRandom
{
    public static int Seed { get; private set; } = Guid.NewGuid().GetHashCode();
    public static Random Instance { get; private set; } = new(Seed);

    public static void Set(int seed)
    {
        Seed = seed;
        Instance = new Random(seed);
    }
}