using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities.Actors;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common.Shape;

/// <summary>
/// <see cref="StructureNode"/> or <see cref="UnitNode"/> with abilities and stats.
/// </summary>
public partial class ActorNode : EntityNode, INodeFromBlueprint<Actor>
{
    public bool HasHealth => HasStat(StatType.Health);
    public bool HasShields => HasStat(StatType.Shields) && Shields?.MaxAmount > 0;
    public CombatStatNode? Health => Stats.FirstOrDefault(x => x.StatType == StatType.Health);
    public CombatStatNode? Shields => Stats.FirstOrDefault(x => x.StatType == StatType.Shields);
    public bool HasMeleeArmour => HasStat(StatType.MeleeArmour);
    public bool HasRangedArmour => HasStat(StatType.RangedArmour);
    public CombatStatNode? MeleeArmour => Stats.FirstOrDefault(x => x.StatType == StatType.MeleeArmour);
    public CombatStatNode? RangedArmour => Stats.FirstOrDefault(x => x.StatType == StatType.RangedArmour);
    public bool HasMeleeAttack => Attacks.Any(x => x.IsMelee);
    public bool HasRangedAttack => Attacks.Any(x => x.IsRanged);
    public AttackStatNode? MeleeAttack => Attacks.FirstOrDefault(x => x.IsMelee);
    public AttackStatNode? RangedAttack => Attacks.FirstOrDefault(x => x.IsRanged);
    
    public IList<AttackStatNode> Attacks { get; protected set; } = null!;
    public IList<CombatStatNode> Stats { get; protected set; } = null!;
    public IList<ActorAttribute> Attributes { get; protected set; } = null!;
    public ActorRotation ActorRotation { get; protected set; }
    public Abilities Abilities { get; protected set; } = null!;

    private Actor Blueprint { get; set; } = null!;
    private Node2D StatsNode { get; set; } = null!;
    private Node2D AttacksNode { get; set; } = null!;
    private TextureProgressBar _health = null!;
    private TextureProgressBar _shields = null!;
    private Vector2 _startingHealthPosition;
    private Vector2 _startingShieldsPosition;

    public override void _Ready()
    {
        base._Ready();

        Abilities = GetNode<Abilities>(nameof(Abilities));
        StatsNode = GetNode<Node2D>("Stats");
        AttacksNode = GetNode<Node2D>("Attacks");
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
        Attacks = blueprint.Statistics
            .Where(stat => stat is AttackStat)
            .Select(stat => AttackStatNode.InstantiateAsChild((AttackStat)stat, AttacksNode))
            .ToList();
        Stats = blueprint.Statistics
            .Where(stat => stat is CombatStat)
            .Select(stat => CombatStatNode.InstantiateAsChild((CombatStat)stat, StatsNode))
            .ToList();
        Attributes = blueprint.ActorAttributes;
        ActorRotation = ActorRotation.BottomRight;
        Abilities.PopulateFromBlueprint(Blueprint.Abilities);
        Behaviours.AddOnBuildBehaviours(Abilities.GetPassives());
        
        UpdateVitalsValues();
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

    public override (int, bool) ReceiveAttack(EntityNode source, AttackType attackType, bool isSimulation)
    {
        base.ReceiveAttack(source, attackType, isSimulation);

        if (source is not ActorNode attacker)
            return (0, false);

        var damage = GetDamage(attacker, attackType);
        var damageType = attackType.Equals(AttackType.Melee) ? DamageType.Melee : DamageType.Ranged;
        return ReceiveDamage(source, damageType, damage, isSimulation);
    }

    protected override (int, bool) ReceiveDamage(EntityNode source, DamageType damageType, int amount, bool isSimulation)
    {
        base.ReceiveDamage(source, damageType, amount, isSimulation);
        
        if (source is not ActorNode attacker)
            return (0, false);

        var damage = damageType switch
        {
            _ when damageType.Equals(DamageType.Melee) => amount - (MeleeArmour?.MaxAmount ?? 0),
            _ when damageType.Equals(DamageType.Ranged) => amount - (RangedArmour?.MaxAmount ?? 0),
            _ when damageType.Equals(DamageType.Pure) => amount,
            
            _ when damageType.Equals(DamageType.CurrentMelee) => 
                amount + GetDamage(attacker, AttackType.Melee) - (MeleeArmour?.MaxAmount ?? 0),
            _ when damageType.Equals(DamageType.CurrentRanged) => 
                amount + GetDamage(attacker, AttackType.Ranged) - (RangedArmour?.MaxAmount ?? 0),
            
            _ when damageType.Equals(DamageType.OverrideMelee) => 
                GetDamage(attacker, AttackType.Melee) - (MeleeArmour?.MaxAmount ?? 0),
            _ when damageType.Equals(DamageType.OverrideRanged) => 
                GetDamage(attacker, AttackType.Ranged) - (RangedArmour?.MaxAmount ?? 0),
            
            _ when damageType.Equals(DamageType.TargetMelee) => 
                GetDamage(this, AttackType.Melee) - (MeleeArmour?.MaxAmount ?? 0),
            _ when damageType.Equals(DamageType.TargetRanged) => 
                GetDamage(this, AttackType.Ranged) - (RangedArmour?.MaxAmount ?? 0),
            
            _ => 0,
        };
        
        damage = Math.Max(damage, 1);
        
        if (isSimulation)
            return GetSimulatedResult(damage);

        if (HasShields && Shields!.CurrentAmount > 0)
        {
            Shields!.CurrentAmount -= damage;
            if (Shields!.CurrentAmount < 0)
            {
                damage = (int)Shields!.CurrentAmount * -1;
                Shields!.CurrentAmount = 0;
            }
            else
            {
                damage = 0;
            }
        }

        if (HasHealth is false)
            return (0, false);

        Health!.CurrentAmount -= damage;
        if ((int)Health!.CurrentAmount < 0)
            Destroy();
        
        UpdateVitalsValues();
        return (0, false);
    }

    protected int GetDamage(ActorNode from, AttackType attackType)
    {
        var attack = attackType.Equals(AttackType.Melee) ? from.MeleeAttack : from.RangedAttack;
        if (attack is null)
            return 0;
        
        var damage = attack.Damage;
        if (attack.HasBonusDamage && Attributes.Any(x => x.Equals(attack.BonusTo)))
            damage += attack.BonusDamage;

        return damage;
    }

    public (int, bool) GetSimulatedResult(int damage)
    {
        var healthAndShields = (int)(Health?.CurrentAmount ?? 0) + (int)(Shields?.CurrentAmount ?? 0);
        var isLethal = HasHealth && healthAndShields - damage <= 0;
        return (damage, isLethal);
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

    public List<Tiles.TileInstance> GetMeleeAttackTargetTiles(Vector2Int mapSize)
    {
        if (HasMeleeAttack is false)
            return [];
        
        var positions = GetPotentialAttackPositions(AttackType.Melee, mapSize);
        
        // TODO filter invalid attack tiles (e.g. can't melee across (unascendable) high ground)
        
        var foundTiles = new List<Tiles.TileInstance>();
        foreach (var position in positions)
        {
            var highGroundTile = GetTile(position, true);
            if (highGroundTile is not null)
            {
                foundTiles.Add(highGroundTile);
            }

            var lowGroundTile = GetTile(position, false);
            if (lowGroundTile is not null)
                foundTiles.Add(lowGroundTile);
        }

        return foundTiles;
    }
    
    public List<Tiles.TileInstance> GetRangedAttackTargetTiles(Vector2Int mapSize)
    {
        if (HasRangedAttack is false)
            return [];
        
        var positions = GetPotentialAttackPositions(AttackType.Ranged, mapSize);
        
        var foundTiles = new List<Tiles.TileInstance>();
        foreach (var position in positions)
        {
            var highGroundTile = GetTile(position, true);
            if (highGroundTile is not null)
                foundTiles.Add(highGroundTile);

            var lowGroundTile = GetTile(position, false);
            if (lowGroundTile is not null)
                foundTiles.Add(lowGroundTile);
        }

        return foundTiles;
    }
    
    protected Actor GetActorBlueprint() => Blueprint;
    
    protected bool HasStat(StatType statType) => Stats.Any(x => x.StatType.Equals(statType));

    protected void UpdateVitalsValues()
    {
        _health.MaxValue = Health?.MaxAmount ?? 0;
        _health.Value = Health?.CurrentAmount ?? 0;
        _shields.MaxValue = Shields?.MaxAmount ?? 0;
        _shields.Value = Shields?.CurrentAmount ?? 0;
    }

    protected void UpdateVitalsPosition()
    {
        var offset = GetTopCenterOffset();
        
        _health.Position = new Vector2(_startingHealthPosition.X, -2) + offset;
        _shields.Position = new Vector2(_startingShieldsPosition.X, -3) + offset;
    }
    
    private IEnumerable<Vector2Int> GetPotentialAttackPositions(AttackType attackType, Vector2Int mapSize)
    {
        var attack = attackType switch
        {
            _ when attackType.Equals(AttackType.Melee) => MeleeAttack,
            _ when attackType.Equals(AttackType.Ranged) => RangedAttack,
            _ => null,
        };

        if (attack is null)
            return [];
        
        var radius = attack.MaximumDistance;
        var skip = Math.Max(attack.MinimumDistance - 1, 0);
        return new Circle(radius, skip).ToPositions(
            EntityPrimaryPosition, 
            mapSize, 
            this);
    }
}
