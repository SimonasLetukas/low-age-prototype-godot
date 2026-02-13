using LowAgeData.Domain.Effects;

public class ApplyBehaviourNode : EffectNode, INodeFromBlueprint<ApplyBehaviour>
{
    private ApplyBehaviour Blueprint { get; set; } = null!;
    
    public ApplyBehaviourNode(ApplyBehaviour blueprint, EntityNode initiator, Effects history) 
        : base(history, initiator)
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

        foreach (var target in Targets)
        {
            target.Behaviours.AddBehaviours(Blueprint.BehavioursToApply, History);
        }
        
        return true;
    }
}