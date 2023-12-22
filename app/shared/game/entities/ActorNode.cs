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
    public List<StatNode> CurrentStats { get; protected set; }
    public ActorRotation ActorRotation { get; protected set; }
    
    private Actor Blueprint { get; set; }
    private TextureProgress _health;
    private TextureProgress _shields;
    
    public override void _Ready()
    {
        base._Ready();

        _health = GetNode<TextureProgress>("Health");
        _shields = GetNode<TextureProgress>("Shields");
    }
    
    public void SetBlueprint(Actor blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        ActorRotation = ActorRotation.BottomRight;
        CurrentStats = blueprint.Statistics.Select(stat => StatNode.InstantiateAsChild(stat, this)).ToList();
        
        var spriteSize = _sprite.Texture.GetSize();
        _health.RectPosition = new Vector2(_health.RectPosition.x, (spriteSize.y * -1) - 2);
        _health.Visible = false;
        _shields.RectPosition = new Vector2(_shields.RectPosition.x, (spriteSize.y * -1) - 1);
        _shields.Visible = false;
    }

    public override void SetOutline(bool to)
    {
        base.SetOutline(to);
        _health.Visible = to;
        _shields.Visible = to && HasShields;
    }

    public bool HasShields => CurrentStats.Any(x => 
        x.Blueprint is CombatStat combatStat 
        && combatStat.CombatType.Equals(StatType.Shields));
}
