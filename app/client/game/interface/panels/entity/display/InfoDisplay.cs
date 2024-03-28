using Godot;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;

public class InfoDisplay : MarginContainer
{
    [Signal] public delegate void AbilitiesClosed();
    [Signal] public delegate void AbilityTextResized();

    public View CurrentView { get; private set; } = View.UnitStats;
    private View _previousView = View.UnitStats;
    private bool _showMinimal = false;

    private int _valueCurrentHealth = 999;
    private int _valueMaxHealth = 999;
    private int _valueCurrentShields = 999;
    private int _valueMaxShields = 0;
    private float _valueCurrentMovement = 99.9f;
    private int _valueMaxMovement = 99;
    private int _valueInitiative = 99;
    private int _valueMeleeArmour = 99;
    private int _valueRangedArmour = 99;
    private IEnumerable<ActorAttribute> _valueActorAttributes = new[]
    {
        ActorAttribute.Biological, ActorAttribute.Armoured, ActorAttribute.Ranged
    };
    
    private bool _hasMeleeAttack = true;
    private string _valueMeleeName = "Venom Fangs";
    private int _valueMeleeDistance = 1;
    private int _valueMeleeDamage = 999;
    private int _valueMeleeBonusDamage = 999;
    private ActorAttribute _valueMeleeBonusType = ActorAttribute.Biological;
    
    private bool _hasRangedAttack = true;
    private string _valueRangedName = "Monev Fangs";
    private int _valueRangedDistance = 999;
    private int _valueRangedDamage = 999;
    private int _valueRangedBonusDamage = 999;
    private ActorAttribute _valueRangedBonusType = ActorAttribute.Armoured;
    
    private string _valueAbilityName = "Build";
    private TurnPhase _valueAbilityTurnPhase = TurnPhase.Planning;
    private string _valueAbilityText = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action.";
    private EndsAtNode _valueAbilityCooldown = null;
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
    private StatBlockText _leftSideBottomEmptyBlock;
    private Control _rightSide;
    private AttackTypeBox _rightSideMeleeAttack;
    private AttackTypeBox _rightSideRangedAttack;
    private Control _abilityDescription;
    private ActorAttributes _actorAttributes;

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
        _leftSideBottomEmptyBlock = GetNode<StatBlockText>($"{nameof(VBoxContainer)}/TopPart/LeftSide/Rows/Bottom/EmptyRow");
        _rightSide = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/RightSide");
        _rightSideMeleeAttack = GetNode<AttackTypeBox>($"{nameof(VBoxContainer)}/TopPart/RightSide/Attacks/Melee");
        _rightSideRangedAttack = GetNode<AttackTypeBox>($"{nameof(VBoxContainer)}/TopPart/RightSide/Attacks/Ranged");
        _abilityDescription = GetNode<Control>($"{nameof(VBoxContainer)}/AbilityDescription");
        _actorAttributes = GetNode<ActorAttributes>($"{nameof(VBoxContainer)}/{nameof(ActorAttributes)}");

        _rightSideMeleeAttack.Connect(nameof(AttackTypeBox.Clicked), this, nameof(OnMeleeClicked));
        _rightSideRangedAttack.Connect(nameof(AttackTypeBox.Clicked), this, nameof(OnRangedClicked));
        _rightSideMeleeAttack.Connect(nameof(AttackTypeBox.Hovering), this, nameof(OnMeleeHovering));
        _rightSideRangedAttack.Connect(nameof(AttackTypeBox.Hovering), this, nameof(OnRangedHovering));
        _navigationBack.Connect(nameof(NavigationBox.Clicked), this, nameof(OnNavigationBoxClicked));
        _abilityDescription.GetNode<RichTextLabel>("Text").Connect("resized", this, nameof(OnTextResized));
        
        _leftSideBottomEmptyBlock.SetEmpty();
        ShowView(View.UnitStats);
    }

    public void ShowPreviousView()
    {
        CurrentView = _previousView;
        ShowView(CurrentView);
    }

    public void ShowView(View view, bool showMinimal = false)
    {
        _previousView = CurrentView;
        _showMinimal = showMinimal;
        CurrentView = view;
        switch (CurrentView)
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
                Reset(false);
                ShowMeleeAttack();
                break;
            case View.AttackRanged:
                Reset(false);
                ShowRangedAttack();
                break;
            case View.Ability:
                Reset();
                ShowAbility();
                break;
        }
    }

    public void SetEntityStats(EntityNode entity)
    {
        var actor = (ActorNode)entity;
        
        var currentStats = actor.CurrentStats;
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
        var actorAttributes = actor.Attributes;
        
        SetEntityStats(
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
    }

    private void SetEntityStats(
        int currentHealth,
        int maxHealth,
        float currentMovement,
        int maxMovement,
        int initiative,
        int meleeArmour,
        int rangedArmour,
        IEnumerable<ActorAttribute> actorAttributes,
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
        _valueActorAttributes = actorAttributes;
        _valueCurrentShields = currentShields;
        _valueMaxShields = maxShields;
    }
    
    public void SetMeleeAttackStats(
        bool hasAttack,
        string attackName,
        int distance = 0,
        int damage = 0,
        int bonusDamage = 0,
        ActorAttribute bonusType = null)
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
        ActorAttribute bonusType = null)
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
        TurnPhase turnPhase,
        string text,
        EndsAtNode cooldown,
        IList<ResearchId> research = null)
    {
        _valueAbilityName = abilityName;
        _valueAbilityTurnPhase = turnPhase;
        _valueAbilityText = text;
        _valueResearchText = research is null
            ? string.Empty
            : string.Join(", ", research.Select(x => x.ToString()).ToList()); // TODO add nice display names to research
        _valueAbilityCooldown = cooldown;
    }
    
    public void ResetAttacks()
    {
        _hasMeleeAttack = false;
        _hasRangedAttack = false;
    }

    public void Reset(bool fullReset = true)
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
        _leftSideBottomEmptyBlock.Visible = false;
        _rightSide.Visible = false;
        _rightSideMeleeAttack.Visible = false;
        _rightSideRangedAttack.Visible = false;
        _abilityDescription.Visible = false;
        _actorAttributes.Visible = false;

        if (fullReset is false)
            return;
        
        _rightSideMeleeAttack.SetSelected(false);
        _rightSideRangedAttack.SetSelected(false);
    }

    private void ShowUnitStats()
    {
        _leftSideTopHealth.SetValue(_valueMaxHealth, (_valueCurrentHealth == _valueMaxHealth) is false, _valueCurrentHealth);
        _leftSideTopShields.SetValue(_valueMaxShields, (_valueCurrentShields == _valueMaxShields) is false, _valueCurrentShields);
        _leftSideMiddleMovement.SetValue(_valueMaxMovement, (_valueCurrentMovement == _valueMaxMovement) is false, _valueCurrentMovement);
        _leftSideMiddleInitiative.SetValue(_valueInitiative);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _actorAttributes.SetTypes(_valueActorAttributes);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideMiddleMovement.Visible = _valueCurrentMovement > 0 || _valueMaxMovement > 0;
        _leftSideMiddleInitiative.Visible = _valueInitiative > 0;
        _leftSideBottomMeleeArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideBottomRangedArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _rightSide.Visible = _showMinimal is false;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;
    }

    private void ShowStructureStats()
    {
        _leftSideTopHealth.SetValue(_valueMaxHealth, (_valueCurrentHealth == _valueMaxHealth) is false, _valueCurrentHealth);
        _leftSideTopShields.SetValue(_valueMaxShields, (_valueCurrentShields == _valueMaxShields) is false, _valueCurrentShields);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _actorAttributes.SetTypes(_valueActorAttributes);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideBottomMeleeArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideBottomRangedArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _actorAttributes.Visible = true;
    }

    private void ShowMeleeAttack()
    {
        _leftSideTopText.SetNameText(_valueMeleeName);
        _leftSideTopText.SetMelee();
        _leftSideMiddleDamage.SetValue(_valueMeleeDamage);
        _leftSideMiddleDistance.SetValue(_valueMeleeDistance);
        _actorAttributes.SetTypes(_valueActorAttributes);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;

        if (_valueMeleeBonusType is null)
        {
            _leftSideBottomEmptyBlock.Visible = true;
            return;
        }
        
        _leftSideBottomStatBlockText.SetValue(_valueMeleeBonusDamage);
        _leftSideBottomStatBlockText.SetText(_valueMeleeBonusType.ToDisplayValue());
        _leftSideBottomStatBlockText.Visible = true;
    }

    private void ShowRangedAttack()
    {
        _leftSideTopText.SetNameText(_valueRangedName);
        _leftSideTopText.SetRanged();
        _leftSideMiddleDamage.SetValue(_valueRangedDamage);
        _leftSideMiddleDistance.SetValue(_valueRangedDistance);
        _actorAttributes.SetTypes(_valueActorAttributes);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;

        if (_valueRangedBonusType is null)
        {
            _leftSideBottomEmptyBlock.Visible = true;
            return;
        }
        
        _leftSideBottomStatBlockText.SetValue(_valueRangedBonusDamage);
        _leftSideBottomStatBlockText.SetText(_valueRangedBonusType.ToDisplayValue());
        _leftSideBottomStatBlockText.Visible = true;
    }

    private void ShowAbility()
    {
        GetNode<Label>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/Name/Label").Text = _valueAbilityName;
        _researchText.SetResearch(_valueResearchText);
        GetNode<AbilitySubtitle>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/{nameof(AbilitySubtitle)}")
            .SetAbilitySubtitle(_valueAbilityTurnPhase, _valueAbilityCooldown);
        _abilityDescription.GetNode<RichTextLabel>("Text").BbcodeText = _valueAbilityText;

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
        if (CurrentView != View.AttackMelee)
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
        if (CurrentView != View.AttackRanged)
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