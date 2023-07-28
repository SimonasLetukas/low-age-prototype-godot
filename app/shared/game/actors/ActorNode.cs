using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Shared;

public abstract class ActorNode : Node2D
{
    public abstract Guid Id { get; protected set; }
    public abstract Actor Blueprint { get; protected set; }
    public abstract List<StatNode> Stats { get; protected set; }

    protected readonly PackedScene StatNodeScene = GD.Load<PackedScene>(StatNode.ScenePath);

    public virtual void SetBlueprint(Actor blueprint)
    {
        Blueprint = blueprint;
        Stats = Blueprint.Statistics.Select(InstantiateStatNode).ToList();
    }

    protected virtual StatNode InstantiateStatNode(Stat blueprint)
    {
        var stat = (StatNode) StatNodeScene.Instance();
        AddChild(stat);
        stat.SetBlueprint(blueprint);
        return stat;
    }
}
