using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Entities;
using low_age_prototype_common;

public partial class SelectionPanel : Control
{
    public event Action<BuildNode, EntityId> SelectedToBuild = delegate { };
    
    private NinePatchRect _background;
    private GridContainer _gridContainer;
    private RichTextLabel _text;
    private int _selectionAmount = 0;
    private const int OneLineSize = 100;
    private const int NewLineHeight = 44;
    private const int Columns = 4;
    private const float PanelMoveDuration = 0.05f;
    
    public override void _Ready()
    {
        base._Ready();

        _background = GetNode<NinePatchRect>("Background");
        _gridContainer = GetNode<GridContainer>(nameof(GridContainer));
        _gridContainer.Columns = Columns;
        _text = GetNode<RichTextLabel>(nameof(RichTextLabel));
        
        Reset();
        HidePanel();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    public void OnSelectableAbilityPressed(AbilityButton abilityButton)
    {
        if (abilityButton.Ability is ISelectable selectableAbility is false)
            return;
        
        Reset();
        Populate(selectableAbility);
        ShowPanel();
    }

    public void OnGoBackPressed()
    {
        HidePanel();
    }

    public void OnEntityIsBeingPlaced(EntityNode entity)
    {
        var buildableBehaviours = entity.Behaviours.GetBuildables();
        var text = buildableBehaviours.Aggregate(string.Empty, (current, behaviour) => 
            current + behaviour.Description + "\n\n");
        _text.Text = text;
        
        _text.Visible = true;
        _gridContainer.Visible = false;
    }

    private void Populate(ISelectable ability)
    {
        _selectionAmount = 0;
        var selectionIds = new List<Id>();
        Action<ISelectable, Id> selectionButtonOnClicked = null;
        switch (ability)
        {
            case BuildNode build:
                selectionIds = build.Selection.Select(x => x.Name).OfType<Id>().ToList();
                _selectionAmount = build.Selection.Count;
                selectionButtonOnClicked = OnBuildSelectionItemPressed;
                break;
            default:
                break;
        }

        foreach (var selectionId in selectionIds)
        {
            var selectionButton = SelectionButton.InstantiateAsChild(ability, selectionId, _gridContainer);
            selectionButton.Clicked += selectionButtonOnClicked;
        }
    }

    private void OnBuildSelectionItemPressed(ISelectable abilityNode, Id id)
    {
        SelectedToBuild((BuildNode)abilityNode, (EntityId)id);
    }

    private void HidePanel()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "offset_top", 0, PanelMoveDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad);
    }

    private void ShowPanel()
    {
        var additionalRows = (int)Mathf.Ceil((float)_selectionAmount / Columns) - 1;
        Size = new Vector2(Size.X, Size.Y + additionalRows * NewLineHeight);
        _background.Size = new Vector2(_background.Size.X,
            _background.Size.Y + additionalRows * (NewLineHeight / _background.Scale.Y));
        var newMarginTop = Size.Y * -1;
        Visible = true;
        
        var tween = CreateTween();
        tween.TweenProperty(this, "offset_top", newMarginTop, PanelMoveDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad);
    }

    private void Reset()
    {
        Visible = false;
        
        foreach (var child in _gridContainer.GetChildren().OfType<SelectionButton>())
        {
            child.Clicked -= OnBuildSelectionItemPressed;
            child.QueueFree();
        }

        _text.Visible = false;
        _gridContainer.Visible = true;
        
        Size = new Vector2(Size.X, OneLineSize);
        OffsetTop = 0;
        _background.Size = new Vector2(_background.Size.X, OneLineSize / _background.Scale.Y);
    }
}
