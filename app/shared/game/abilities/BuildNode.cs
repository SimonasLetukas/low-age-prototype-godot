using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Shape;
using low_age_data.Domain.Entities;
using low_age_data.Shared;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;

public class BuildNode : AbilityNode, INodeFromBlueprint<Build>, ISelectable
{
    public const string ScenePath = @"res://app/shared/game/abilities/BuildNode.tscn";
    public static BuildNode Instance() => (BuildNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static BuildNode InstantiateAsChild(Build blueprint, Node parentNode)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        return ability;
    }
    
    public IList<Selection<EntityId>> Selection { get; private set; }
    
    private Build Blueprint { get; set; }
    private IShape PlacementArea { get; set; }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    public void SetBlueprint(Build blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        PlacementArea = Blueprint.PlacementArea;
        Selection = Blueprint.Selection;
    }

    public bool CanBePlacedOnTheWholeMap() => PlacementArea is low_age_data.Domain.Common.Shape.Map 
                                             && Blueprint.UseWalkableTilesAsPlacementArea is false;

    public IEnumerable<Vector2> GetPlacementPositions(EntityNode caster, Vector2 mapSize)
    {
        if (Blueprint.UseWalkableTilesAsPlacementArea && caster is StructureNode structure)
        {
            return structure.WalkablePositions;
        }

        return PlacementArea.ToPositions(caster, mapSize);
    }

    public string GetSelectableItemText(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        var entity = Data.Instance.GetEntityBlueprintById(item.Name);

        var cost = item.Cost.Aggregate(string.Empty, (current, payment) 
            => current + $"{payment.Amount} " +
               $"{Data.Instance.Blueprint.Resources.First(x => x.Id.Equals(payment.Resource)).DisplayName}, ");
        cost = cost.Remove(cost.Length - 2);

        var research = string.Empty;
        if (item.ResearchNeeded.Any())
        {
            research += "\nResearch needed to unlock: ";
            research = item.ResearchNeeded.Aggregate(research, (current, researchId) 
                => current + $"{researchId}, ");
            research = research.Remove(research.Length - 2);
        }

        return $"Build {entity.DisplayName} \n" +
               $"{research}" +
               $"\nCost: {cost.WrapToLines(Constants.MaxTooltipCharCount)} \n\n" + // TODO describe how long it will
                                                                                   // take to build this with the
                                                                                   // current income
               $"{entity.Description.WrapToLines(Constants.MaxTooltipCharCount)}";
    }

    public bool IsSelectableItemDisabled(Id selectableItemId)
    {
        var item = Selection.First(x => x.Name.Equals(selectableItemId));
        return item.ResearchNeeded.Any() || item.GrayOutIfAlreadyExists;
    }
}
