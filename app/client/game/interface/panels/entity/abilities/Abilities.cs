using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Abilities : HBoxContainer
{
    [Export] public Dictionary<string, string> AbilityIconPaths { get; set; }
    
    [Signal] public delegate void AbilitiesPopulated();

    private readonly Dictionary<string, Texture> _abilityIcons = new Dictionary<string, Texture>();

    public override void _Ready()
    {
        Reset();
        foreach (var iconId in AbilityIconPaths.Keys)
        {
            _abilityIcons[iconId] = GD.Load<Texture>(AbilityIconPaths[iconId]);
        }
    }

    public void Populate(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            var abilityButtonScene = GD.Load<PackedScene>(AbilityButton.ScenePath);
            var abilityButton = abilityButtonScene.Instance<AbilityButton>();
            var icon = _abilityIcons[id];
            abilityButton.SetIcon(icon);
            abilityButton.SetId(id);
            AddChild(abilityButton);
        }
        
        EmitSignal(nameof(AbilitiesPopulated));
    }

    public void DeselectAll()
    {
        foreach (var abilityButton in GetChildren().OfType<AbilityButton>())
        {
            abilityButton.SetSelected(false);
        }
    }

    public bool IsAnySelected() => GetChildren().OfType<AbilityButton>()
        .Any(abilityButton => abilityButton.IsSelected);

    private void Reset()
    {
        foreach (var child in GetChildren().OfType<Node>())
        {
            child.QueueFree();
        }
    }
}
