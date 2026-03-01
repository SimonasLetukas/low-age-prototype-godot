namespace LowAgeData.Domain.Common.Modifications;

public interface IModificationVisitor
{
    void Visit(AttackModification modification);
    void Visit(DurationModification modification);
    void Visit(ResourceModification modification);
    void Visit(SizeModification modification);
    void Visit(StatCopyModification modification);
    void Visit(StatModification modification);
}