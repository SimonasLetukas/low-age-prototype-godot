using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities;
using low_age_prototype_common;

public class SelectionPanel : Control
{
    public event Action<BuildNode, EntityId> SelectedToBuild = delegate { };
    
    private NinePatchRect _background;
    private Tween _tween;
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
        _tween = GetNode<Tween>(nameof(Tween));
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
        _text.BbcodeText = text;
        
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
        _tween.InterpolateProperty(this, "margin_top", MarginTop, 
            0, PanelMoveDuration, Tween.TransitionType.Quad);
        _tween.Start();
    }

    private void ShowPanel()
    {
        var additionalRows = (int)Mathf.Ceil((float)_selectionAmount / Columns) - 1;
        RectSize = new Vector2(RectSize.x, RectSize.y + additionalRows * NewLineHeight);
        _background.RectSize = new Vector2(_background.RectSize.x,
            _background.RectSize.y + additionalRows * (NewLineHeight / _background.RectScale.y));
        var newMarginTop = RectSize.y * -1;
        Visible = true;
        _tween.InterpolateProperty(this, "margin_top", MarginTop, newMarginTop, 
            PanelMoveDuration, Tween.TransitionType.Quad);
        _tween.Start();
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
        
        RectSize = new Vector2(RectSize.x, OneLineSize);
        MarginTop = 0;
        _background.RectSize = new Vector2(_background.RectSize.x, OneLineSize / _background.RectScale.y);
    }
}
