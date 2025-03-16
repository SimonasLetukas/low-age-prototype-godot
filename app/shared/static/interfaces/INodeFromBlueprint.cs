using System;

public interface INodeFromBlueprint<TBlueprint> : IEquatable<INodeFromBlueprint<TBlueprint>>
{
    Guid InstanceId { get; set; }
    void SetBlueprint(TBlueprint blueprint);
    
    bool IEquatable<INodeFromBlueprint<TBlueprint>>.Equals(INodeFromBlueprint<TBlueprint>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return InstanceId.Equals(other.InstanceId);
    }
}

public static class NodeFromBlueprint
{
    public static bool Equals<TBlueprint>(INodeFromBlueprint<TBlueprint> current, object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(current, obj)) return true;
        if (obj is not INodeFromBlueprint<TBlueprint> other) return false;
        return current.InstanceId.Equals(other.InstanceId);
    }

    public static int GetHashCode<TBlueprint>(INodeFromBlueprint<TBlueprint> instance)
    {
        return instance.InstanceId.GetHashCode();
    }
}