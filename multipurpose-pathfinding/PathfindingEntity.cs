using System;
using low_age_prototype_common;
using NetTopologySuite.Geometries;

namespace multipurpose_pathfinding
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
        internal bool HasOccupation => Size != Vector2Int.Zero;

        public PathfindingEntity(Guid id, Vector2<int> position, Vector2<int> size, Team team, bool isOnHighGround,
            Func<Point, Team, bool> canBeMovedThroughAt)
        {
            Id = id;
            Position = position;
            Size = size;
            Team = team;
            Bounds = new Envelope(position.X, position.X + size.X, position.Y, position.Y + size.Y);
            IsOnHighGround = isOnHighGround;
            CanBeMovedThroughAt = canBeMovedThroughAt;
        }

        internal static PathfindingEntity WithoutOccupation(PathfindingEntity from) => new PathfindingEntity(from.Id,
            from.Position, Vector2Int.Zero, from.Team, from.IsOnHighGround, from.CanBeMovedThroughAt);
    }
}