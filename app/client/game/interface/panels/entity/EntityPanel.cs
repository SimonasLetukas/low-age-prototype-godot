using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;

public class EntityPanel : Control
{
    private Abilities _abilityButtons;
    private InfoDisplay _display;
    private RichTextLabel _abilityTextBox;
    private Tween _tween;
    private bool _isAbilitySelected;
    private bool _isSwitchingBetweenAbilities;
    private int _biggestPreviousAbilityTextSize = 0;
    private int _ySizeForAbility = 834;
    private int _ySizeForUnit = 786;
    private int _ySizeForStructure = 816;
    private readonly Dictionary<string, string> _abilityDescriptionsById = new Dictionary<string, string>();

    public override void _Ready()
    {
        _abilityButtons = GetNode<Abilities>($"{nameof(Panel)}/{nameof(Abilities)}");
        _display = GetNode<InfoDisplay>($"{nameof(Panel)}/{nameof(InfoDisplay)}");
        _abilityTextBox = GetNode<RichTextLabel>($"{nameof(Panel)}/{nameof(InfoDisplay)}/{nameof(VBoxContainer)}/AbilityDescription/Text");
        _tween = GetNode<Tween>($"{nameof(Tween)}");

        _abilityButtons.Connect(nameof(Abilities.AbilitiesPopulated), this, nameof(OnAbilityButtonsPopulated));
        _display.Connect(nameof(InfoDisplay.AbilitiesClosed), this, nameof(OnInfoDisplayAbilitiesClosed));
        _display.Connect(nameof(InfoDisplay.AbilityTextResized), this, nameof(OnInfoDisplayTextResized));
        
        // TODO
        
        var mockedId1 = "slave_build";
        var mockedDescription1 = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action";
        var mockedId2 = "slave_manual_labour";
        var mockedDescription2 = "Select an adjacent Hut. At the start of the next planning phase receive +2 Scraps. Maximum of one Slave per Hut.";

        _abilityDescriptionsById[mockedId1] = mockedDescription1;
        _abilityDescriptionsById[mockedId2] = mockedDescription2;
        
        _abilityButtons.Populate(_abilityDescriptionsById.Keys);
    }

    private void ConnectAbilityButtons()
    {
        foreach (var abilityButton in _abilityButtons.GetChildren().OfType<AbilityButton>())
        {
            var id = abilityButton.Id; // TODO this ID should be used later
            abilityButton.Connect(nameof(AbilityButton.Clicked), this, nameof(OnAbilityButtonClicked));
        }
    }

    private void ChangeDisplay(AbilityButton clickedAbility)
    {
        if (_isAbilitySelected)
        {
            var id = clickedAbility.Id;
            var type = TurnPhase.Planning;
            var text = _abilityDescriptionsById[id];
            var research = id;
            var cooldown = 1;
            var cooldownType = TurnPhase.Action;
            _display.SetAbilityStats(id, type, text, research, cooldown, cooldownType);
            _display.ShowView(View.Ability);
        }
        else
        {
            // TODO set parameters here
            _display.ShowView(View.UnitStats);
            _biggestPreviousAbilityTextSize = 0;
        }
    }

    private void MovePanel()
    {
        Vector2 newSize;
        if (_isAbilitySelected)
        {
            var abilityTextBoxSizeY = CalculateBiggestPreviousSize((int)_abilityTextBox.RectSize.y);
            
            var newY = _ySizeForAbility - abilityTextBoxSizeY;
            if (newY > _ySizeForUnit) // TODO check here if structure is selected
                newY = _ySizeForUnit;

            newSize = new Vector2(RectSize.x, newY);
        }
        else
        {
            newSize = new Vector2(RectSize.x, _ySizeForUnit);
        }

        _tween.InterpolateProperty(this, "rect_size", RectSize, newSize, 0.1f, Tween.TransitionType.Quad);
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

    private void OnAbilityButtonClicked(AbilityButton abilityButton)
    {
        _isSwitchingBetweenAbilities = false;

        if (abilityButton.IsSelected)
        {
            abilityButton.SetSelected(false);
            _isAbilitySelected = false;
            ChangeDisplay(null);
            MovePanel();
            return;
        }

        if (_abilityButtons.IsAnySelected()) 
            _isSwitchingBetweenAbilities = true;
        
        _abilityButtons.DeselectAll();
        abilityButton.SetSelected(true);
        _isAbilitySelected = true;
        ChangeDisplay(abilityButton);
        MovePanel();
    }

    private void OnAbilityButtonsPopulated()
    {
        ConnectAbilityButtons();
    }

    private void OnInfoDisplayAbilitiesClosed()
    {
        _isSwitchingBetweenAbilities = false;
        
        _abilityButtons.DeselectAll();
        _isAbilitySelected = false;
        ChangeDisplay(null);
        MovePanel();
    }

    private void OnInfoDisplayTextResized()
    {
        MovePanel();
    }
}
