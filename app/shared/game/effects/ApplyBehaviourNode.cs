using LowAgeData.Domain.Effects;

public class ApplyBehaviourNode : EffectNode, INodeFromBlueprint<ApplyBehaviour>
{
    private ApplyBehaviour Blueprint { get; set; }
    
    public ApplyBehaviourNode(ApplyBehaviour blueprint, EntityNode initiator, Effects history) : base(history)
    {
        Initiator = initiator;
        SetBlueprint(blueprint);
    }
    
    public void SetBlueprint(ApplyBehaviour blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public override bool Execute()
    {
        if (base.Execute() is false)
            return false;
        
        Target.Behaviours.AddBehaviours(Blueprint.BehavioursToApply, History);
        return true;
    }
}