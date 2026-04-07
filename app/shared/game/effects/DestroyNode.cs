using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Effects;

public class DestroyNode : EffectNode, INodeFromBlueprint<Destroy>
{
    private Destroy Blueprint { get; set; } = null!;
    
    public DestroyNode(Destroy blueprint, Effects history, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity) 
        : base(history, initialTargets, initiatorPlayer, initiatorEntity)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(Destroy blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }

    public override bool Execute()
    {
        if (IsValidated is false)
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(DestroyNode), nameof(Execute), 
                $"Executing '{Blueprint.Id}' for {nameof(FoundTargets)}: " +
                $"'{string.Join(", ", FoundTargets.Select(t => t.ToString()))}'.");

        var source = History.SourceEntityOrNull;
        var triggersOnDeathBehaviours = Blueprint.BlocksBehaviours is false;
        
        foreach (var target in FoundTargets)
        {
            if (target is not EntityNode entity)
                continue;

            if (Log.DebugEnabled)
                Log.Info(nameof(DestroyNode), nameof(Execute), 
                    $"'{Blueprint.Id}' is destroying {entity} by {nameof(source)} '{source}', " +
                    $"{nameof(triggersOnDeathBehaviours)} '{triggersOnDeathBehaviours}'.");
            
            entity.Destroy(source, triggersOnDeathBehaviours);
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => [];
}