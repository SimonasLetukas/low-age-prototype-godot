using System;
using System.Linq;
using Godot;

public partial class InformationalText : Control
{
    public enum InfoTextType
    {
        Default,
        Placing,
        PlacingRotatable,
        Selected,
        SelectedMovement
    }

    private Vector2 _textSize = new(300, 20);
    private VBoxContainer _vBoxContainer = null!;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>(nameof(VBoxContainer));

        Visible = Config.Instance.ShowHints;
        SwitchToDefault();
    }

    public override void _Process(double delta)
    {
        Position = GetGlobalMousePosition();
    }

    public void SwitchTo(InfoTextType type, bool executionAllowed = true)
    {
        Reset();
        
        switch (type)
        {
            case InfoTextType.Default:
                AddText($"Left-click: select");
                AddText($"{GetInput(Constants.Input.FocusSelection)}: focus selection");
                break;
            case InfoTextType.PlacingRotatable:
                AddText($"{GetInput(Constants.Input.Rotate)}: rotate clockwise");
                AddText($"Left-click: place");
                AddText($"Right-click: cancel");
                AddText($"{GetInput(Constants.Input.RepeatPlacement)}: repeated placement");
                break;
            case InfoTextType.Placing:
                AddText($"Left-click: place");
                AddText($"Right-click: cancel");
                AddText($"{GetInput(Constants.Input.RepeatPlacement)}: repeated placement");
                break;
            case InfoTextType.Selected:
                AddText($"Left-click: select");
                AddText($"{GetInput(Constants.Input.FocusSelection)}: focus selection");
                break;
            case InfoTextType.SelectedMovement:
                AddText($"Left-click: select");
                if (executionAllowed) 
                    AddText($"Right-click: move");
                AddText($"{GetInput(Constants.Input.FocusSelection)}: focus selection");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        AddText($"{GetInput(Constants.Input.Flatten)}: flat mode");
    }
    
    public void SwitchToDefault()
    {
        SwitchTo(InfoTextType.Default);
    }

    private void Reset()
    {
        foreach (var child in _vBoxContainer.GetChildren()) 
            child.QueueFree();
    }
    
    private static string GetInput(string id) => InputMap.ActionGetEvents(id).First().AsText();

    private void AddText(string text)
    {
        var row = Text.Instance();
        row.Text = $"[center]{text}";
        row.IsBrighter = true;
        row.HasOutline = true;
        row.CustomMinimumSize = _textSize;
        row.Size = _textSize;
        row.SetFontSize(16);
        _vBoxContainer.AddChild(row);
    }
}
