using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors;

public class ActorNode : Node2D, INodeFromBlueprint<Actor>
{
    public Guid Id { get; } = Guid.NewGuid();
    public Vector2 ActorPosition { get; set; }
    public List<StatNode> CurrentStats { get; protected set; }
    public ActorRotation ActorRotation { get; protected set; }
    
    private Actor Blueprint { get; set; }
    
    public virtual void SetBlueprint(Actor blueprint)
    {
        Blueprint = blueprint;
        ActorRotation = ActorRotation.BottomRight;
        CurrentStats = blueprint.Statistics.Select(stat => StatNode.InstantiateAsChild(stat, this)).ToList();
    }
}
