using System;

namespace multipurpose_pathfinding
{
    /// <summary>
    /// Value used to keep track of the pathfinding team, to enable tracking differences between pathfinding of
    /// different teams.
    /// </summary>
    public readonly struct Team : IEquatable<Team>
    {
        public static Team Default => new Team(1);

        public int Value { get; }

        public Team(int value)
        {
            Value = value;
        }

        public bool Equals(Team other) => Value == other.Value;

        public override bool Equals(object obj) => obj is Team other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator Team(int team) => new Team(team);
    }
}