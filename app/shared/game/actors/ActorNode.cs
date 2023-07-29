using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors;

public abstract class ActorNode : Node2D, INodeFromBlueprint<Actor>
{
    public Guid Id { get; } = Guid.NewGuid();
    public abstract Actor Blueprint { get; protected set; }
    public abstract List<StatNode> Stats { get; protected set; }
    
    public virtual void SetBlueprint(Actor blueprint)
    {
        Blueprint = blueprint;
        Stats = Blueprint.Statistics.Select(stat => StatNode.InstantiateAsChild(stat, this)).ToList();
    }
}
