using System;

namespace low_age_dijkstra
{
    /// <summary>
    /// Index of what each point is made of.
    /// </summary>
    public readonly struct Terrain : IEquatable<Terrain>
    {
        public static Terrain Default => new Terrain(-1);

        public int Value { get; }

        public Terrain(int value)
        {
            Value = value;
        }

        public bool Equals(Terrain other) => Value == other.Value;

        public override bool Equals(object obj) => obj is Terrain other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
        
        public static implicit operator int(Terrain terrain) => terrain.Value;
    }
}