using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Effects;

public class ModifyPlayerNode : EffectNode, INodeFromBlueprint<ModifyPlayer>
{
    private ModifyPlayer Blueprint { get; set; } = null!;
    
    public ModifyPlayerNode(ModifyPlayer blueprint, Effects history, IList<ITargetable> initialTargets, 
        Player initiatorPlayer, EntityNode? initiatorEntity) 
        : base(history, initialTargets, initiatorPlayer, initiatorEntity)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(ModifyPlayer blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }
    
    public override bool Execute()
    {
        if (IsValidated is false)
            return false;
        
        if (Log.DebugEnabled)
            Log.Info(nameof(ModifyPlayerNode), nameof(Execute), 
                $"Executing '{Blueprint.Id}' for {nameof(FoundTargets)}: " +
                $"'{string.Join(", ", FoundTargets.Select(t => t.ToString()))}'.");

        var players = new HashSet<Player>();
        foreach (var target in FoundTargets)
        {
            if (target is not EntityNode entity)
                continue;

            players.Add(entity.Player);
        }

        foreach (var player in players)
        {
            ResolveFlagFor(player);
            ResolveResourceModificationFor(player);
        }
        
        return true;
    }

    protected override IList<IFilterItem> GetFilters() => [];

    private void ResolveFlagFor(Player player)
    {
        var flags = Blueprint.ModifyFlags;
        foreach (var flag in flags)
        {
            if (flag.Equals(PlayerModificationFlag.GameLost))
                EventBus.Instance.RaiseGameLost(player);
        }
    }

    private void ResolveResourceModificationFor(Player player)
    {
        var resourcesGained = Blueprint.ResourceModifications
            .Select(r => new Payment(r.Resource, (int)r.Amount))
            .ToList();
        
        EventBus.Instance.RaiseResourceGainRequested(player, resourcesGained);
    }
}