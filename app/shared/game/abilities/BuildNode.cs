using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeCommon.Extensions;

public partial class BuildNode : AbilityNode, INodeFromBlueprint<Build>, ISelectable
{
    public const string ScenePath = @"res://app/shared/game/abilities/BuildNode.tscn";
    public static BuildNode Instance() => (BuildNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BuildNode InstantiateAsChild(Build blueprint, Node parentNode, ActorNode owner)
    {
        var ability = Instance();
        parentNode.AddChild(ability);
        ability.SetBlueprint(blueprint);
        ability.Owner = owner;
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

    public bool CanBePlacedOnTheWholeMap() => PlacementArea is LowAgeData.Domain.Common.Shape.Map 
                                              && Blueprint.UseWalkableTilesAsPlacementArea is false;

    public IEnumerable<Vector2Int> GetPlacementPositions(EntityNode caster, Vector2Int mapSize)
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
