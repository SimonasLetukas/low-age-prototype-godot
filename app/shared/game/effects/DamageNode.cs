using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;

public class DamageNode : EffectNode, INodeFromBlueprint<Damage>
{
    private Damage Blueprint { get; set; } = null!;
    
    public DamageNode(Damage blueprint, Effects history, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity) 
        : base(history, initialTargets, initiatorPlayer, initiatorEntity)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(Damage blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }

    public int ApplyDamage(ActorNode source, ActorNode target, bool isSimulation)
    {
        var damage = (int)Math.Floor(AmountNode
            .Build(Blueprint.Amount)
            .GetResolvedAmount(0, History, InitiatorEntity, target));
        var bonusDamage = (int)Math.Floor(AmountNode
            .Build(Blueprint.BonusAmount)
            .GetResolvedAmount(0, History, InitiatorEntity, target));
        var applyBonus = Blueprint.BonusTo is not null 
                         && target.Attributes.Any(x => x.Equals(Blueprint.BonusTo));
        var finalDamage = applyBonus ? damage + bonusDamage : damage;
            
        var (appliedDamage, _) = target.ReceiveDamage(source, finalDamage, Blueprint.DamageType, 
            Blueprint.IgnoresArmour, Blueprint.IgnoresShield, isSimulation);

        return appliedDamage;
    }

    public override bool Execute()
    {
        if (IsValidated is false)
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(DamageNode), nameof(Execute), 
                $"Executing '{Blueprint.Id}' for {nameof(FoundTargets)}: " +
                $"'{string.Join(", ", FoundTargets.Select(t => t.ToString()))}'.");

        var sourceEntity = History.SourceEntityOrNull ?? InitiatorEntity;
        if (sourceEntity is not ActorNode sourceActor)
        {
            if (Log.DebugEnabled)
                Log.Info(nameof(DamageNode), nameof(Execute), 
                    $"'{Blueprint.Id}' invalid {nameof(sourceEntity)} '{sourceEntity}'.");
            
            return false;
        }
        
        foreach (var target in FoundTargets)
        {
            if (target is not ActorNode targetActor || targetActor.IsBeingDestroyed)
                continue;

            if (Log.DebugEnabled)
                Log.Info(nameof(DamageNode), nameof(Execute), 
                    $"{sourceActor} is damaging {targetActor} by '{Blueprint.Id}'.");

            ApplyDamage(sourceActor, targetActor, false);
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => Blueprint.Filters;
}