using Godot;
using System;
using LowAgeData.Domain.Effects;

public partial class EffectNode : Node2D, INodeFromBlueprint<Effect>
{
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    
    private Effect Blueprint { get; set; }

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
    }
}
