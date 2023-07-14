using Godot;
using System.Linq;
using Array = Godot.Collections.Array;

public class Interface : CanvasLayer
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    [Signal] public delegate void MouseEntered();
    [Signal] public delegate void MouseExited();
    
    private Vector2 _mapSize = Vector2.Zero;
    
    public override void _Ready()
    {
        foreach (var control in GetChildren().OfType<Control>())
        {
            GD.Print(control.Name);
            control.Connect("mouse_entered", this, nameof(OnControlMouseEntered), new Array { control });
            control.Connect("mouse_exited", this, nameof(OnControlMouseExited), new Array { control });
        }
    }

    public void OnMapCreatorMapSizeDeclared(Vector2 mapSize)
    {
        _mapSize = mapSize;
    }

    private void OnControlMouseEntered(Control which)
    {
        if (DebugEnabled)
            GD.Print($"{nameof(Interface)}: Control '{which.Name}' was entered by mouse.");
        
        EmitSignal(nameof(MouseEntered));
    }

    private void OnControlMouseExited(Control which)
    {
        if (DebugEnabled)
            GD.Print($"{nameof(Interface)}: Control '{which.Name}' was exited by mouse.");
        
        EmitSignal(nameof(MouseExited));
    }

    private void OnMapNewTileHovered(Vector2 tileHovered, Constants.Game.Terrain terrain)
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
            terrainText = terrain.ToString().Capitalize();
        }

        GetNode<Label>("Theme/DebugPanel/Coordinates").Text = coordinatesText;
        GetNode<Label>("Theme/DebugPanel/Coordinates/Shadow").Text = coordinatesText;
        GetNode<Label>("Theme/DebugPanel/TerrainType").Text = terrainText;
        GetNode<Label>("Theme/DebugPanel/TerrainType/Shadow").Text = terrainText;
    }
}
