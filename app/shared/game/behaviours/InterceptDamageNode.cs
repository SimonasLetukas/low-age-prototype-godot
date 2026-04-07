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
        Blueprint = blueprint;
        base.SetBlueprint(blueprint);
        
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
            EndBehaviour(true);
        }
    }

    private (int Damage, DamageType DamageType) GetDamageDealtInsteadOf(int damage, DamageType damageType, 
        ActorNode from)
    {
        var amountDealtInstead = Blueprint.AmountDealtInstead;
        if (amountDealtInstead is null)
            return (damage, damageType);

        var amountNode = AmountNode.Build(amountDealtInstead);
        var adjustedDamage = (int)Math.Round(amountNode.GetResolvedAmount(
            damage, History, from, Parent));
        
        var adjustedDamageType = Blueprint.DamageTypeDealtInstead ?? damageType;
        
        return (adjustedDamage, adjustedDamageType);
    }
}