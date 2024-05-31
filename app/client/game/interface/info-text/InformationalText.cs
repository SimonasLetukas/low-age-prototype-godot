using System;
using System.Linq;
using Godot;

public class InformationalText : Control
{
    public enum InfoTextType
    {
        Default,
        Placing,
        PlacingRotatable,
        Selected,
        SelectedMovement
    }
    
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>(nameof(VBoxContainer));

        Visible = Config.Instance.ShowHints;
        SwitchToDefault();
    }

    public override void _Process(float delta)
    {
        RectPosition = GetGlobalMousePosition();
    }

    public void SwitchTo(InfoTextType type)
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
        foreach (var child in _vBoxContainer.GetChildren().OfType<Node>()) 
            child.QueueFree();
    }
    
    private static string GetInput(string id) => InputMap.GetActionList(id).Cast<InputEvent>().First().AsText();

    private void AddText(string text)
    {
        var row = InfoTextRow.Instance();
        row.BbcodeText = $"[center]{text}";
        _vBoxContainer.AddChild(row);
    }
}
