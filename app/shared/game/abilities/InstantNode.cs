using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public partial class InstantNode : AbilityNode, INodeFromBlueprint<Instant>
{
    private Instant Blueprint { get; set; } = null!;
    
    public void SetBlueprint(Instant blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
    }

    public override bool TryActivate(TurnPhase currentTurnPhase, ActorNode? actorInAction)
    {
        // TODO execute effects
        
        return base.TryActivate(currentTurnPhase, actorInAction);
    }
}
