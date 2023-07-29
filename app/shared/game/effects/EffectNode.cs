using Godot;
using System;
using low_age_data.Domain.Effects;

public abstract class EffectNode : Node2D, INodeFromBlueprint<Effect>
{
    public Guid Id { get; } = Guid.NewGuid();
    public abstract Effect Blueprint { get; protected set; }

    public virtual void SetBlueprint(Effect blueprint)
    {
        Blueprint = blueprint;
    }
}
