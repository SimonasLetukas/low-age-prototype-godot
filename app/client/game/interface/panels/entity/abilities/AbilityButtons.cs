using Godot;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Abilities;

public class AbilityButtons : HBoxContainer
{
    [Export] public Dictionary<string, string> AbilityIconPaths { get; set; } // TODO have paths to icons on blueprint level instead
    
    [Signal] public delegate void AbilitiesPopulated();

    private readonly Dictionary<AbilityId, Texture> _abilityIcons = new Dictionary<AbilityId, Texture>();

    public override void _Ready()
    {
        Reset();
        foreach (var iconId in AbilityIconPaths.Keys)
        {
            _abilityIcons[new AbilityId(iconId)] = GD.Load<Texture>(AbilityIconPaths[iconId]);
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
            var abilityButton = abilityButtonScene.Instance<AbilityButton>();
            if (_abilityIcons.TryGetValue(ability.Id, out var icon) is false)
            {
                icon = _abilityIcons.Values.First();
            }
            abilityButton.SetIcon(icon);
            abilityButton.SetAbility(ability);
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

    public void Reset()
    {
        foreach (var child in GetChildren().OfType<Node>())
        {
            child.QueueFree();
        }
    }
}
