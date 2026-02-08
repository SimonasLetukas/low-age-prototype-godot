using System;
using Godot;
using System.Linq;
using LowAgeData.Domain.Common;

public partial class EntityPanel : Control
{
    public event Action<EntityNode> CandidatePlacementCancelled = delegate { };
    public event Action<AbilityButton> AbilityViewOpened = delegate { };
    public event Action AbilityViewClosed = delegate { };
    public event Action<bool, AttackType?> AttackSelected = delegate { };

    private AvailableActionsDisplay _availableActions = null!;
    private GridContainer _behaviours = null!;
    private EntityName _entityName = null!;
    private CancelButton _cancelButton = null!;
    private AbilityButtons _abilityButtons = null!;
    private InfoDisplay _display = null!;
    private Text _abilityTextBox = null!;
    private bool _isShowingAbility;
    private IAbilityNode? _selectedAbility;
    private bool _isSwitchingBetweenAbilities;
    private EntityNode? _selectedEntity;
    private int _biggestPreviousAbilityTextSize = 0;
    private const int YSizeForAbility = 978;
    private const int YSizeForUnit = 738;
    private const int YSizeForStructure = 796;
    private const int YSizeForHiding = 1500;
    private const float PanelMoveDuration = 0.1f;

    public override void _Ready()
    {
        _availableActions = GetNode<AvailableActionsDisplay>($"{nameof(AvailableActionsDisplay)}");
        _behaviours = GetNode<GridContainer>($"Behaviours");
        _entityName = GetNode<EntityName>($"{nameof(EntityName)}");
        _cancelButton = GetNode<CancelButton>($"{nameof(CancelButton)}");
        _abilityButtons = GetNode<AbilityButtons>($"{nameof(Panel)}/{nameof(AbilityButtons)}");
        _display = GetNode<InfoDisplay>($"{nameof(Panel)}/{nameof(InfoDisplay)}");
        _abilityTextBox = GetNode<Text>($"{nameof(Panel)}/{nameof(InfoDisplay)}/{nameof(VBoxContainer)}/AbilityDescription/{nameof(Text)}");

        _cancelButton.Clicked += OnCancelButtonClicked;
        _abilityButtons.AbilitiesPopulated += OnAbilityButtonsPopulated;
        _display.AbilitiesClosed += OnInfoDisplayAbilitiesClosed;
        _display.AbilityTextResized += OnInfoDisplayTextResized;
        _display.AttackSelected += OnInfoDisplayAttackSelected;
        _display.AbilityCancelled += OnInfoDisplayAbilityCancelled;
        
        HidePanel();
    }

    public override void _ExitTree()
    {
        _cancelButton.Clicked -= OnCancelButtonClicked;
        _abilityButtons.AbilitiesPopulated -= OnAbilityButtonsPopulated;
        _display.AbilitiesClosed -= OnInfoDisplayAbilitiesClosed;
        _display.AbilityTextResized -= OnInfoDisplayTextResized;
        _display.AttackSelected -= OnInfoDisplayAttackSelected;
        _display.AbilityCancelled -= OnInfoDisplayAbilityCancelled;
        
        base._ExitTree();
    }

    public void OnEntitySelected(EntityNode selectedEntity)
    {
        UnregisterAbility();
        _isSwitchingBetweenAbilities = false;
        _selectedEntity = selectedEntity;
        
        RemoveAllBehaviours();
        AddBehaviours(_selectedEntity);

        _cancelButton.Visible = _selectedEntity.IsCandidate();

        DisconnectAbilityButtons();
        _abilityButtons.Reset();
        _availableActions.Reset();
        if (selectedEntity is ActorNode selectedActor)
        {
            _abilityButtons.Populate(selectedActor.Abilities); 
            _availableActions.Populate(selectedActor.ActionEconomy);
        }
        
        _entityName.SetValue(selectedEntity.DisplayName);
        ChangeDisplay();
        MovePanel();
        AbilityViewClosed();
    }

    public void OnEntityDeselected()
    {
        _selectedEntity = null;
        UnregisterAbility();
        HidePanel();
        AbilityViewClosed();
    }

    private void ChangeDisplay()
    {
        if (_selectedEntity is null)
        {
            HidePanel();
            return;
        }
        
        if (_isShowingAbility && _selectedAbility != null)
        {
            var name = _selectedAbility.DisplayName;
            var turnPhase = _selectedAbility.TurnPhase;
            var text = _selectedAbility.Description;
            var research = _selectedAbility.ResearchNeeded;
            var cooldown = _selectedAbility.RemainingCooldown;
            var hasAbilityToCancel = _selectedAbility is IActiveAbilityNode { IsActivated: true };
            _display.SetAbilityStats(name, turnPhase, text, cooldown, research, hasAbilityToCancel);
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
                    _display.SetMeleeAttackStats(true, attack.DisplayName, attack.MaximumDistance,
                        attack.Damage, attack.BonusDamage, attack.BonusTo);
                
                if (attack.IsRanged) 
                    _display.SetRangedAttackStats(true, attack.DisplayName, attack.MaximumDistance, 
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

    private void RegisterAbility(IAbilityNode ability)
    {
        _isShowingAbility = true;
        _selectedAbility = ability;
        
        if (_selectedAbility is IActiveAbilityNode activeAbility)
            activeAbility.Cancelled += OnAbilityCancelled;
    }

    private void UnregisterAbility()
    {
        if (_selectedAbility is IActiveAbilityNode activeAbility)
            activeAbility.Cancelled -= OnAbilityCancelled;
        
        _isShowingAbility = false;
        _selectedAbility = null;
    }

    private void OnAbilityCancelled(IActiveAbilityFocus focus) => ChangeDisplay();

    private void OnCancelButtonClicked()
    {
        if (_selectedEntity is null || _selectedEntity.IsCandidate() is false)
            return;

        CandidatePlacementCancelled(_selectedEntity);
    }

    private void OnAbilityButtonClicked(AbilityButton abilityButton)
    {
        _isSwitchingBetweenAbilities = false;

        if (abilityButton.IsSelected)
        {
            abilityButton.SetSelected(false);
            UnregisterAbility();
            ChangeDisplay();
            MovePanel();
            AbilityViewClosed();
            return;
        }

        if (_abilityButtons.IsAnySelected()) 
            _isSwitchingBetweenAbilities = true;
        
        _abilityButtons.DeselectAll();
        abilityButton.SetSelected(true);
        RegisterAbility(abilityButton.Ability);
        ChangeDisplay();
        MovePanel();
        AbilityViewOpened(abilityButton);
    }

    private void OnAbilityButtonsPopulated()
    {
        ConnectAbilityButtons();
    }

    private void OnInfoDisplayAbilitiesClosed()
    {
        _isSwitchingBetweenAbilities = false;
        
        _abilityButtons.DeselectAll();
        UnregisterAbility();
        ChangeDisplay();
        MovePanel();

        AbilityViewClosed();
    }

    private void OnInfoDisplayTextResized()
    {
        MovePanel();
    }

    private void OnInfoDisplayAttackSelected(bool started, AttackType? attackType) => AttackSelected(started, attackType);
    
    private void OnInfoDisplayAbilityCancelled()
    {
        if (_selectedAbility is not IActiveAbilityNode activeAbility)
            return;

        if (Players.Instance.IsActionAllowedForCurrentPlayerOn(activeAbility.OwnerActor) is false)
            return;
        
        var currentPhase = GlobalRegistry.Instance.GetCurrentPhase();
        if (currentPhase.Equals(activeAbility.TurnPhase) is false)
            return;
        
        activeAbility.CancelActivations();
    }
}
