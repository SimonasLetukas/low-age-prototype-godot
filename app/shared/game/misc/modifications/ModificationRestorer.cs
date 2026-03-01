using LowAgeData.Domain.Common.Modifications;

public class ModificationRestorer(EntityNode entity) : IModificationVisitor
{
    public void Visit(AttackModification modification)
    {
        if (entity is not ActorNode actor)
            return;

        foreach (var attackStat in actor.Attacks) 
            attackStat.Remove(modification);
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
            stat.Remove(modification);
    }
}