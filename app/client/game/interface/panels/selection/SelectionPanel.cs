using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeData.Domain.Researches;

public partial class SelectionPanel : Control
{
    public event Action<BuildNode, EntityId> SelectedToBuild = delegate { };
    public event Action<ResearchNode, ResearchId> SelectedToResearch = delegate { };

    private NinePatchRect _background = null!;
    private GridContainer _gridContainer = null!;
    private Text _text = null!;

    private IActiveAbilityNode? _ability;
    
    private const int MinimumYPaddingButtons = 96 - 40;
    private const int MinimumYPaddingText = 96 - 46;
    private const int Columns = 4;
    private const float PanelMoveDuration = 0.05f;
    
    public override void _Ready()
    {
        base._Ready();

        _background = GetNode<NinePatchRect>("Background");
        _gridContainer = GetNode<GridContainer>(nameof(GridContainer));
        _gridContainer.Columns = Columns;
        _text = GetNode<Text>(nameof(Text));
        
        Reset();
        HidePanel();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    public void OnSelectableAbilityPressed(AbilityButton abilityButton)
    {
        if (abilityButton.Ability is IAbilityHasSelection selectableAbility is false 
            || abilityButton.Ability is IActiveAbilityNode activeAbility is false)
            return;
        
        Reset();

        _ability = activeAbility;
        _ability.ActivationsCancelled += OnAbilityActivationsCancelled;
        
        var playerCanSeeProgress = abilityButton.Ability.OwnerActor.Player.Equals(Players.Instance.Current);
        if (playerCanSeeProgress && abilityButton.Ability is ResearchNode { IsActivated: true } researchNode)
            ShowResearchProgressText(researchNode);
        else
            PopulateSelection(selectableAbility);

        Callable.From(ShowPanel).CallDeferred();
    }

    private void OnAbilityActivationsCancelled(IActiveAbilityNode ability)
    {
        if (ability.Equals(_ability) is false)
            return;

        if (ability is not ResearchNode research)
            return;
        
        Reset();
        
        _ability = ability;
        _ability.ActivationsCancelled += OnAbilityActivationsCancelled;
        
        PopulateSelection(research);
        
        Callable.From(ShowPanel).CallDeferred();
    }

    public void OnGoBackPressed()
    {
        HidePanel();
    }

    public async void OnEntityIsBeingPlaced(EntityNode entity)
    {
        Reset();
        
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        var buildableBehaviours = entity.Behaviours.GetBuildables();
        var text = buildableBehaviours.Aggregate(string.Empty, (current, behaviour) => 
            current + behaviour.Description + "\n\n");
        _text.Text = text;
        
        _text.Visible = true;
        _gridContainer.Visible = false;
        
        Callable.From(ShowPanel).CallDeferred();
    }

    private void ShowResearchProgressText(ResearchNode researchNode)
    {
        _text.Text = researchNode.GetResearchProgressText();
        
        _text.Visible = true;
        _gridContainer.Visible = false;
    }

    private void PopulateSelection(IAbilityHasSelection ability)
    {
        var selectionIds = new List<Id>();
        Action<IAbilityHasSelection, Id> selectionButtonOnClicked = delegate { };
        switch (ability)
        {
            case BuildNode build:
                selectionIds = build.Selection.Select(x => x.Name).OfType<Id>().ToList();
                selectionButtonOnClicked = OnBuildSelectionItemPressed;
                break;
            case ResearchNode research:
                selectionIds = research.Selection.Select(x => x.Name).OfType<Id>().ToList();
                selectionButtonOnClicked = OnResearchSelectionItemPressed;
                break;
        }

        foreach (var selectionId in selectionIds)
        {
            var selectionButton = SelectionButton.InstantiateAsChild(ability, selectionId, _gridContainer);
            selectionButton.Clicked += selectionButtonOnClicked;
        }

        _text.Visible = false;
        _gridContainer.Visible = true;
    }

    private void OnBuildSelectionItemPressed(IAbilityHasSelection abilityNode, Id selectionId)
    {
        var buildAbility = (BuildNode)abilityNode;
        if (Players.Instance.IsActionAllowedForCurrentPlayerOn(buildAbility.OwnerActor) is false)
            return;
        
        SelectedToBuild(buildAbility, (EntityId)selectionId);
    }

    private void OnResearchSelectionItemPressed(IAbilityHasSelection abilityNode, Id selectionId)
    {
        var researchAbility = (ResearchNode)abilityNode;
        if (Players.Instance.IsActionAllowedForCurrentPlayerOn(researchAbility.OwnerActor) is false)
            return;

        SelectedToResearch(researchAbility, (ResearchId)selectionId);
    }

    private void HidePanel()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "offset_top", 0, PanelMoveDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad);
        
        tween.TweenCallback(Callable.From(() =>
        {
            Visible = false;
        }));
    }

    private async void ShowPanel()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        var height = Math.Max(
            _text.GetCombinedMinimumSize().Y + MinimumYPaddingText,
            _gridContainer.GetCombinedMinimumSize().Y + MinimumYPaddingButtons
        );
        Size = new Vector2(Size.X, height);
        _background.Size = new Vector2(_background.Size.X, height / _background.Scale.Y);
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
        
        if (_ability is not null)
            _ability.ActivationsCancelled -= OnAbilityActivationsCancelled;
        _ability = null;
        
        foreach (var child in _gridContainer.GetChildren().OfType<SelectionButton>())
        {
            switch (child.Ability)
            {
                case BuildNode:
                    child.Clicked -= OnBuildSelectionItemPressed;
                    break;
                case ResearchNode:
                    child.Clicked -= OnResearchSelectionItemPressed;
                    break;
            }

            child.GetParent()?.RemoveChild(child);
            child.QueueFree();
        }

        _text.Text = string.Empty;
        
        _text.Visible = false;
        _gridContainer.Visible = false;
        
        // This may not work correctly due to minimum Y sizes in the control nodes.
        Size = new Vector2(Size.X, 0); 
        OffsetTop = 0;
        _background.Size = new Vector2(_background.Size.X, Size.Y / _background.Scale.Y);
    }
}
