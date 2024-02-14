using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;

public class EntityPanel : Control
{
    public event Action<AbilityButton> AbilityViewOpened = delegate { };
    public event Action AbilityViewClosed = delegate { };
    
    private EntityName _entityName;
    private AbilityButtons _abilityButtons;
    private InfoDisplay _display;
    private RichTextLabel _abilityTextBox;
    private Tween _tween;
    private bool _isShowingAbility;
    private bool _isSwitchingBetweenAbilities;
    private EntityNode _selectedEntity;
    private int _biggestPreviousAbilityTextSize = 0;
    private const int YSizeForAbility = 834;
    private const int YSizeForUnit = 758;
    private const int YSizeForStructure = 816;
    private const int YSizeForHiding = 1500;
    private const float PanelMoveDuration = 0.1f;
    private readonly IList<Ability> _abilitiesBlueprint = Data.Instance.Blueprint.Abilities;

    public override void _Ready()
    {
        _entityName = GetNode<EntityName>($"{nameof(EntityName)}");
        _abilityButtons = GetNode<AbilityButtons>($"{nameof(Panel)}/{nameof(AbilityButtons)}");
        _display = GetNode<InfoDisplay>($"{nameof(Panel)}/{nameof(InfoDisplay)}");
        _abilityTextBox = GetNode<RichTextLabel>($"{nameof(Panel)}/{nameof(InfoDisplay)}/{nameof(VBoxContainer)}/AbilityDescription/Text");
        _tween = GetNode<Tween>($"{nameof(Tween)}");

        _abilityButtons.Connect(nameof(AbilityButtons.AbilitiesPopulated), this, nameof(OnAbilityButtonsPopulated));
        _display.Connect(nameof(InfoDisplay.AbilitiesClosed), this, nameof(OnInfoDisplayAbilitiesClosed));
        _display.Connect(nameof(InfoDisplay.AbilityTextResized), this, nameof(OnInfoDisplayTextResized));
        
        HidePanel();
    }

    public void OnEntitySelected(EntityNode selectedEntity)
    {
        _isShowingAbility = false;
        _isSwitchingBetweenAbilities = false;
        _selectedEntity = selectedEntity;

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

    private void ChangeDisplay(AbilityButton clickedAbility = null)
    {
        if (_selectedEntity is null)
        {
            HidePanel();
            return;
        }
        
        if (_isShowingAbility && clickedAbility != null)
        {
            var abilityBlueprint = _abilitiesBlueprint.First(x => x.Id.Equals(clickedAbility.Ability.Id));
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
        
        var currentStats = selectedActor.CurrentStats;
        var health = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.Health));
        var shields = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.Shields));
        var movement = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.Movement));
        var initiative = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.Initiative));
        var meleeArmour = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.MeleeArmour));
        var rangedArmour = currentStats.FirstOrDefault(x =>
            x.Blueprint is CombatStat combatStat
            && combatStat.CombatType.Equals(StatType.RangedArmour));
        var actorAttributes = selectedActor.Attributes;
        
        // TODO this should show current values and update whenever values change dynamically while the display is open
        _display.SetEntityStats(
            health is null ? 0 : (int)health.CurrentValue, 
            health is null ? 0 : health.Blueprint.MaxAmount, 
            movement?.CurrentValue ?? 0, 
            movement is null ? 0 : movement.Blueprint.MaxAmount, 
            initiative is null ? 0 : (int)initiative.CurrentValue, 
            meleeArmour is null ? 0 : (int)meleeArmour.CurrentValue, 
            rangedArmour is null ? 0 : (int)rangedArmour.CurrentValue, 
            actorAttributes, 
            shields is null ? 0 : (int)shields.CurrentValue, 
            shields is null ? 0 : shields.Blueprint.MaxAmount);

        if (selectedActor is UnitNode selectedUnit)
        {
            _display.ResetAttacks();
            foreach (var blueprint in selectedUnit.CurrentStats
                         .Where(x => x.Blueprint is AttackStat)
                         .Select(attack => (AttackStat)attack.Blueprint))
            {
                if (blueprint.AttackType.Equals(Attacks.Melee)) 
                    _display.SetMeleeAttackStats(true, selectedActor.Name, blueprint.MaximumDistance, 
                        blueprint.MaxAmount, blueprint.BonusAmount, blueprint.BonusTo);
                
                if (blueprint.AttackType.Equals(Attacks.Ranged)) 
                    _display.SetRangedAttackStats(true, selectedActor.Name, blueprint.MaximumDistance, 
                        blueprint.MaxAmount, blueprint.BonusAmount, blueprint.BonusTo);
            }
            
            _display.ShowView(View.UnitStats);
            return;
        }

        if (selectedActor is StructureNode selectedStructure)
        {
            // TODO
            return;
        }
    }

    private void MovePanel()
    {
        Vector2 newSize;
        if (_isShowingAbility)
        {
            var abilityTextBoxSizeY = CalculateBiggestPreviousSize((int)_abilityTextBox.RectSize.y);
            
            var newY = YSizeForAbility - abilityTextBoxSizeY;
            if (newY > YSizeForUnit) // TODO change check here if structure is selected
                newY = YSizeForUnit;

            newSize = new Vector2(RectSize.x, newY);
        }
        else // TODO change for structures and other selectables
        {
            newSize = new Vector2(RectSize.x, YSizeForUnit);
        }

        _tween.InterpolateProperty(this, "rect_size", RectSize, newSize, 
            PanelMoveDuration, Tween.TransitionType.Quad);
        _tween.Start();
    }
    
    private void HidePanel()
    {
        _tween.InterpolateProperty(this, "rect_size", RectSize, 
            new Vector2(RectSize.x, YSizeForHiding), PanelMoveDuration, Tween.TransitionType.Quad);
        _tween.Start();
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
        ChangeDisplay(null);
        MovePanel();

        AbilityViewClosed();
    }

    private void OnInfoDisplayTextResized()
    {
        MovePanel();
    }
}
