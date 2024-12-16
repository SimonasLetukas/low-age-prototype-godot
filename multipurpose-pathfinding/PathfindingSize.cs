using System;

namespace multipurpose_pathfinding
{
    /// <summary>
    /// Value used to keep track of the pathfinding size, to enable tracking pathfinding of MxM sized entities.
    /// </summary>
    public readonly struct PathfindingSize : IEquatable<PathfindingSize>
    {
        public static PathfindingSize Default => new PathfindingSize(1);

        public int Value { get; }

        public PathfindingSize(int value)
        {
            Value = value;
        }

        public bool Equals(PathfindingSize other) => Value == other.Value;

        public override bool Equals(object obj) => obj is PathfindingSize other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator PathfindingSize(int pathfindingSize) => new PathfindingSize(pathfindingSize);
    }
}