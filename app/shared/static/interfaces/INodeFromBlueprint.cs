using System;

public interface INodeFromBlueprint<in TBlueprint>
{
    Guid InstanceId { get; set; }
    void SetBlueprint(TBlueprint blueprint);
}