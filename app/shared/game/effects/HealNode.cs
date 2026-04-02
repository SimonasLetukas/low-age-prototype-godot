using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;
using Newtonsoft.Json;

public class HealNode : EffectNode, INodeFromBlueprint<Heal>
{
    private Heal Blueprint { get; set; } = null!;
    
    public HealNode(Heal blueprint, Effects history, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity) 
        : base(history, initialTargets, initiatorPlayer, initiatorEntity)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(Heal blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }

    public override bool Execute()
    {
        if (IsValidated is false)
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(HealNode), nameof(Execute), 
                $"Executing '{Blueprint.Id}' for {nameof(FoundTargets)}: " +
                $"'{string.Join(", ", FoundTargets.Select(t => t.ToString()))}'.");
        
        foreach (var target in FoundTargets)
        {
            if (target is not ActorNode actor)
                continue;

            if (Log.DebugEnabled)
                Log.Info(nameof(HealNode), nameof(Execute), 
                    $"Healing {Blueprint.StatType} of {actor} by " +
                    $"{JsonConvert.SerializeObject(Blueprint.Amount)}.");
            
            actor.Heal(Blueprint.Amount.Flat, Blueprint.StatType.Equals(StatType.Shields));
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => Blueprint.Filters;
}