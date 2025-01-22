using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;

public partial class HoveringPanel : Control
{
    private Vector2 _mapSize;

    private InfoDisplay _infoDisplay;

    public override void _Ready()
    {
        _infoDisplay = GetNode<InfoDisplay>($"{nameof(InfoDisplay)}");
        _infoDisplay.Reset();
    }

    public void SetMapSize(Vector2 mapSize)
    {
        _mapSize = mapSize;
    }
    
    public void OnMapNewTileHovered(Vector2 tileHovered, Terrain terrain, IList<EntityNode> occupants)
    {
        string coordinatesText;
        string terrainText;
        var occupantsText = string.Empty;

        if (tileHovered.IsInBoundsOf(_mapSize) is false)
        {
            coordinatesText = "-, -";
            terrainText = "Mountains";
            _infoDisplay.Reset();
        }
        else
        {
            coordinatesText = $"{tileHovered.X}, {tileHovered.Y}";
            terrainText = terrain.ToDisplayValue().Capitalize();
            
            if (occupants.IsEmpty())
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
