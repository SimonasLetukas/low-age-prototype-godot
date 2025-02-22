namespace DijkstraMap
{
    /// <summary>
    /// Weight of a connection between two points.
    /// </summary>
    public readonly struct Weight : IEquatable<Weight>
    {
        public static Weight Infinite = new Weight(float.PositiveInfinity);
        public static Weight One = new Weight(1);

        public float Value { get; }

        public Weight(float value)
        {
            Value = value;
        }

        public float Halved => Value * 0.5f;

        public bool Equals(Weight other) => float.Equals(Value, other.Value) 
                                            || Math.Abs(Value - other.Value) < Constants.FloatComparisonEpsilon;

        public override bool Equals(object obj) => obj is Weight other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();
        
        public static implicit operator Weight(float weight) => new Weight(weight);
    }
}