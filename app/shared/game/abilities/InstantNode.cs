using low_age_data.Domain.Abilities;

public class InstantNode : AbilityNode, INodeFromBlueprint<Instant>
{
    private Instant Blueprint { get; set; }
    
    public void SetBlueprint(Instant blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public override bool TryActivate()
    {
        // TODO execute effects
        
        return base.TryActivate();
    }
}
