using System.Collections.Generic;
using LowAgeData.Domain.Effects;

namespace LowAge.app.shared.game.effects;

public class SearchNode : EffectNode, INodeFromBlueprint<Search>
{
    private Search Blueprint { get; set; } = null!;
    
    public SearchNode(Search blueprint, EntityNode initiator, Effects history) 
        : base(history, initiator)
    {
        SetBlueprint(blueprint);
    }
    
    public SearchNode(Search blueprint, Player initiator, Effects history) 
        : base(history, initiator)
    {
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(Search blueprint)
    {
        Blueprint = blueprint;
        
        base.SetBlueprint(blueprint);
    }
    
    public override bool Execute()
    {
        if (base.Execute() is false)
            return false;

        var targets = FilterEvaluator.Apply(Targets, Blueprint.Filters, new FilterContext
        {
            Initiator = InitiatorEntity,
            CurrentPlayer = Players.Instance.Current,
            Chain = History
        });
        
        foreach (var target in targets)
        {
            foreach (var effectId in Blueprint.Effects)
            {
                // TODO redo the whole effect chain structure because target shouldn't be the initiator here...
                var chain = new Effects(History, effectId, target);
                if (chain.ValidateLast())
                    chain.ExecuteLast();
            }
        }
        
        return true;
    }

    protected override IEnumerable<EntityNode> GetInheritedTargets()
    {
        return Blueprint.Shape is LowAgeData.Domain.Common.Shape.Map 
            ? base.GetInheritedTargets() 
            : [];
    }
}