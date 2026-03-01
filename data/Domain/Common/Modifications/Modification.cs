namespace LowAgeData.Domain.Common.Modifications;

public abstract record Modification
{
    protected Modification(Change change, float amount)
    {
        Change = change;
        Amount = amount;
    }

    public Change Change { get; }
    public float Amount { get; }
        
    public abstract void Accept(IModificationVisitor visitor);
}