using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities.Actors;
using LowAgeCommon.Extensions;

/// <summary>
/// <see cref="StructureNode"/> or <see cref="UnitNode"/> with abilities and stats.
/// </summary>
public partial class ActorNode : EntityNode, INodeFromBlueprint<Actor>
{
    public IList<StatNode> CurrentStats { get; protected set; } = null!;
    public IList<ActorAttribute> Attributes { get; protected set; } = null!;
    public ActorRotation ActorRotation { get; protected set; }
    public Abilities Abilities { get; protected set; } = null!;

    private Actor Blueprint { get; set; } = null!;
    private TextureProgressBar _health = null!;
    private TextureProgressBar _shields = null!;
    private Vector2 _startingHealthPosition;
    private Vector2 _startingShieldsPosition;

    public override void _Ready()
    {
        base._Ready();

        Abilities = GetNode<Abilities>(nameof(Abilities));

        _health = GetNode<TextureProgressBar>($"Vitals/Health");
        _shields = GetNode<TextureProgressBar>($"Vitals/Shields");

        _startingHealthPosition = _health.Position;
        _startingShieldsPosition = _shields.Position;

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

    public override void ForcePlace(EntityPlacedEvent @event)
    {
        SetActorRotation(@event.ActorRotation);
        base.ForcePlace(@event);
    }

    public override void Complete()
    {
        base.Complete();
        Abilities.OnActorBirth();
    }

    public virtual void Rotate()
    {
        ActorRotation = ActorRotation switch
        {
            ActorRotation.BottomRight => ActorRotation.BottomLeft,
            ActorRotation.BottomLeft => ActorRotation.TopLeft,
            ActorRotation.TopLeft => ActorRotation.TopRight,
            ActorRotation.TopRight => ActorRotation.BottomRight,
            _ => throw new ArgumentOutOfRangeException()
        };

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
        const int quarterTileWidth = Constants.TileWidth / 4;
        const int halfTileWidth = Constants.TileWidth / 2;
        const int halfTileHeight = Constants.TileHeight / 2;
        
        var spriteSize = Renderer.SpriteSize;
        var offsetFromX = (RelativeSize.Size.X - 1) * new Vector2(quarterTileWidth, halfTileHeight) +
                          RelativeSize.Start.X * new Vector2(halfTileWidth, halfTileHeight);
        var offsetFromY = (RelativeSize.Size.Y - 1) * new Vector2(quarterTileWidth * -1, halfTileHeight) +
                          RelativeSize.Start.Y * new Vector2(halfTileWidth * -1, halfTileHeight);
        
        _health.Position = new Vector2(_startingHealthPosition.X,
            (spriteSize.Y * -1) - 2 - Renderer.YHighGroundOffset) + offsetFromX + offsetFromY;
        _shields.Position = new Vector2(_startingShieldsPosition.X,
            (spriteSize.Y * -1) - 3 - Renderer.YHighGroundOffset) + offsetFromX + offsetFromY;
    }
}
