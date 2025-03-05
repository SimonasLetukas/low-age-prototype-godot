using Priority_Queue;

namespace DijkstraMap
{
    public class PointId : StablePriorityQueueNode, IEquatable<PointId>, IComparable<PointId>
    {
        public int Value { get; }

        public PointId(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PointId)obj);
        }

        public bool Equals(PointId other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public int CompareTo(PointId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return Value.CompareTo(other.Value);
        }
        
        public static implicit operator PointId(int pointId) => new PointId(pointId);
    }
}