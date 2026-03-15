using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;
using Newtonsoft.Json;

public class ApplyBehaviourNode : EffectNode, INodeFromBlueprint<ApplyBehaviour>
{
    private ApplyBehaviour Blueprint { get; set; } = null!;
    
    public ApplyBehaviourNode(ApplyBehaviour blueprint, Effects history, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity) 
        : base(history, initialTargets, initiatorPlayer, initiatorEntity)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(ApplyBehaviour blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }

    public override bool Execute()
    {
        if (base.Execute() is false)
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ApplyBehaviourNode), nameof(Execute), 
                $"Executing '{Blueprint.Id}' for {nameof(FoundTargets)}: " +
                $"'{string.Join(", ", FoundTargets.Select(t => t.ToString()))}'.");
        
        foreach (var target in FoundTargets)
        {
            if (target is not EntityNode entity)
                continue;

            if (Log.DebugEnabled)
                Log.Info(nameof(ApplyBehaviourNode), nameof(Execute), 
                    $"Applying behaviours '{JsonConvert.SerializeObject(Blueprint.BehavioursToApply)}' " +
                    $"to {entity}.");
            
            entity.Behaviours.AddBehaviours(Blueprint.BehavioursToApply, History);
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => Blueprint.Filters;
}