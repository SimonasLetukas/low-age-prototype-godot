using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;

public static class FilterEvaluator
{
    public static IEnumerable<ITargetable> Apply(
        IEnumerable<ITargetable> items,
        IEnumerable<IFilterItem> filters,
        FilterContext context)
    {
        var root = new FilterGroup(
            Policy.Include,
            Quantifier.All,
            filters.ToList());

        return items.Where(e => Evaluate(root, e, context));
    }

    private static bool Evaluate(
        IFilterItem filter,
        ITargetable item,
        FilterContext context) => filter switch
    {
        FilterGroup group => EvaluateGroup(group, item, context),
        SpecificCombatAttribute attr => item is ActorNode actor && actor.Attributes.Contains(attr.Value),
        SpecificEntity entityId => item is EntityNode entity && entity.BlueprintId == entityId.Value,
        SpecificFaction factionId => item is ActorNode actor && actor.IsOriginallyFrom(factionId.Value),
        SpecificFlag flag => EvaluateFlag(flag.Value, item, context),
        _ => throw new NotSupportedException($"Unknown {nameof(IFilterItem)} type {filter.GetType()}")
    };

    private static bool EvaluateGroup(
        FilterGroup group,
        ITargetable item,
        FilterContext context)
    {
        var results = group.Filters
            .Select(filter => Evaluate(filter, item, context));

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
        ITargetable item,
        FilterContext context) => flag switch
    {
        _ when flag.Equals(FilterFlag.Origin) => item is EntityNode entity 
                                                 && (context.Chain.OriginEntityOrNull?.Equals(entity) ?? false),
        
        _ when flag.Equals(FilterFlag.Source) => item is EntityNode entity 
                                                 && (context.Chain.SourceEntityOrNull?.Equals(entity) ?? false),
        
        _ when flag.Equals(FilterFlag.Self) => item is EntityNode entity 
                                               && entity.Equals(context.Initiator),
        
        _ when flag.Equals(FilterFlag.Player) => item is EntityNode entity 
                                                 && entity.Player.Equals(context.CurrentPlayer),
        
        _ when flag.Equals(FilterFlag.Ally) => item is EntityNode entity 
                                               && entity.Player.Team.IsAllyTo(context.CurrentPlayer.Team) 
                                               && entity.Player.Equals(context.CurrentPlayer) is false,
        
        _ when flag.Equals(FilterFlag.Enemy) => item is EntityNode entity 
                                                && entity.Player.Team.IsEnemyTo(context.CurrentPlayer.Team),
        
        _ when flag.Equals(FilterFlag.Unit) => item is UnitNode,
        
        _ when flag.Equals(FilterFlag.Structure) => item is StructureNode,
        
        _ => throw new NotSupportedException($"Unknown {nameof(FilterFlag)} type {flag}")
    };
}