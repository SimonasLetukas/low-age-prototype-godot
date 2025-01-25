using System;
using LowAgeCommon;
using NetTopologySuite.Geometries;

namespace MultipurposePathfinding
{
    public class PathfindingEntity
    {
        public Guid Id { get; }
        public Vector2<int> Position { get; private set; }
        public Vector2<int> Size { get; private set; }
        public Team Team { get; private set; }
        internal Envelope Bounds { get; private set; }
        internal bool IsOnHighGround { get; private set; }
        internal Func<Point, Team, bool> CanBeMovedThroughAt { get; }
        internal Func<Point, Point, Team, bool> AllowsConnectionBetweenPoints { get; }
        internal bool HasOccupation => Size != Vector2Int.Zero;
        internal Vector2<int> UpperBounds => Position + Size;

        public PathfindingEntity(Guid id, Vector2<int> position, Vector2<int> size, Team team, bool isOnHighGround,
            Func<Point, Team, bool> canBeMovedThroughAt, Func<Point, Point, Team, bool> allowsConnectionBetweenPoints)
        {
            Id = id;
            Position = position;
            Size = size;
            Team = team;
            Bounds = new Envelope(position.X, position.X + size.X, position.Y, position.Y + size.Y);
            IsOnHighGround = isOnHighGround;
            CanBeMovedThroughAt = canBeMovedThroughAt;
            AllowsConnectionBetweenPoints = allowsConnectionBetweenPoints;
        }
    }
}