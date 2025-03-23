using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;

public partial class EntityPanel : Control
{
    public event Action<AbilityButton> AbilityViewOpened = delegate { };
    public event Action AbilityViewClosed = delegate { };
    public event Action<bool, AttackType?> AttackSelected = delegate { };

    private GridContainer _behaviours = null!;
    private EntityName _entityName = null!;
    private AbilityButtons _abilityButtons = null!;
    private InfoDisplay _display = null!;
    private Text _abilityTextBox = null!;
    private bool _isShowingAbility;
    private bool _isSwitchingBetweenAbilities;
    private EntityNode? _selectedEntity;
    private int _biggestPreviousAbilityTextSize = 0;
    private const int YSizeForAbility = 978;
    private const int YSizeForUnit = 738;
    private const int YSizeForStructure = 796;
    private const int YSizeForHiding = 1500;
    private const float PanelMoveDuration = 0.1f;
    private readonly IList<Ability> _abilitiesBlueprint = Data.Instance.Blueprint.Abilities;

    public override void _Ready()
    {
        _behaviours = GetNode<GridContainer>($"Behaviours");
        _entityName = GetNode<EntityName>($"{nameof(EntityName)}");
        _abilityButtons = GetNode<AbilityButtons>($"{nameof(Panel)}/{nameof(AbilityButtons)}");
        _display = GetNode<InfoDisplay>($"{nameof(Panel)}/{nameof(InfoDisplay)}");
        _abilityTextBox = GetNode<Text>($"{nameof(Panel)}/{nameof(InfoDisplay)}/{nameof(VBoxContainer)}/AbilityDescription/{nameof(Text)}");

        _abilityButtons.AbilitiesPopulated += OnAbilityButtonsPopulated;
        _display.AbilitiesClosed += OnInfoDisplayAbilitiesClosed;
        _display.AbilityTextResized += OnInfoDisplayTextResized;
        _display.AttackSelected += OnInfoDisplayAttackSelected;
        
        HidePanel();
    }

    public override void _ExitTree()
    {
        _abilityButtons.AbilitiesPopulated -= OnAbilityButtonsPopulated;
        _display.AbilitiesClosed -= OnInfoDisplayAbilitiesClosed;
        _display.AbilityTextResized -= OnInfoDisplayTextResized;
        _display.AttackSelected -= OnInfoDisplayAttackSelected;
        
        base._ExitTree();
    }

    public void OnEntitySelected(EntityNode selectedEntity)
    {
        _isShowingAbility = false;
        _isSwitchingBetweenAbilities = false;
        _selectedEntity = selectedEntity;
        
        RemoveAllBehaviours();
        AddBehaviours(_selectedEntity);

        DisconnectAbilityButtons();
        _abilityButtons.Reset();
        if (selectedEntity is ActorNode selectedActor)
        {
            _abilityButtons.Populate(selectedActor.Abilities); 
        }
        
        _entityName.SetValue(selectedEntity.DisplayName);
        ChangeDisplay();
        MovePanel();
        AbilityViewClosed();
    }

    public void OnEntityDeselected()
    {
        _selectedEntity = null;
        HidePanel();
        AbilityViewClosed();
    }

    private void ChangeDisplay(AbilityButton? clickedAbility = null)
    {
        if (_selectedEntity is null)
        {
            HidePanel();
            return;
        }
        
        if (_isShowingAbility && clickedAbility != null)
        {
            var abilityBlueprint = _abilitiesBlueprint.First(x => x.Id.Equals(clickedAbility.Ability.Id));
            // TODO should probably get all of these properties from AbilityNode instead of all blueprints 
            var abilityInstance = clickedAbility.Ability;
            var name = abilityBlueprint.DisplayName;
            var turnPhase = abilityBlueprint.TurnPhase;
            var text = abilityBlueprint.Description;
            var research = abilityBlueprint.ResearchNeeded;
            var cooldown = abilityInstance.RemainingCooldown;
            _display.SetAbilityStats(name, turnPhase, text, cooldown, research);
            _display.ShowView(View.Ability);
            return;
        }
        
        _biggestPreviousAbilityTextSize = 0;
        
        // if (_selectedEntity is DoodadNode selectedDoodad) TODO

        var selectedActor = (ActorNode)_selectedEntity;
        
        // TODO this should show current values and update whenever values change dynamically while the display is open
        _display.SetEntityStats(_selectedEntity);

        if (selectedActor is UnitNode selectedUnit)
        {
            _display.ResetAttacks();
            foreach (var attack in selectedUnit.Attacks)
            {
                if (attack.IsMelee) 
                    _display.SetMeleeAttackStats(true, "Melee Attack", attack.MaximumDistance,
                        attack.Damage, attack.BonusDamage, attack.BonusTo);
                
                if (attack.IsRanged) 
                    _display.SetRangedAttackStats(true, "Ranged Attack", attack.MaximumDistance, 
                        attack.Damage, attack.BonusDamage, attack.BonusTo);
            }
            
            _display.ShowView(View.UnitStats);
            return;
        }

        if (selectedActor is StructureNode selectedStructure)
        {
            _display.ShowView(View.StructureStats);
            return;
        }
    }

    private void MovePanel()
    {
        Vector2 newSize;
        if (_isShowingAbility)
        {
            var abilityTextBoxSizeY = CalculateBiggestPreviousSize((int)_abilityTextBox.Size.Y);
            
            var newY = YSizeForAbility - abilityTextBoxSizeY;
            if (newY > YSizeForUnit) // TODO change check here if structure is selected
                newY = YSizeForUnit;

            newSize = new Vector2(Size.X, newY);
        }
        else // TODO change for structures and other selectables
        {
            newSize = new Vector2(Size.X, YSizeForUnit);
        }
        
        var tween = CreateTween();
        tween.TweenProperty(this, "size", newSize, PanelMoveDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad);
    }
    
    private void HidePanel()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "size", new Vector2(Size.X, YSizeForHiding), PanelMoveDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad);
    }

    private int CalculateBiggestPreviousSize(int currentSizeY = 0)
    {
        if (_isSwitchingBetweenAbilities)
        {
            if (currentSizeY <= _biggestPreviousAbilityTextSize) 
                return _biggestPreviousAbilityTextSize;
            
            _biggestPreviousAbilityTextSize = currentSizeY;
            return _biggestPreviousAbilityTextSize;
        }

        _biggestPreviousAbilityTextSize = currentSizeY;
        return currentSizeY;
    }
    
    private void ConnectAbilityButtons()
    {
        foreach (var abilityButton in _abilityButtons.GetChildren().OfType<AbilityButton>())
        {
            abilityButton.Clicked += OnAbilityButtonClicked;
            //abilityButton.Hovering += OnAbilityButtonHovering; TODO find out the best UX for handling all types of abilities 
        }
    }

    private void DisconnectAbilityButtons()
    {
        foreach (var abilityButton in _abilityButtons.GetChildren().OfType<AbilityButton>())
        {
            abilityButton.Clicked -= OnAbilityButtonClicked;
            //abilityButton.Hovering -= OnAbilityButtonHovering; TODO find out the best UX for handling all types of abilities 
        }
    }

    private void RemoveAllBehaviours()
    {
        foreach (var behaviour in _behaviours.GetChildren())
        {
            behaviour.QueueFree();
        }
    }

    private void AddBehaviours(EntityNode entity)
    {
        foreach (var behaviour in entity.Behaviours.GetAll())
        {
            BehaviourBox.InstantiateAsChild(behaviour, _behaviours);
        }
    }

    private void OnAbilityButtonClicked(AbilityButton abilityButton)
    {
        _isSwitchingBetweenAbilities = false;

        if (abilityButton.IsSelected)
        {
            abilityButton.SetSelected(false);
            _isShowingAbility = false;
            ChangeDisplay();
            MovePanel();
            AbilityViewClosed();
            return;
        }

        if (_abilityButtons.IsAnySelected()) 
            _isSwitchingBetweenAbilities = true;
        
        _abilityButtons.DeselectAll();
        abilityButton.SetSelected(true);
        _isShowingAbility = true;
        ChangeDisplay(abilityButton);
        MovePanel();
        AbilityViewOpened(abilityButton);
    }

    // TODO find out the best UX for handling all types of abilities 
    private void OnAbilityButtonHovering(bool started, AbilityButton abilityButton)
    {
        if (abilityButton.IsSelected)
            return;
        
        if (started)
        {
            _isSwitchingBetweenAbilities = true;
            _isShowingAbility = true;
            ChangeDisplay(abilityButton);
            MovePanel();
            return;
        }

        var wasAnyAbilitySelected = _abilityButtons.IsAnySelected();
        _isSwitchingBetweenAbilities = wasAnyAbilitySelected;
        _isShowingAbility = wasAnyAbilitySelected;

        if (wasAnyAbilitySelected is false && _display.CurrentView != View.Ability)
            return;
        
        _display.ShowPreviousView();
    }

    private void OnAbilityButtonsPopulated()
    {
        ConnectAbilityButtons();
    }

    private void OnInfoDisplayAbilitiesClosed()
    {
        _isSwitchingBetweenAbilities = false;
        
        _abilityButtons.DeselectAll();
        _isShowingAbility = false;
        ChangeDisplay();
        MovePanel();

        AbilityViewClosed();
    }

    private void OnInfoDisplayTextResized()
    {
        MovePanel();
    }

    private void OnInfoDisplayAttackSelected(bool started, AttackType? attackType) => AttackSelected(started, attackType);
}
