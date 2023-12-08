using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors;

public abstract class ActorNode<TBlueprint> : Node2D, INodeFromBlueprint<TBlueprint> where TBlueprint : Actor
{
    public Guid Id { get; } = Guid.NewGuid();
    public List<StatNode> CurrentStats { get; protected set; }
    public ActorRotation ActorRotation { get; protected set; }
    
    public abstract TBlueprint Blueprint { get; protected set; }
    
    public virtual void SetBlueprint(TBlueprint blueprint)
    {
        Blueprint = blueprint;
        ActorRotation = ActorRotation.BottomRight;
        CurrentStats = Blueprint.Statistics.Select(stat => StatNode.InstantiateAsChild(stat, this)).ToList();
    }
}
