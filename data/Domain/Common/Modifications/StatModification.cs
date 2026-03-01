namespace LowAgeData.Domain.Common.Modifications;

public record StatModification : Modification
{
    public StatModification(
        Change change, 
        float amount,
        StatType statType) : base(change, amount)
    {
        StatType = statType;
    }

    public StatType StatType { get; }
    
    public override void Accept(IModificationVisitor visitor) => visitor.Visit(this);
}