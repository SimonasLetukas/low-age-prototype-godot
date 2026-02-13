using System;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Flags;

public partial class InterceptDamageNode : BehaviourNode, INodeFromBlueprint<InterceptDamage>
{
    private const string ScenePath = @"res://app/shared/game/behaviours/InterceptDamageNode.tscn";
    private static InterceptDamageNode Instance() => (InterceptDamageNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static InterceptDamageNode InstantiateAsChild(InterceptDamage blueprint, Node parentNode, Effects history, 
        EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.History = history;
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }
    
    private InterceptDamage Blueprint { get; set; } = null!;

    private int _interceptionsLeft;

    public void SetBlueprint(InterceptDamage blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        
        _interceptionsLeft = Blueprint.NumberOfInterceptions;
    }

    public bool HasMultiplication() => Blueprint.AmountDealtInstead?.Multiplier is not null 
                                       || Blueprint.AmountShared?.Multiplier is not null;

    public (int Damage, DamageType DamageType) Resolve(int damage, DamageType damageType, ActorNode from)
    {
        if (CanIntercept(damageType) is false)
            return (damage, damageType);

        var (adjustedDamage, adjustedDamageType) = GetDamageDealtInsteadOf(damage, damageType, from);
        
        ReduceInterceptionCount();
        
        return (adjustedDamage, adjustedDamageType);
    }

    private bool CanIntercept(DamageType damageType)
    {
        if (Blueprint.InterceptedDamageTypes.Any(t => t.Equals(damageType)) is false)
            return false;

        if (_interceptionsLeft == 0)
            return false;

        return true;
    }

    private void ReduceInterceptionCount()
    {
        _interceptionsLeft--;
        
        if (_interceptionsLeft == 0)
        {
            Destroy();
        }
    }

    private (int Damage, DamageType DamageType) GetDamageDealtInsteadOf(int damage, DamageType damageType, 
        ActorNode from)
    {
        var amountDealtInstead = Blueprint.AmountDealtInstead;
        if (amountDealtInstead is null)
            return (damage, damageType);

        var flat = amountDealtInstead.Flat;
        var multiplied = GetMultipliedDamage(amountDealtInstead, damage, from);

        var adjustedDamage = flat + multiplied;
        var adjustedDamageType = Blueprint.DamageTypeDealtInstead ?? damageType;
        
        return (adjustedDamage, adjustedDamageType);
    }

    private int GetMultipliedDamage(Amount amount, int currentDamage, ActorNode from)
    {
        if (amount.Multiplier is null)
            return 0;

        var target = amount.MultiplyTarget;
        if (target.Equals(Location.Inherited))
            return (int)Math.Round(currentDamage * amount.Multiplier.Value);

        var entity = target switch
        {
            _ when target.Equals(Location.Self) => from,
            _ when target.Equals(Location.Actor) => Parent,
            _ when target.Equals(Location.Source) => History.SourceEntityOrNull ?? from,
            _ when target.Equals(Location.Origin) => History.OriginEntityOrNull ?? from,
            _ => Parent
        };

        var multiplierOf = amount.MultiplierOf;
        if (entity is not ActorNode actor || multiplierOf is null)
            return 0;

        var baseValue = multiplierOf switch
        {
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.Health) => actor.Health?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.Vitals)
                => actor.Health?.CurrentAmount ?? 0 + actor.Shields?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.MissingHealth)
                => actor.Health?.MaxAmount ?? 0 - actor.Health?.CurrentAmount ?? 0,
            
            _ when multiplierOf.Equals(AmountMultiplyOfFlag.MissingVitals)
                => (actor.Health?.MaxAmount ?? 0 + actor.Shields?.MaxAmount ?? 0) -
                   (actor.Health?.CurrentAmount ?? 0 + actor.Shields?.CurrentAmount ?? 0),
            
            _ => 0
        };

        return (int)Math.Round(baseValue * amount.Multiplier.Value);
    }
}