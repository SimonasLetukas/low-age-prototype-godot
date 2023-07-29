using System;

public interface INodeFromBlueprint<in TBlueprint>
{
    Guid Id { get; }
    void SetBlueprint(TBlueprint blueprint);
}