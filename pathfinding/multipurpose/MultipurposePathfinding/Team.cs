namespace MultipurposePathfinding
{
    /// <summary>
    /// Value used to keep track of the pathfinding team, to enable tracking differences between pathfinding of
    /// different teams.
    /// </summary>
    public readonly struct Team(int value) : IEquatable<Team>, IComparer<Team>
    {
        public static Team Default => new(1);

        public int Value { get; } = value;

        public bool IsEnemyTo(Team team) => IsAllyTo(team) is false;

        public bool IsAllyTo(Team team) => team == this;
        
        public override string ToString() => Value.ToString();

        public bool Equals(Team other) => Value == other.Value;

        public override bool Equals(object? obj) => obj is Team other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator Team(int team) => new(team);
        
        public static bool operator ==(Team left, Team right) => left.Equals(right);

        public static bool operator !=(Team left, Team right) => !left.Equals(right);
        
        public static bool operator <(Team left, Team right) => left.Value < right.Value;

        public static bool operator >(Team left, Team right) => left.Value > right.Value;

        public int Compare(Team x, Team y) => x.Value.CompareTo(y.Value);
    }
}