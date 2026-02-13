using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;

public static class FilterEvaluator
{
    public static IEnumerable<EntityNode> Apply(
        IEnumerable<EntityNode> entities,
        IEnumerable<IFilterItem> filters,
        FilterContext context)
    {
        var root = new FilterGroup(
            Policy.Include,
            Quantifier.All,
            filters.ToList());

        return entities.Where(e => Evaluate(root, e, context));
    }

    private static bool Evaluate(
        IFilterItem item,
        EntityNode entity,
        FilterContext context) => item switch
    {
        FilterGroup group => EvaluateGroup(group, entity, context),
        SpecificCombatAttribute attr => entity is ActorNode actor && actor.Attributes.Contains(attr.Value),
        SpecificEntity entityId => entity.BlueprintId == entityId.Value,
        SpecificFaction factionId => entity is ActorNode actor && actor.IsOriginallyFrom(factionId.Value),
        SpecificFlag flag => EvaluateFlag(flag.Value, entity, context),
        _ => throw new NotSupportedException($"Unknown {nameof(IFilterItem)} type {item.GetType()}")
    };

    private static bool EvaluateGroup(
        FilterGroup group,
        EntityNode entity,
        FilterContext context)
    {
        var results = group.Items
            .Select(item => Evaluate(item, entity, context));

        var aggregated = group.Quantifier switch
        {
            Quantifier.All => results.All(r => r),
            Quantifier.Any => results.Any(r => r),
            _ => throw new NotSupportedException($"Unknown {nameof(Quantifier)} type {group.Quantifier}")
        };

        return group.Policy switch
        {
            Policy.Include => aggregated,
            Policy.Exclude => aggregated is false,
            _ => throw new NotSupportedException($"Unknown {nameof(Policy)} type {group.Policy}")
        };
    }

    private static bool EvaluateFlag(
        FilterFlag flag,
        EntityNode entity,
        FilterContext context) => flag switch
    {
        _ when flag.Equals(FilterFlag.Origin) => context.Chain.OriginEntityOrNull?.Equals(entity) ?? false,
        _ when flag.Equals(FilterFlag.Source) => context.Chain.SourceEntityOrNull?.Equals(entity) ?? false,
        _ when flag.Equals(FilterFlag.Self) => entity.Equals(context.Initiator),
        _ when flag.Equals(FilterFlag.Player) => entity.Player.Equals(context.CurrentPlayer),
        _ when flag.Equals(FilterFlag.Ally) => entity.Player.Team.IsAllyTo(context.CurrentPlayer.Team) 
                                               && entity.Player.Equals(context.CurrentPlayer) is false,
        _ when flag.Equals(FilterFlag.Enemy) => entity.Player.Team.IsEnemyTo(context.CurrentPlayer.Team),
        _ when flag.Equals(FilterFlag.Unit) => entity is UnitNode,
        _ when flag.Equals(FilterFlag.Structure) => entity is StructureNode,
        _ => throw new NotSupportedException($"Unknown {nameof(FilterFlag)} type {flag}")
    };
}