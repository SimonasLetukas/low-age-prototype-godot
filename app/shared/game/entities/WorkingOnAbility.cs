using LowAgeData.Domain.Common;

public record WorkingOnAbility
{
    public required IAbilityNode Ability { get; init; }
    public required TurnPhase Timing { get; init; }
    public required bool ConsumesAction { get; init; }

    public override string ToString() => $"{nameof(WorkingOnAbility)} {Ability}, {nameof(Timing)} '{Timing}', " +
                                         $"{nameof(ConsumesAction)} '{ConsumesAction}'";
}