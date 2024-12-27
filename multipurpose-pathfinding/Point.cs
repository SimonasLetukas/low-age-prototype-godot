using System;
using low_age_prototype_common;

namespace multipurpose_pathfinding
{
    public class Point : IEquatable<Point>
    {
        public int Id { get; set; }
        public Vector2<int> Position { get; set; }
        public int HighGroundAscensionLevel { get; set; }
        public bool IsHighGround { get; set; }
        private bool _isImpassable;

        public bool IsImpassable
        {
            get
            {
                if (_isImpassable)
                    return true;
                var weight = Configuration.TerrainWeights[CalculatedTerrainIndex];
                return weight.Equals(float.PositiveInfinity);
            }
            set => _isImpassable = value;
        }

        public int OriginalTerrainIndex { get; set; }

        public int CalculatedTerrainIndex => _isImpassable
            ? Configuration.ImpassableIndex
            : IsHighGround
                ? Configuration.HighGroundIndex
                : OriginalTerrainIndex;

        internal Configuration Configuration { private get; set; }

        public bool Equals(Point other)
        {
            return Id == other.Id
                   && Position.Equals(other.Position)
                   && IsHighGround == other.IsHighGround
                   && HighGroundAscensionLevel == other.HighGroundAscensionLevel
                   && OriginalTerrainIndex == other.OriginalTerrainIndex
                   && IsImpassable == other.IsImpassable;
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ IsHighGround.GetHashCode();
                hashCode = (hashCode * 397) ^ HighGroundAscensionLevel;
                hashCode = (hashCode * 397) ^ OriginalTerrainIndex;
                hashCode = (hashCode * 397) ^ IsImpassable.GetHashCode();
                return hashCode;
            }
        }
    }
}