using LowAgeData.Domain.Abilities;

public partial class InstantNode : AbilityNode, INodeFromBlueprint<Instant>
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
