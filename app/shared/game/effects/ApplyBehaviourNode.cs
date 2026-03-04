using System.Collections.Generic;
using LowAgeData.Domain.Effects;

public class ApplyBehaviourNode : EffectNode, INodeFromBlueprint<ApplyBehaviour>
{
    private ApplyBehaviour Blueprint { get; set; } = null!;
    
    public ApplyBehaviourNode(ApplyBehaviour blueprint, Effects history, ITargetable? initialTarget, EntityNode? initiator) 
        : base(history, initialTarget, initiator)
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

        foreach (var target in FoundTargets)
        {
            if (target is not EntityNode entity)
                continue;
            
            entity.Behaviours.AddBehaviours(Blueprint.BehavioursToApply, History);
        }
        
        return true;
    }
}