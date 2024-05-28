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
    private Vector2 _startingHealthPosition;
    private Vector2 _startingShieldsPosition;
    
    public override void _Ready()
    {
        base._Ready();
        
        Abilities = GetNode<Abilities>(nameof(Abilities));

        _health = GetNode<TextureProgress>($"Vitals/Health");
        _shields = GetNode<TextureProgress>($"Vitals/Shields");

        _startingHealthPosition = _health.RectPosition;
        _startingShieldsPosition = _shields.RectPosition;
        
        _health.Visible = false;
        _shields.Visible = false;
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
    }

    public override void SetOutline(bool to)
    {
        base.SetOutline(to);
        _health.Visible = to && HasHealth;
        _shields.Visible = to && HasShields;
    }

    public override void Complete()
    {
        base.Complete();
        Abilities.OnActorBirth(this);
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
        UpdateVitalsPosition();
    }

    public void SetActorRotation(ActorRotation targetRotation)
    {
        var startingRotation = ActorRotation;
        for (var r = 0; r < startingRotation.CountTo(targetRotation); r++) 
            Rotate();
    }

    public bool HasHealth => CurrentStats.Any(x =>
        x.Blueprint is CombatStat combatStat
        && combatStat.CombatType.Equals(StatType.Health));
    
    public bool HasShields => CurrentStats.Any(x => 
        x.Blueprint is CombatStat combatStat 
        && combatStat.CombatType.Equals(StatType.Shields));

    protected Actor GetActorBlueprint() => Blueprint;

    protected void UpdateVitalsPosition()
    {
        var spriteSize = Renderer.SpriteSize;
        var offsetFromX = (int)(RelativeSize.Size.x - 1) * 
                          new Vector2((int)(Constants.TileWidth / 4), (int)(Constants.TileHeight / 2)) +
                          (int)RelativeSize.Position.x * 
                          new Vector2((int)(Constants.TileWidth / 2), (int)(Constants.TileHeight / 2));
        var offsetFromY = (int)(RelativeSize.Size.y - 1) *
                          new Vector2((int)(Constants.TileWidth / 4) * -1, (int)(Constants.TileHeight / 2)) +
                          (int)RelativeSize.Position.y * 
                          new Vector2((int)(Constants.TileWidth / 2) * -1, (int)(Constants.TileHeight / 2));
        _health.RectPosition = 
            new Vector2(_startingHealthPosition.x, (spriteSize.y * -1) - 2) + offsetFromX + offsetFromY;
        _shields.RectPosition = 
            new Vector2(_startingShieldsPosition.x, (spriteSize.y * -1) - 3) + offsetFromX + offsetFromY;
    }
}
