using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities.Actors;

/// <summary>
/// <see cref="StructureNode"/> or <see cref="UnitNode"/> with abilities and stats.
/// </summary>
public class ActorNode : EntityNode, INodeFromBlueprint<Actor>
{
    public IList<StatNode> CurrentStats { get; protected set; }
    public IList<ActorAttribute> Attributes { get; protected set; }
    public ActorRotation ActorRotation { get; protected set; }
    public Abilities Abilities { get; protected set; }
    
    private Actor Blueprint { get; set; }
    private TextureProgress _health;
    private TextureProgress _shields;
    
    public override void _Ready()
    {
        base._Ready();
        
        Abilities = GetNode<Abilities>(nameof(Abilities));

        _health = GetNode<TextureProgress>("Health");
        _shields = GetNode<TextureProgress>("Shields");
    }
    
    public void SetBlueprint(Actor blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        CurrentStats = blueprint.Statistics.Select(stat => StatNode.InstantiateAsChild(stat, this)).ToList();
        Attributes = blueprint.ActorAttributes;
        ActorRotation = ActorRotation.BottomRight;
        Abilities.PopulateFromBlueprint(Blueprint.Abilities);
        Behaviours.AddOnBuildBehaviours(Abilities.GetPassives());
        
        var spriteSize = Renderer.SpriteSize;
        _health.RectPosition = new Vector2(_health.RectPosition.x, (spriteSize.y * -1) - 2);
        _health.Visible = false;
        _shields.RectPosition = new Vector2(_shields.RectPosition.x, (spriteSize.y * -1) - 3);
        _shields.Visible = false;
    }

    public override void SetOutline(bool to)
    {
        base.SetOutline(to);
        _health.Visible = to;
        _shields.Visible = to && HasShields;
    }

    public virtual void Rotate()
    {
        switch (ActorRotation)
        {
            case ActorRotation.BottomRight:
                ActorRotation = ActorRotation.BottomLeft;
                break;
            case ActorRotation.BottomLeft:
                ActorRotation = ActorRotation.TopLeft;
                break;
            case ActorRotation.TopLeft:
                ActorRotation = ActorRotation.TopRight;
                break;
            case ActorRotation.TopRight:
                ActorRotation = ActorRotation.BottomRight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        UpdateSprite();
    }

    public void SetActorRotation(ActorRotation to)
    {
        var delta = to - ActorRotation;
        delta = delta < 0 ? delta + 4 : delta;
        for (var r = 0; r < delta; r++) 
            Rotate();
    }

    public bool HasShields => CurrentStats.Any(x => 
        x.Blueprint is CombatStat combatStat 
        && combatStat.CombatType.Equals(StatType.Shields));

    internal Actor GetActorBlueprint() => Blueprint;
}
