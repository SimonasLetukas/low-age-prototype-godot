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
            if (target is not ActorNode actor || actor.IsBeingDestroyed)
                continue;

            if (Log.DebugEnabled)
                Log.Info(nameof(DamageNode), nameof(Execute), 
                    $"{sourceActor} is damaging {actor} by '{Blueprint.Id}'.");
            
            var damage = (int)Math.Round(AmountNode.Build(Blueprint.Amount)
                .GetResolvedAmount(0, History, InitiatorEntity, actor));
            var bonusDamage = (int)Math.Round(AmountNode.Build(Blueprint.BonusAmount)
                .GetResolvedAmount(0, History, InitiatorEntity, actor));
            var applyBonus = Blueprint.BonusTo is not null 
                             && actor.Attributes.Any(x => x.Equals(Blueprint.BonusTo));
            var finalDamage = applyBonus ? damage + bonusDamage : damage;
            
            actor.ReceiveDamage(sourceActor, finalDamage, Blueprint.DamageType, Blueprint.IgnoresArmour, 
                Blueprint.IgnoresShield, false);
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => Blueprint.Filters;
}