using Godot;
using System;
using low_age_data.Domain.Effects;

public class EffectNode : Node2D, INodeFromBlueprint<Effect>
{
    public Guid Id { get; } = Guid.NewGuid();
    
    private Effect Blueprint { get; set; }

    public void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
    }
}
