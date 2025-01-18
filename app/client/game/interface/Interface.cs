using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities;
using Array = Godot.Collections.Array;

public partial class Interface : CanvasLayer
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    public event Action MouseEntered = delegate { };
    public event Action MouseExited = delegate { };
    public event Action<BuildNode, EntityId> SelectedToBuild = delegate { };
    
    private EntityPanel _entityPanel;
    private SelectionPanel _selectionPanel;
    private InformationalText _informationalText;
    private HoveringPanel _hoveringPanel;
    
    public override void _Ready()
    {
        base._Ready();
        
        _entityPanel = GetNode<EntityPanel>($"{nameof(EntityPanel)}");
        _selectionPanel = GetNode<SelectionPanel>($"{nameof(SelectionPanel)}");
        _informationalText = GetNode<InformationalText>($"{nameof(InformationalText)}");
        _hoveringPanel = GetNode<HoveringPanel>($"{nameof(HoveringPanel)}");

        foreach (var firstLevel in GetChildren().OfType<Control>())
        {
            foreach (var control in firstLevel.GetChildren().OfType<Control>())
            {
                if (DebugEnabled) GD.Print(control.Name);
                control.Connect("mouse_entered", new Callable(this, nameof(OnControlMouseEntered)), new Array { control });
                control.Connect("mouse_exited", new Callable(this, nameof(OnControlMouseExited)), new Array { control });
            }
        }

        _entityPanel.AbilityViewOpened += _selectionPanel.OnSelectableAbilityPressed;
        _entityPanel.AbilityViewClosed += _selectionPanel.OnGoBackPressed;
        _selectionPanel.SelectedToBuild += OnSelectionPanelSelectedToBuild;
    }

    public override void _ExitTree()
    {
        _entityPanel.AbilityViewOpened -= _selectionPanel.OnSelectableAbilityPressed;
        _entityPanel.AbilityViewClosed -= _selectionPanel.OnGoBackPressed;
        _selectionPanel.SelectedToBuild -= OnSelectionPanelSelectedToBuild;
        base._ExitTree();
    }

    public void SetMapSize(Vector2 mapSize)
    {
        _hoveringPanel.SetMapSize(mapSize);
    }

    public void OnEntityIsBeingPlaced(EntityNode entity)
    {
        _selectionPanel.OnEntityIsBeingPlaced(entity);
        _informationalText.Enable(entity is StructureNode
            ? InformationalText.InfoTextType.PlacingRotatable 
            : InformationalText.InfoTextType.Placing);
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
        _informationalText.Enable(entity is StructureNode 
            ? InformationalText.InfoTextType.Selected 
            : InformationalText.InfoTextType.SelectedMovement);
    }

    internal void OnEntityDeselected()
    {
        _entityPanel.OnEntityDeselected();
        _informationalText.Disable();
    }

    internal void OnMapNewTileHovered(Vector2 tileHovered, Terrain terrain, IList<EntityNode> occupants)
    {
        _hoveringPanel.OnMapNewTileHovered(tileHovered, terrain, occupants);
    }

    private void OnSelectionPanelSelectedToBuild(BuildNode buildAbility, EntityId entityId)
    {
        SelectedToBuild(buildAbility, entityId);
    }
}
