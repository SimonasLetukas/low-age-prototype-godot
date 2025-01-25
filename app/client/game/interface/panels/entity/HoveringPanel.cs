using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;
using LowAgeCommon;
using LowAgeCommon.Extensions;

public partial class HoveringPanel : Control
{
    private Vector2 _mapSize;

    private InfoDisplay _infoDisplay;

    public override void _Ready()
    {
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
    
    private void OnNewTileFocused(Vector2<int> tileHovered, Terrain terrain, IList<EntityNode> occupants)
    {
        string coordinatesText;
        string terrainText;
        var occupantsText = string.Empty;

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
            
            if (occupants is null || occupants.IsEmpty())
            {
                _infoDisplay.Reset();
            }
            else
            {
                occupantsText = occupants.Aggregate(string.Empty, (current, occupant) => 
                    current + $", {occupant.DisplayName}").Substring(2);
                var entity = occupants.Last();
                _infoDisplay.SetEntityStats(entity);
                _infoDisplay.ShowView(entity is StructureNode ? View.StructureStats : View.UnitStats, true);
            }
        }

        GetNode<Label>("DebugPanel/Coordinates").Text = coordinatesText;
        GetNode<Label>("DebugPanel/TerrainType").Text = terrainText;
        GetNode<Label>("DebugPanel/EntityName").Text = occupantsText;
    }
}
