using low_age_data.Domain.Entities.Actors.Units;

public class UnitNode : ActorNode<Unit>
{
    public override Unit Blueprint { get; protected set; }
}