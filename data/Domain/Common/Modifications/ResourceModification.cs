using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Common.Modifications;

public record ResourceModification : Modification
{
    public ResourceModification(
        float amount,
        ResourceId resource) : base(Change.AddCurrent, amount)
    {
        Resource = resource;
    }

    public ResourceId Resource { get; }
        
    public override void Accept(IModificationVisitor visitor) => visitor.Visit(this);
}