using Godot;
using System;

public class InfoDisplay : MarginContainer
{
    public enum View
    {
        UnitStats,
        StructureStats,
        AttackMelee,
        AttackRanged,
        Ability
    }
    
    [Signal] public delegate void AbilitiesClosed();
    [Signal] public delegate void AbilityTextResized();

    private View _currentView = View.UnitStats;
    private View _previousView = View.UnitStats;

    private int _valueCurrentHealth = 999;
    private int _valueMaxHealth = 999;
    private int _valueCurrentShields = 999;
    private int _valueMaxShields = 0;
    private float _valueCurrentMovement = 99.9f;
    private int _valueMaxMovement = 99;
    private int _valueInitiative = 99;
    private int _valueMeleeArmour = 99;
    private int _valueRangedArmour = 99;
    private Constants.Game.EntityType[] _valueEntityTypes =
    {
        Constants.Game.EntityType.Biological, Constants.Game.EntityType.Armoured, Constants.Game.EntityType.Ranged
    };
    
    private bool _hasMeleeAttack = true;
    private string _valueMeleeName = "Venom Fangs";
    private int _valueMeleeDistance = 1;
    private int _valueMeleeDamage = 999;
    private int _valueMeleeBonusDamage = 999;
    private Constants.Game.EntityType _valueMeleeBonusType = Constants.Game.EntityType.Biological;
    
    private bool _hasRangedAttack = true;
    private string _valueRangedName = "Monev Fangs";
    private int _valueRangedDistance = 999;
    private int _valueRangedDamage = 999;
    private int _valueRangedBonusDamage = 999;
    private Constants.Game.EntityType _valueRangedBonusType = Constants.Game.EntityType.Armoured;
    
    private string _valueAbilityName = "Build";
    private Constants.Game.AbilityType _valueAbilityType = Constants.Game.AbilityType.Planning;
    private string _valueAbilityText = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action.";
    private int _valueAbilityCooldown = 3;
    private Constants.Game.AbilityType _valueAbilityCooldownType = Constants.Game.AbilityType.Action;
    private string _valueResearchText = "Hardened Matrix";

    private Control _abilityTitle;
    private Research _researchText;
    private NavigationBox _navigationBack;
    private Control _leftSide;
    private Control _leftSideTop;
    private TopText _leftSideTopText;
    private StatBlock _leftSideTopHealth;
    private StatBlock _leftSideTopShields;
    private StatBlock _leftSideMiddleMovement;
    private StatBlock _leftSideMiddleInitiative;
    private StatBlock _leftSideMiddleDamage;
    private StatBlock _leftSideMiddleDistance;
    private StatBlock _leftSideBottomMeleeArmour;
    private StatBlock _leftSideBottomRangedArmour;
    private StatBlockText _leftSideBottomStatBlockText;
    private Control _rightSide;
    private AttackTypeBox _rightSideMeleeAttack;
    private AttackTypeBox _rightSideRangedAttack;
    private Control _abilityDescription;
    private EntityTypes _entityTypes;

    public override void _Ready()
    {
        _abilityTitle = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle");
        _researchText = GetNode<Research>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/{nameof(Research)}");
        _navigationBack = GetNode<NavigationBox>($"{nameof(VBoxContainer)}/TopPart/{nameof(NavigationBox)}");
        _leftSide = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/LeftSide");
        _leftSideTop = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Top");
        _leftSideTopText = GetNode<TopText>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/{nameof(TopText)}");
        _leftSideTopHealth = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Top/Health");
        _leftSideTopShields = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Top/Shields");
        _leftSideMiddleMovement = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Middle/Movement");
        _leftSideMiddleInitiative = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Middle/Initiative");
        _leftSideMiddleDamage = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Middle/Damage");
        _leftSideMiddleDistance = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Middle/Distance");
        _leftSideBottomMeleeArmour = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Bottom/MeleeArmour");
        _leftSideBottomRangedArmour = GetNode<StatBlock>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Bottom/RangedArmour");
        _leftSideBottomStatBlockText = GetNode<StatBlockText>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Bottom/{nameof(StatBlockText)}");
        _rightSide = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/RightSide");
        _rightSideMeleeAttack = GetNode<AttackTypeBox>($"{nameof(VBoxContainer)}/TopPart/RightSide/Attacks/Melee");
        _rightSideRangedAttack = GetNode<AttackTypeBox>($"{nameof(VBoxContainer)}/TopPart/RightSide/Attacks/Ranged");
        _abilityDescription = GetNode<Control>($"{nameof(VBoxContainer)}/AbilityDescription");
        _entityTypes = GetNode<EntityTypes>($"{nameof(VBoxContainer)}/{nameof(EntityTypes)}");

        _rightSideMeleeAttack.Connect(nameof(AttackTypeBox.Clicked), this, nameof(OnMeleeClicked));
        _rightSideRangedAttack.Connect(nameof(AttackTypeBox.Clicked), this, nameof(OnRangedClicked));
        _rightSideMeleeAttack.Connect(nameof(AttackTypeBox.Hovering), this, nameof(OnMeleeHovering));
        _rightSideRangedAttack.Connect(nameof(AttackTypeBox.Hovering), this, nameof(OnRangedHovering));
        _navigationBack.Connect(nameof(NavigationBox.Clicked), this, nameof(OnNavigationBoxClicked));
        _abilityDescription.GetNode<RichTextLabel>("Text").Connect("resized", this, nameof(OnTextResized));
        
        ShowView(View.UnitStats);
    }

    public void ShowView(View view)
    {
        _previousView = _currentView;
        _currentView = view;
        switch (_currentView)
        {
            case View.UnitStats:
            default:
                Reset();
                ShowUnitStats();
                break;
            case View.StructureStats:
                Reset();
                ShowStructureStats();
                break;
            case View.AttackMelee:
                Reset();
                ShowMeleeAttack();
                break;
            case View.AttackRanged:
                Reset();
                ShowRangedAttack();
                break;
            case View.Ability:
                Reset();
                ShowAbility();
                break;
        }
    }

    public void SetEntityStats(
        int currentHealth,
        int maxHealth,
        float currentMovement,
        int maxMovement,
        int initiative,
        int meleeArmour,
        int rangedArmour,
        Constants.Game.EntityType[] entityTypes,
        int currentShields = 0,
        int maxShields = 0)
    {
        _valueCurrentHealth = currentHealth;
        _valueMaxHealth = maxHealth;
        _valueCurrentMovement = currentMovement;
        _valueMaxMovement = maxMovement;
        _valueInitiative = initiative;
        _valueMeleeArmour = meleeArmour;
        _valueRangedArmour = rangedArmour;
        _valueEntityTypes = entityTypes;
        _valueCurrentShields = currentShields;
        _valueMaxShields = maxShields;
    }

    public void SetMeleeAttackStats(
        bool hasAttack,
        string attackName,
        int distance = 0,
        int damage = 0,
        int bonusDamage = 0,
        Constants.Game.EntityType bonusType = Constants.Game.EntityType.Armoured)
    {
        _hasMeleeAttack = hasAttack;
        _valueMeleeName = attackName;
        _valueMeleeDistance = distance;
        _valueMeleeDamage = damage;
        _valueMeleeBonusDamage = bonusDamage;
        _valueMeleeBonusType = bonusType;
    }
    
    public void SetRangedAttackStats(
        bool hasAttack,
        string attackName,
        int distance = 0,
        int damage = 0,
        int bonusDamage = 0,
        Constants.Game.EntityType bonusType = Constants.Game.EntityType.Armoured)
    {
        _hasRangedAttack = hasAttack;
        _valueRangedName = attackName;
        _valueRangedDistance = distance;
        _valueRangedDamage = damage;
        _valueRangedBonusDamage = bonusDamage;
        _valueRangedBonusType = bonusType;
    }

    public void SetAbilityStats(
        string abilityName,
        Constants.Game.AbilityType type,
        string text,
        string research = "",
        int cooldown = 0,
        Constants.Game.AbilityType cooldownType = Constants.Game.AbilityType.Action)
    {
        _valueAbilityName = abilityName;
        _valueAbilityType = type;
        _valueAbilityText = text;
        _valueResearchText = research;
        _valueAbilityCooldown = cooldown;
        _valueAbilityCooldownType = cooldownType;
    }

    private void Reset()
    {
        _abilityTitle.Visible = false;
        _researchText.Visible = false;
        _navigationBack.Visible = false;
        _leftSide.Visible = false;
        _leftSideTop.Visible = false;
        _leftSideTopText.Visible = false;
        _leftSideTopHealth.Visible = false;
        _leftSideTopShields.Visible = false;
        _leftSideMiddleMovement.Visible = false;
        _leftSideMiddleInitiative.Visible = false;
        _leftSideMiddleDamage.Visible = false;
        _leftSideMiddleDistance.Visible = false;
        _leftSideBottomMeleeArmour.Visible = false;
        _leftSideBottomRangedArmour.Visible = false;
        _leftSideBottomStatBlockText.Visible = false;
        _rightSide.Visible = false;
        _rightSideMeleeAttack.Visible = false;
        _rightSideRangedAttack.Visible = false;
        _abilityDescription.Visible = false;
        _entityTypes.Visible = false;
    }

    private void ShowUnitStats()
    {
        _leftSideTopHealth.SetValue(_valueMaxHealth, (_valueCurrentHealth == _valueMaxHealth) is false, _valueCurrentHealth);
        _leftSideTopShields.SetValue(_valueMaxShields, (_valueCurrentShields == _valueMaxShields) is false, _valueCurrentShields);
        _leftSideMiddleMovement.SetValue(_valueMaxMovement, (_valueCurrentMovement == _valueMaxMovement) is false, _valueCurrentMovement);
        _leftSideMiddleInitiative.SetValue(_valueInitiative);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _entityTypes.SetTypes(_valueEntityTypes);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = true;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideMiddleMovement.Visible = true;
        _leftSideMiddleInitiative.Visible = true;
        _leftSideBottomMeleeArmour.Visible = true;
        _leftSideBottomRangedArmour.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _entityTypes.Visible = true;
    }

    private void ShowStructureStats()
    {
        _leftSideTopHealth.SetValue(_valueMaxHealth, (_valueCurrentHealth == _valueMaxHealth) is false, _valueCurrentHealth);
        _leftSideTopShields.SetValue(_valueMaxShields, (_valueCurrentShields == _valueMaxShields) is false, _valueCurrentShields);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _entityTypes.SetTypes(_valueEntityTypes);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = true;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideBottomMeleeArmour.Visible = true;
        _leftSideBottomRangedArmour.Visible = true;
        _entityTypes.Visible = true;
    }

    private void ShowMeleeAttack()
    {
        _leftSideTopText.SetNameText(_valueMeleeName);
        _leftSideTopText.SetMelee();
        _leftSideMiddleDamage.SetValue(_valueMeleeDamage);
        _leftSideMiddleDistance.SetValue(_valueMeleeDistance);
        _leftSideBottomStatBlockText.SetValue(_valueMeleeBonusDamage);
        _leftSideBottomStatBlockText.SetText(_valueMeleeBonusType.ToString());
        _entityTypes.SetTypes(_valueEntityTypes);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _leftSideBottomStatBlockText.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _entityTypes.Visible = true;
    }

    private void ShowRangedAttack()
    {
        _leftSideTopText.SetNameText(_valueRangedName);
        _leftSideTopText.SetMelee();
        _leftSideMiddleDamage.SetValue(_valueRangedDamage);
        _leftSideMiddleDistance.SetValue(_valueRangedDistance);
        _leftSideBottomStatBlockText.SetValue(_valueRangedBonusDamage);
        _leftSideBottomStatBlockText.SetText(_valueRangedBonusType.ToString());
        _entityTypes.SetTypes(_valueEntityTypes);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _leftSideBottomStatBlockText.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _entityTypes.Visible = true;
    }

    private void ShowAbility()
    {
        GetNode<Label>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/Name/Label").Text = _valueAbilityName;
        GetNode<Label>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/Name/Label/Shadow").Text = _valueAbilityName;
        _researchText.SetResearch(_valueResearchText);
        GetNode<Type>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/{nameof(Type)}").SetType(_valueAbilityType, _valueAbilityCooldown, _valueAbilityCooldownType);
        _abilityDescription.GetNode<RichTextLabel>("Text").BbcodeText = _valueAbilityText;
        _abilityDescription.GetNode<RichTextLabel>("Text/Shadow").BbcodeText = _valueAbilityText;

        _abilityTitle.Visible = true;
        _researchText.Visible = _valueResearchText.Empty() is false;
        _navigationBack.Visible = true;
        _abilityDescription.Visible = true;
        
        _rightSideMeleeAttack.SetSelected(false);
        _rightSideRangedAttack.SetSelected(false);
    }

    private void OnMeleeClicked()
    {
        if (_rightSideMeleeAttack.IsSelected)
        {
            ShowView(View.UnitStats);
            return;
        }
        
        _rightSideRangedAttack.SetSelected(false);
        if (_currentView != View.AttackMelee)
        {
            ShowView(View.AttackMelee);
        }
    }

    private void OnRangedClicked()
    {
        if (_rightSideRangedAttack.IsSelected)
        {
            ShowView(View.UnitStats);
            return;
        }
        
        _rightSideMeleeAttack.SetSelected(false);
        if (_currentView != View.AttackRanged)
        {
            ShowView(View.AttackRanged);
        }
    }

    private void OnMeleeHovering(bool started)
    {
        switch (started)
        {
            case true:
                ShowView(View.AttackMelee);
                break;
            case false:
                ShowView(_previousView);
                break;
        }
    }

    private void OnRangedHovering(bool started)
    {
        switch (started)
        {
            case true:
                ShowView(View.AttackRanged);
                break;
            case false:
                ShowView(_previousView);
                break;
        }
    }

    private void OnNavigationBoxClicked()
    {
        ShowView(_previousView);
        EmitSignal(nameof(AbilitiesClosed));
    }

    private void OnTextResized()
    {
        EmitSignal(nameof(AbilityTextResized));
    }
}
