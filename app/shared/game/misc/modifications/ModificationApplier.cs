using LowAgeData.Domain.Common.Modifications;

public class ModificationApplier(EntityNode entity, bool restoreChanges) : IModificationVisitor
{
    public void Visit(AttackModification modification)
    {
        if (entity is not ActorNode actor)
            return;

        foreach (var attackStat in actor.Attacks) 
            attackStat.Apply(modification, restoreChanges is false);
    }

    public void Visit(DurationModification modification)
    {
        return;
    }

    public void Visit(ResourceModification modification)
    {
        return;
    }

    public void Visit(SizeModification modification)
    {
        return;
    }

    public void Visit(StatCopyModification modification)
    {
        return;
    }

    public void Visit(StatModification modification)
    {
        if (entity is not ActorNode actor)
            return;

        foreach (var stat in actor.Stats) 
            stat.Apply(modification, restoreChanges is false);
    }
}