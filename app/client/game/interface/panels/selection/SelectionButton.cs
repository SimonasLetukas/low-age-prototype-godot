using System;
using Godot;
using low_age_data.Domain.Entities;
using low_age_data.Shared;

public partial class SelectionButton : BaseButton
{
    public const string ScenePath = @"res://app/client/game/interface/panels/selection/SelectionButton.tscn";
    public static SelectionButton Instance() => (SelectionButton) GD.Load<PackedScene>(ScenePath).Instance();
    public static SelectionButton InstantiateAsChild(ISelectable abilityNode, Id selectionId, Node parentNode)
    {
        var selectionButton = Instance();
        selectionButton.SetupSelectionButton(abilityNode, selectionId);
        parentNode.AddChild(selectionButton);
        return selectionButton;
    }
    
    public new event Action<ISelectable, Id> Clicked = delegate { };
    
    public ISelectable Ability { get; private set; }
    public Id SelectionId { get; private set; }
    
    public override void _Ready()
    {
        base._Ready();

        base.Clicked += OnButtonClicked;
    }

    public override void _ExitTree()
    {
        base.Clicked -= OnButtonClicked;
        base._ExitTree();
    }

    public void SetupSelectionButton(ISelectable ability, Id selectionId)
    {
        Ability = ability;
        SelectionId = selectionId;
        
        TooltipText = ability.GetSelectableItemText(selectionId);

        var entity = Data.Instance.GetEntityBlueprintById((EntityId)selectionId);
        if (entity?.Sprite2D != null)
            SetIcon(GD.Load<Texture2D>(entity.Sprite2D));
        
        if (Config.Instance.ResearchEnabled)
            SetDisabled(ability.IsSelectableItemDisabled(selectionId));
    }
    
    private void OnButtonClicked() => Clicked(Ability, SelectionId);
}
