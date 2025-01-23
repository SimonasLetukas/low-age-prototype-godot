using System;

namespace low_age_dijkstra
{
    /// <summary>
    /// Cost of a path.
    /// </summary>
    public readonly struct Cost : IEquatable<Cost>, IComparable<Cost>
    {
        public static Cost Infinity = new Cost(float.PositiveInfinity);

        public float Value { get; }

        public Cost(float value)
        {
            Value = value;
        }

        public int CompareTo(Cost other) => Value.CompareTo(other.Value);

        public bool Equals(Cost other) => float.Equals(Value, other.Value) 
                                          || Math.Abs(Value - other.Value) < Constants.FloatComparisonEpsilon;

        public override bool Equals(object obj) => obj is Cost other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
        
        public static implicit operator Cost(float cost) => new Cost(cost);
    }
}