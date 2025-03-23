using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Abilities;

public partial class AbilityButtons : HBoxContainer
{
    [Export] public Godot.Collections.Dictionary<string, string> AbilityIconPaths { get; set; } 
    // TODO have paths to icons on blueprint level instead
    
    public event Action AbilitiesPopulated = delegate { };

    private readonly Dictionary<AbilityId, Texture2D> _abilityIcons = new Dictionary<AbilityId, Texture2D>();

    public override void _Ready()
    {
        Reset();
        foreach (var iconId in AbilityIconPaths.Keys)
        {
            _abilityIcons[new AbilityId(iconId)] = GD.Load<Texture2D>(AbilityIconPaths[iconId]);
        }
    }

    public void Populate(Abilities abilities)
    {
        Reset();
        
        foreach (var ability in abilities.GetChildren().OfType<AbilityNode>())
        {
            if (ability.HasButton is false)
                continue;
            
            var abilityButtonScene = GD.Load<PackedScene>(AbilityButton.ScenePath);
            var abilityButton = abilityButtonScene.Instantiate<AbilityButton>();
            if (_abilityIcons.TryGetValue(ability.Id, out var icon) is false)
            {
                icon = _abilityIcons.Values.First();
            }
            abilityButton.SetIcon(icon);
            abilityButton.SetAbility(ability);
            AddChild(abilityButton);
        }
        
        AbilitiesPopulated();
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

    public void Reset()
    {
        foreach (var child in GetChildren().OfType<Node>())
        {
            child.QueueFree();
        }
    }
}
