using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeCommon;
using LowAgeCommon.Extensions;

public partial class HoveringPanel : Control
{
    private Vector2 _mapSize;

    private AvailableActionsDisplay _availableActions = null!;
    private GridContainer _behaviours = null!;
    private InfoDisplay _infoDisplay = null!;

    public override void _Ready()
    {
        _availableActions = GetNode<AvailableActionsDisplay>($"{nameof(AvailableActionsDisplay)}");
        _behaviours = GetNode<GridContainer>("Behaviours");
        _infoDisplay = GetNode<InfoDisplay>($"{nameof(InfoDisplay)}");
        _infoDisplay.Reset();
        
        EventBus.Instance.NewTileFocused += OnNewTileFocused;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.NewTileFocused -= OnNewTileFocused;
        base._ExitTree();
    }

    public void SetMapSize(Vector2 mapSize)
    {
        _mapSize = mapSize;
    }
    
    private void OnNewTileFocused(Vector2Int tileHovered, Terrain terrain, IList<EntityNode>? occupants)
    {
        string coordinatesText;
        string terrainText;
        var occupantsText = string.Empty;
        
        RemoveAllBehaviours();

        if (tileHovered.IsInBoundsOf(_mapSize.ToVector2()) is false)
        {
            coordinatesText = "-, -";
            terrainText = "Mountains";
            _infoDisplay.Reset();
        }
        else
        {
            coordinatesText = $"{tileHovered.X}, {tileHovered.Y}";
            terrainText = terrain.ToDisplayValue().Capitalize();
            
            if (occupants is null || occupants.IsEmpty() || occupants.Last().IsRevealed() is false)
            {
                _infoDisplay.Reset();
                _availableActions.Reset();
            }
            else
            {
                occupantsText = occupants.Aggregate(string.Empty, (current, occupant) => 
                    current + $", {occupant.DisplayName}")[2..];
                var entity = occupants.Last();
                _infoDisplay.SetEntityStats(entity);
                _infoDisplay.ShowView(entity is StructureNode ? View.StructureStats : View.UnitStats, true);
                
                _availableActions.Reset();
                if (entity is ActorNode actor)
                    _availableActions.Populate(actor.ActionEconomy);
                
                AddBehaviours(entity);
            }
        }

        GetNode<Label>("DebugPanel/Coordinates").Text = coordinatesText;
        GetNode<Label>("DebugPanel/TerrainType").Text = terrainText;
        GetNode<Label>("DebugPanel/EntityName").Text = occupantsText;
    }
    
    private void RemoveAllBehaviours()
    {
        foreach (var behaviour in _behaviours.GetChildren())
        {
            behaviour.QueueFree();
        }
    }

    private void AddBehaviours(EntityNode entity)
    {
        foreach (var behaviours in entity.Behaviours
                     .GetAll()
                     .GroupBy(b => b.BlueprintId))
        {
            var behaviour = behaviours.OrderBy(b => b.CurrentDuration).First();
            BehaviourBox.InstantiateAsChild(behaviour, _behaviours);
        }
    }
}
