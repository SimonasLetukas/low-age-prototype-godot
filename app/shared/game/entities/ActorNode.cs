using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeCommon;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities.Actors;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common.Shape;
using MultipurposePathfinding;

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
    public bool HasInitiative => HasStat(StatType.Initiative);
    public CombatStatNode? Initiative => Stats.FirstOrDefault(x => x.StatType == StatType.Initiative);
    public bool HasMeleeAttack => Attacks.Any(x => x.IsMelee);
    public bool HasRangedAttack => Attacks.Any(x => x.IsRanged);
    public AttackStatNode? MeleeAttack => Attacks.FirstOrDefault(x => x.IsMelee);
    public AttackStatNode? RangedAttack => Attacks.FirstOrDefault(x => x.IsRanged);
    
    public ActionEconomy ActionEconomy { get; protected set; } = null!;
    public IList<AttackStatNode> Attacks { get; protected set; } = null!;
    public IList<CombatStatNode> Stats { get; protected set; } = null!;
    public IList<ActorAttribute> Attributes { get; protected set; } = null!;
    public IsometricRotation ActorRotation { get; protected set; }
    public Abilities Abilities { get; protected set; } = null!;
    public IList<WorkingOnAbility> WorkingOn { get; set; } = new List<WorkingOnAbility>();

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
        ActorRotation = IsometricRotation.BottomRight;
        Abilities.PopulateFromBlueprint(Blueprint.Abilities);
        Behaviours.AddOnBuildBehaviours(Abilities.GetPassives());
        CreationProgress = Behaviours.GetBuildables().FirstOrDefault();
        if (CreationProgress is not null)
        {
            CreationProgress.Updated += OnCreationProgressUpdated;
            CreationProgress.Completed += OnCreationProgressCompleted;
        }
        ActionEconomy = ActionEconomy.For(this);
        
        UpdateVitalsValuesForDisplay();
    }

    public override void SetOutline(bool to)
    {
        base.SetOutline(to);
        _health.Visible = to && HasHealth;
        _shields.Visible = to && HasShields;
    }

    public override void ForcePlace(EntityPlacedResponseEvent @event)
    {
        SetActorRotation(@event.ActorRotation);
        base.ForcePlace(@event);
    }

    protected override void Complete()
    {
        base.Complete();

        if (CreationProgress is not null)
        {
            CreationProgress.Updated -= OnCreationProgressUpdated;
            CreationProgress.Completed -= OnCreationProgressCompleted;
        }
        
        Behaviours.RemoveAll<BuildableNode>();
        CreationProgress = null;
        
        Abilities.OnActorBirth();
    }

    public override void SetCost(IList<Payment> cost)
    {
        base.SetCost(cost);
        
        if (HasHealth) 
            Health!.CurrentAmount = HasCost ? 1 : Health!.MaxAmount;

        if (HasShields) 
            Shields!.CurrentAmount = HasCost ? 1 : Shields!.MaxAmount;
        
        UpdateVitalsValuesForDisplay();
    }

    public void RestoreActionEconomy(TurnPhase phase, bool restoringOnlyAbilityAction)
    {
        if (IsCompleted())
            ActionEconomy.Restore(phase, restoringOnlyAbilityAction);
    }

    public void AddWorkingOnAbility(IAbilityNode ability, TurnPhase timing, bool consumesAction)
    {
        var workingOn = new WorkingOnAbility
        {
            Ability = ability,
            Timing = timing,
            ConsumesAction = consumesAction
        };
        
        if (WorkingOn.Contains(workingOn) is false)
            WorkingOn.Add(workingOn);
    }

    public void RemoveWorkingOnAbility(IAbilityNode ability)
    {
        foreach (var workingOnAbility in WorkingOn
                     .ToList()
                     .Where(workingOnAbility => workingOnAbility.Ability.Equals(ability)))
        {
            WorkingOn.Remove(workingOnAbility);
        }
    }

    public bool CanAttack(bool? isMelee) => isMelee is null 
        ? CanAttack(AttackType.Melee) || CanAttack(AttackType.Ranged) 
        : CanAttack(isMelee is true ? AttackType.Melee : AttackType.Ranged);

    public bool CanAttack(AttackType attackType) 
        => (attackType.Equals(AttackType.Melee) && ActionEconomy.CanMeleeAttack) 
           || (attackType.Equals(AttackType.Ranged) && ActionEconomy.CanRangedAttack);

    public override (int Damage, bool IsLethal) ReceiveAttack(EntityNode source, AttackType attackType, 
        bool isSimulation)
    {
        base.ReceiveAttack(source, attackType, isSimulation);

        if (source is not ActorNode attacker)
            return (0, false);

        var (damage, _) = GetDamage(attacker, attackType);
        return ReceiveDamage(source, damage, isSimulation);
    }

    protected override (int Damage, bool IsLethal) ReceiveDamage(EntityNode source, int amount, bool isSimulation)
    {
        base.ReceiveDamage(source, amount, isSimulation);
        
        if (source is not ActorNode)
            return (0, false);
        
        if (isSimulation)
            return GetSimulatedResult(amount);

        if (HasShields && Shields!.CurrentAmount > 0)
        {
            Shields!.CurrentAmount -= amount;
            if (Shields!.CurrentAmount < 0)
            {
                amount = (int)Shields!.CurrentAmount * -1;
                Shields!.CurrentAmount = 0;
            }
            else
            {
                amount = 0;
            }
        }

        if (HasHealth is false)
            return (0, false);

        Health!.CurrentAmount -= amount;
        if ((int)Health!.CurrentAmount < 0)
            Destroy();
        
        UpdateVitalsValuesForDisplay();
        return (0, false);
    }

    public virtual void Rotate()
    {
        ActorRotation = ActorRotation switch
        {
            IsometricRotation.BottomRight => IsometricRotation.BottomLeft,
            IsometricRotation.BottomLeft => IsometricRotation.TopLeft,
            IsometricRotation.TopLeft => IsometricRotation.TopRight,
            IsometricRotation.TopRight => IsometricRotation.BottomRight,
            _ => throw new ArgumentOutOfRangeException()
        };

        UpdateSprite();
        UpdateVitalsPosition();
    }

    public void SetActorRotation(IsometricRotation targetRotation)
    {
        var startingRotation = ActorRotation;
        for (var r = 0; r < startingRotation.CountTo(targetRotation); r++)
            Rotate();
    }

    public virtual List<Tiles.TileInstance> GetMeleeAttackTargetTiles(Vector2Int mapSize, 
        IEnumerable<Point> availablePoints) 
        => HasMeleeAttack ? GetAttackTargetTiles(AttackType.Melee, mapSize) : [];
    
    public virtual List<Tiles.TileInstance> GetRangedAttackTargetTiles(Vector2Int mapSize) => HasRangedAttack 
        ? GetAttackTargetTiles(AttackType.Ranged, mapSize)
        : [];

    protected Actor GetActorBlueprint() => Blueprint;
    
    protected bool HasStat(StatType statType) => Stats.Any(x => x.StatType.Equals(statType));

    protected void UpdateVitalsValuesForDisplay()
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
    
    protected (int Amount, DamageType Type) GetDamage(ActorNode from, int damage, DamageType type)
    {
        var (initialAmount, initialType) = ResolveDamageType(damage, type, from);
        
        var (interceptedAmount, interceptedType) = Behaviours.InterceptDamage(initialAmount, initialType, from);
        
        var (adjustedAmount, adjustedType) = ResolveDamageType(interceptedAmount, interceptedType, from);
        
        var amountAfterArmour = ResolveDamageArmour(adjustedAmount, adjustedType);
        
        return (amountAfterArmour, adjustedType);
    }
    
    protected override (int Amount, DamageType Type) GetDamage(EntityNode from, AttackType attackType)
    {
        if (from is not ActorNode fromActor)
            return base.GetDamage(from, attackType);
        
        var attack = attackType.Equals(AttackType.Melee) ? fromActor.MeleeAttack : fromActor.RangedAttack;
        if (attack is null)
            return (0, DamageType.Pure);
        
        var damage = attack.Damage;
        if (attack.HasBonusDamage && Attributes.Any(x => x.Equals(attack.BonusTo)))
            damage += attack.BonusDamage;

        var damageType = attackType.Equals(AttackType.Melee) ? DamageType.Melee : DamageType.Ranged;
        return GetDamage(fromActor, damage, damageType);
    }
    
    protected override int ResolveDamageArmour(int damage, DamageType damageType) => damageType switch
    {
        _ when damage == 0 => 0,
        
        _ when damageType.Equals(DamageType.Melee) => Math.Max(damage - (MeleeArmour?.MaxAmount ?? 0), 1),
        _ when damageType.Equals(DamageType.Ranged) => Math.Max(damage - (RangedArmour?.MaxAmount ?? 0), 1),
        _ when damageType.Equals(DamageType.Pure) => damage,
        
        _ => 0,
    };

    protected override void OnPhaseStarted(int turn, TurnPhase phase)
    {
        RestoreActionEconomy(phase, false);
        
        base.OnPhaseStarted(turn, phase);
    }
    
    private List<Tiles.TileInstance> GetAttackTargetTiles(AttackType attackType, Vector2Int mapSize)
    {
        var positions = GetPotentialAttackPositions(attackType, mapSize);
        
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
        return new Circle(radius, skip).ToPositions(EntityPrimaryPosition, mapSize, this);
    }

    private (int Damage, bool IsLethal) GetSimulatedResult(int damage)
    {
        var healthAndShields = (int)(Health?.CurrentAmount ?? 0) + (int)(Shields?.CurrentAmount ?? 0);
        var isLethal = HasHealth && healthAndShields - damage <= 0;
        return (damage, isLethal);
    }
    
    private void OnCreationProgressUpdated((int DeltaGainedHealth, int DeltaGainedShields) delta)
    {
        Health!.CurrentAmount += delta.DeltaGainedHealth;
        
        if (HasShields)
            Shields!.CurrentAmount += delta.DeltaGainedShields;
        
        UpdateVitalsValuesForDisplay();
    }
    
    private void OnCreationProgressCompleted() => Complete();
}
