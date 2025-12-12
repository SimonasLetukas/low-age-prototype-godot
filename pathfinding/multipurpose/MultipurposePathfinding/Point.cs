using LowAgeCommon;
using Newtonsoft.Json;

namespace MultipurposePathfinding
{
    public class Point : IEquatable<Point>
    {
        public required int Id { get; init; }
        public required Vector2Int Position { get; init; }
        public required int HighGroundAscensionLevel { get; init; }
        public required bool IsHighGround { get; init; }
        public bool IsLowGround => IsHighGround is false;
        public bool IsAscendable => HighGroundAscensionLevel != 100;
        public float Weight => Configuration.TerrainWeights[CalculatedTerrainIndex];
        
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

        public int OriginalTerrainIndex { get; internal set; }
        
        public int CalculatedTerrainIndex => _isImpassable
            ? Configuration.ImpassableIndex
            : IsHighGround
                ? Configuration.HighGroundIndex
                : OriginalTerrainIndex;

        [JsonProperty] // TODO not serializing this would reduce the amount of text logged by ~50%, perhaps static class could be used instead?
        internal Configuration Configuration { get; init; } = null!;
        
        public bool Equals(Point? other)
        {
            return other is not null 
                   && Id == other.Id
                   && Position.Equals(other.Position)
                   && IsHighGround == other.IsHighGround
                   && HighGroundAscensionLevel == other.HighGroundAscensionLevel
                   && OriginalTerrainIndex == other.OriginalTerrainIndex
                   && IsImpassable == other.IsImpassable;
        }

        public override bool Equals(object? obj)
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
                hashCode = (hashCode * 397) ^ IsImpassable.GetHashCode();
                return hashCode;
            }
        }
    }
}