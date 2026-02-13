public class FilterContext
{
    public required EntityNode? Initiator { get; init; }
    public required Player CurrentPlayer { get; init; }
    public required Effects Chain { get; init; }
}