using System;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using Array = Godot.Collections.Array;

public class Interface : CanvasLayer
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    public event Action MouseEntered = delegate { };
    
    public event Action MouseExited = delegate { };
    
    private Vector2 _mapSize = Vector2.Zero;
    private EntityPanel _entityPanel;
    
    public override void _Ready()
    {
        _entityPanel = GetNode<EntityPanel>($"{nameof(EntityPanel)}");

        foreach (var firstLevel in GetChildren().OfType<Control>())
        {
            foreach (var control in firstLevel.GetChildren().OfType<Control>())
            {
                GD.Print(control.Name);
                control.Connect("mouse_entered", this, nameof(OnControlMouseEntered), new Array { control });
                control.Connect("mouse_exited", this, nameof(OnControlMouseExited), new Array { control });
            }
        }
    }

    public void SetMapSize(Vector2 mapSize)
    {
        _mapSize = mapSize;
    }

    private void OnControlMouseEntered(Control which)
    {
        if (DebugEnabled)
            GD.Print($"{nameof(Interface)}: Control '{which.Name}' was entered by mouse.");
        
        MouseEntered();
    }

    private void OnControlMouseExited(Control which)
    { 
        if (DebugEnabled)
            GD.Print($"{nameof(Interface)}: Control '{which.Name}' was exited by mouse.");
        
        MouseExited();
    }

    internal void OnEntitySelected(EntityNode entity)
    {
        _entityPanel.OnEntitySelected(entity);
    }

    internal void OnEntityDeselected()
    {
        _entityPanel.OnEntityDeselected();
    }

    internal void OnMapNewTileHovered(Vector2 tileHovered, Terrain terrain)
    {
        string coordinatesText;
        string terrainText;

        if (tileHovered.IsInBoundsOf(_mapSize) is false)
        {
            coordinatesText = "-, -";
            terrainText = "Mountains";
        }
        else
        {
            coordinatesText = $"{tileHovered.x}, {tileHovered.y}";
            terrainText = terrain.ToDisplayValue().Capitalize();
        }

        GetNode<Label>("Theme/DebugPanel/Coordinates").Text = coordinatesText;
        GetNode<Label>("Theme/DebugPanel/Coordinates/Shadow").Text = coordinatesText;
        GetNode<Label>("Theme/DebugPanel/TerrainType").Text = terrainText;
        GetNode<Label>("Theme/DebugPanel/TerrainType/Shadow").Text = terrainText;
    }
}
