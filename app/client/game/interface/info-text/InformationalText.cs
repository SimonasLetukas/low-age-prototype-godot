using System;
using System.Linq;
using Godot;

public partial class InformationalText : Control
{
    public enum InfoTextType
    {
        Placing,
        PlacingRotatable,
        Selected,
        SelectedMovement
    }
    
    private bool _enabled = false;
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>(nameof(VBoxContainer));
        
        Disable();
        Reset();
    }

    public override void _Process(float delta)
    {
        if (_enabled is false)
            return;

        Position = GetGlobalMousePosition();
    }

    public void Enable(InfoTextType type)
    {
        Reset();
        
        switch (type)
        {
            case InfoTextType.PlacingRotatable:
                AddText($"{GetInput(Constants.Input.Rotate)}: rotate clockwise");
                AddText($"Left-click: place");
                AddText($"Right-click: cancel");
                break;
            case InfoTextType.Placing:
                AddText($"Left-click: place");
                AddText($"Right-click: cancel");
                break;
            case InfoTextType.Selected:
                AddText($"Left-click: select");
                break;
            case InfoTextType.SelectedMovement:
                AddText($"Left-click: select");
                AddText($"Right-click: move");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        _enabled = true;
        Visible = true;
    }
    
    public void Disable()
    {
        _enabled = false;
        Visible = false;
    }

    private void Reset()
    {
        foreach (var child in _vBoxContainer.GetChildren().OfType<Node>()) 
            child.QueueFree();
    }
    
    private static string GetInput(string id) => InputMap.ActionGetEvents(id).Cast<InputEvent>().First().AsText();

    private void AddText(string text)
    {
        var row = InfoTextRow.Instance();
        row.Text = $"[center]{text}";
        _vBoxContainer.AddChild(row);
    }
}
