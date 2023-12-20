using System;

public interface INodeFromBlueprint<in TBlueprint>
{
    Guid Id { get; set; }
    void SetBlueprint(TBlueprint blueprint);
}