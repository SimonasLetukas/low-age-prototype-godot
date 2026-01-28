using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Common;

public partial class InfoDisplay : MarginContainer
{
    public event Action AbilitiesClosed = delegate { };
    public event Action AbilityTextResized = delegate { };
    public event Action<bool, AttackType?> AttackSelected = delegate { };
    public event Action AbilityCancelled = delegate { };

    public View CurrentView { get; private set; } = View.UnitStats;
    private View _previousView = View.UnitStats;
    private bool _showMinimal = false;

    private int _valueCurrentHealth = 999;
    private int _valueMaxHealth = 999;
    private int _valueCurrentShields = 999;
    private int _valueMaxShields = 0;
    private float _valueCurrentMovement = 99.9f;
    private int _valueMaxMovement = 99;
    private float _valueCurrentInitiative = 99.9f;
    private int _valueMaxInitiative = 99;
    private int _valueMeleeArmour = 99;
    private int _valueRangedArmour = 99;
    private IEnumerable<ActorAttribute> _valueActorAttributes =
    [
        ActorAttribute.Biological, ActorAttribute.Armoured, ActorAttribute.Ranged
    ];
    private Player _valuePlayer = null!;
    
    private bool _hasMeleeAttack = true;
    private string _valueMeleeName = "Venom Fangs";
    private int _valueMeleeDistance = 1;
    private int _valueMeleeDamage = 999;
    private int _valueMeleeBonusDamage = 999;
    private ActorAttribute? _valueMeleeBonusType = ActorAttribute.Biological;
    
    private bool _hasRangedAttack = true;
    private string _valueRangedName = "Monev Fangs";
    private int _valueRangedDistance = 999;
    private int _valueRangedDamage = 999;
    private int _valueRangedBonusDamage = 999;
    private ActorAttribute? _valueRangedBonusType = ActorAttribute.Armoured;
    
    private string _valueAbilityName = "Build";
    private bool _hasAbilityToCancel = false;
    private TurnPhase _valueAbilityTurnPhase = TurnPhase.Planning;
    private string _valueAbilityText = "Place a ghostly rendition of a selected enemy unit in [b]7[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] to an unoccupied space in a [b]3[/b] [img=15x11]Client/UI/Icons/icon_distance_big.png[/img] from the selected target. The rendition has the same amount of [img=15x11]Client/UI/Icons/icon_health_big.png[/img], [img=15x11]Client/UI/Icons/icon_melee_armour_big.png[/img] and [img=15x11]Client/UI/Icons/icon_ranged_armour_big.png[/img] as the selected target, cannot act, can be attacked and stays for [b]2[/b] action phases. [b]50%[/b] of all [img=15x11]Client/UI/Icons/icon_damage_big.png[/img] done to the rendition is done as pure [img=15x11]Client/UI/Icons/icon_damage_big.png[/img]to the selected target. If the rendition is destroyed before disappearing, the selected target emits a blast which deals [b]10[/b][img=15x11]Client/UI/Icons/icon_melee_attack.png[/img] and slows all adjacent enemies by [b]50%[/b] until the end of their next action.";
    private EndsAtNode _valueAbilityCooldown = null!;
    private string _valueResearchText = "Hardened Matrix";

    private Control _abilityTitle = null!;
    private Research _researchText = null!;
    private NavigationBox _cancelAbility = null!;
    private NavigationBox _navigationBack = null!;
    private Control _leftSide = null!;
    private Control _leftSideTop = null!;
    private TopText _leftSideTopText = null!;
    private StatBlock _leftSideTopHealth = null!;
    private StatBlock _leftSideTopShields = null!;
    private StatBlock _leftSideMiddleMovement = null!;
    private StatBlock _leftSideMiddleInitiative = null!;
    private StatBlock _leftSideMiddleDamage = null!;
    private StatBlock _leftSideMiddleDistance = null!;
    private StatBlock _leftSideBottomMeleeArmour = null!;
    private StatBlock _leftSideBottomRangedArmour = null!;
    private StatBlockText _leftSideBottomStatBlockText = null!;
    private StatBlockText _leftSideBottomEmptyBlock = null!;
    private Control _rightSide = null!;
    private AttackTypeBox _rightSideMeleeAttack = null!;
    private AttackTypeBox _rightSideRangedAttack = null!;
    private Control _abilityDescription = null!;
    private Text _abilityText = null!;
    private ActorAttributes _actorAttributes = null!;
    private PlayerAttributes _playerAttributes = null!;

    public override void _Ready()
    {
        _abilityTitle = GetNode<Control>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle");
        _researchText = GetNode<Research>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/{nameof(Research)}");
        _cancelAbility = GetNode<NavigationBox>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/CancelAbility");
        _navigationBack = GetNode<NavigationBox>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/{nameof(NavigationBox)}");
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
        _abilityText = _abilityDescription.GetNode<Text>(nameof(Text));
        _actorAttributes = GetNode<ActorAttributes>($"{nameof(VBoxContainer)}/{nameof(ActorAttributes)}");
        _playerAttributes = GetNode<PlayerAttributes>($"{nameof(VBoxContainer)}/{nameof(PlayerAttributes)}");
        
        _rightSideMeleeAttack.Clicked += OnMeleeClicked;
        _rightSideRangedAttack.Clicked += OnRangedClicked;
        _rightSideMeleeAttack.Hovering += OnMeleeHovering;
        _rightSideRangedAttack.Hovering += OnRangedHovering;
        _navigationBack.Clicked += OnNavigationBoxClicked;
        _cancelAbility.Clicked += OnCancelAbilityClicked;
        _abilityText.Connect("finished", new Callable(this, nameof(OnTextResized)));
        
        _leftSideBottomEmptyBlock.SetEmpty();
        ShowView(View.UnitStats);
    }

    public override void _ExitTree()
    {
        _rightSideMeleeAttack.Clicked -= OnMeleeClicked;
        _rightSideRangedAttack.Clicked -= OnRangedClicked;
        _rightSideMeleeAttack.Hovering -= OnMeleeHovering;
        _rightSideRangedAttack.Hovering -= OnRangedHovering;
        _navigationBack.Clicked -= OnNavigationBoxClicked;
        _cancelAbility.Clicked -= OnCancelAbilityClicked;
        
        base._ExitTree();
    }

    public void ShowPreviousView()
    {
        CurrentView = _previousView;
        ShowView(CurrentView);
    }

    public void ShowView(View view, bool showMinimal = false)
    {
        if (view != View.AttackMelee && view != View.AttackRanged)
            AttackSelected(false, null);
        
        _previousView = CurrentView;
        _showMinimal = showMinimal;
        CurrentView = view;
        
        if (showMinimal is false)
            ClientState.Instance.SetUiLoading(true);
        
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
                AttackSelected(true, AttackType.Melee);
                break;
            case View.AttackRanged:
                Reset(false);
                ShowRangedAttack();
                AttackSelected(true, AttackType.Ranged);
                break;
            case View.Ability:
                Reset();
                ShowAbility();
                break;
        }
        
        if (showMinimal is false)
            Callable.From(() => ClientState.Instance.SetUiLoading(false)).CallDeferred();
    }

    public void SetEntityStats(EntityNode entity)
    {
        var actor = (ActorNode)entity;
        
        var stats = actor.Stats;
        var health = stats.FirstOrDefault(x => x.StatType.Equals(StatType.Health));
        var shields = stats.FirstOrDefault(x => x.StatType.Equals(StatType.Shields));
        var movement = stats.FirstOrDefault(x => x.StatType.Equals(StatType.Movement));
        var initiative = stats.FirstOrDefault(x => x.StatType.Equals(StatType.Initiative));
        var meleeArmour = stats.FirstOrDefault(x => x.StatType.Equals(StatType.MeleeArmour));
        var rangedArmour = stats.FirstOrDefault(x => x.StatType.Equals(StatType.RangedArmour));
        var actorAttributes = actor.Attributes;
        var player = entity.Player;

        var currentMovement = actor.ActionEconomy.CanMove
            ? MathF.Max((movement?.CurrentAmount ?? 0) - Constants.Pathfinding.SearchIncrement, 0)
            : 0;
        
        SetEntityStats(
            health is null ? 0 : (int)health.CurrentAmount, 
            health?.MaxAmount ?? 0, 
            currentMovement, 
            movement?.MaxAmount ?? 0, 
            initiative?.CurrentAmount ?? 0, 
            initiative?.MaxAmount ?? 0,
            meleeArmour is null ? 0 : (int)meleeArmour.CurrentAmount, 
            rangedArmour is null ? 0 : (int)rangedArmour.CurrentAmount, 
            actorAttributes, 
            player,
            shields is null ? 0 : (int)shields.CurrentAmount, 
            shields?.MaxAmount ?? 0);
    }

    private void SetEntityStats(
        int currentHealth,
        int maxHealth,
        float currentMovement,
        int maxMovement,
        float currentInitiative,
        int maxInitiative,
        int meleeArmour,
        int rangedArmour,
        IEnumerable<ActorAttribute> actorAttributes,
        Player player,
        int currentShields = 0,
        int maxShields = 0)
    {
        _valueCurrentHealth = currentHealth;
        _valueMaxHealth = maxHealth;
        _valueCurrentMovement = currentMovement;
        _valueMaxMovement = maxMovement;
        _valueCurrentInitiative = currentInitiative;
        _valueMaxInitiative = maxInitiative;
        _valueMeleeArmour = meleeArmour;
        _valueRangedArmour = rangedArmour;
        _valueActorAttributes = actorAttributes;
        _valuePlayer = player;
        _valueCurrentShields = currentShields;
        _valueMaxShields = maxShields;
    }
    
    public void SetMeleeAttackStats(
        bool hasAttack,
        string attackName,
        int distance = 0,
        int damage = 0,
        int bonusDamage = 0,
        ActorAttribute? bonusType = null)
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
        ActorAttribute? bonusType = null)
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
        IList<ResearchId> research,
        bool hasAbilityToCancel)
    {
        _valueAbilityName = abilityName;
        _valueAbilityTurnPhase = turnPhase;
        _valueAbilityText = text;
        _hasAbilityToCancel = hasAbilityToCancel;
        _valueResearchText = string.Join(", ", research.Select(x => x.ToString()).ToList()); // TODO add nice display names to research
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
        _cancelAbility.Visible = false;
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
        _playerAttributes.Visible = false;

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
        _leftSideMiddleInitiative.SetValue(_valueMaxInitiative, (_valueCurrentInitiative == _valueMaxInitiative) is false, _valueCurrentInitiative);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _actorAttributes.SetTypes(_valueActorAttributes);
        _playerAttributes.Set(_valuePlayer);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideMiddleMovement.Visible = _valueCurrentMovement > 0 || _valueMaxMovement > 0;
        _leftSideMiddleInitiative.Visible = _valueCurrentInitiative > 0 || _valueMaxInitiative > 0;
        _leftSideBottomMeleeArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideBottomRangedArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _rightSide.Visible = _showMinimal is false;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;
        _playerAttributes.Visible = true;
    }

    private void ShowStructureStats()
    {
        _leftSideTopHealth.SetValue(_valueMaxHealth, (_valueCurrentHealth == _valueMaxHealth) is false, _valueCurrentHealth);
        _leftSideTopShields.SetValue(_valueMaxShields, (_valueCurrentShields == _valueMaxShields) is false, _valueCurrentShields);
        _leftSideBottomMeleeArmour.SetValue(_valueMeleeArmour);
        _leftSideBottomRangedArmour.SetValue(_valueRangedArmour);
        _actorAttributes.SetTypes(_valueActorAttributes);
        _playerAttributes.Set(_valuePlayer);

        _leftSide.Visible = true;
        _leftSideTop.Visible = true;
        _leftSideTopHealth.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideTopShields.Visible = _valueCurrentShields > 0 || _valueMaxShields > 0;
        _leftSideBottomMeleeArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _leftSideBottomRangedArmour.Visible = _valueCurrentHealth > 0 || _valueMaxHealth > 0;
        _actorAttributes.Visible = true;
        _playerAttributes.Visible = true;
    }

    private void ShowMeleeAttack()
    {
        _leftSideTopText.SetNameText(_valueMeleeName);
        _leftSideTopText.SetMelee();
        _leftSideMiddleDamage.SetValue(_valueMeleeDamage);
        _leftSideMiddleDistance.SetValue(_valueMeleeDistance);
        _actorAttributes.SetTypes(_valueActorAttributes);
        _playerAttributes.Set(_valuePlayer);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;
        _playerAttributes.Visible = true;

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
        _playerAttributes.Set(_valuePlayer);

        _leftSide.Visible = true;
        _leftSideTopText.Visible = true;
        _leftSideMiddleDamage.Visible = true;
        _leftSideMiddleDistance.Visible = true;
        _rightSide.Visible = true;
        _rightSideMeleeAttack.Visible = _hasMeleeAttack;
        _rightSideRangedAttack.Visible = _hasRangedAttack;
        _actorAttributes.Visible = true;
        _playerAttributes.Visible = true;

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
        GetNode<Text>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/Top/Name/Text").Text = 
            "[b]" + _valueAbilityName;
        _researchText.SetResearch(_valueResearchText);
        GetNode<AbilitySubtitle>($"{nameof(VBoxContainer)}/TopPart/AbilityTitle/{nameof(AbilitySubtitle)}")
            .SetAbilitySubtitle(_valueAbilityTurnPhase, _valueAbilityCooldown);
        _abilityText.Text = _valueAbilityText;
        _abilityText.ResetSize();

        _abilityTitle.Visible = true;
        _researchText.Visible = string.IsNullOrEmpty(_valueResearchText) is false;
        _cancelAbility.Visible = _hasAbilityToCancel;
        _navigationBack.Visible = true;
        _abilityDescription.Visible = true;
        
        _rightSideMeleeAttack.SetSelected(false);
        _rightSideRangedAttack.SetSelected(false);
    }

    private void OnMeleeClicked()
    {
        if (_rightSideMeleeAttack.IsSelected)
        {
            _rightSideMeleeAttack.SetSelected(false);
            ShowView(View.UnitStats);
            return;
        }
        
        _rightSideMeleeAttack.SetSelected(true);
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
            _rightSideRangedAttack.SetSelected(false);
            ShowView(View.UnitStats);
            return;
        }
        
        _rightSideRangedAttack.SetSelected(true);
        _rightSideMeleeAttack.SetSelected(false);
        if (CurrentView != View.AttackRanged)
        {
            ShowView(View.AttackRanged);
        }
    }

    private void OnMeleeHovering(bool started)
    {
        if (_rightSideMeleeAttack.IsSelected || _rightSideRangedAttack.IsSelected)
            return;
        
        switch (started)
        {
            case true:
                if (CurrentView is View.AttackMelee)
                    break;
                ShowView(View.AttackMelee);
                break;
            
            case false:
                if (CurrentView is not View.AttackMelee)
                    break;
                ShowView(_previousView);
                break;
        }
    }

    private void OnRangedHovering(bool started)
    {
        if (_rightSideMeleeAttack.IsSelected || _rightSideRangedAttack.IsSelected)
            return;
        
        switch (started)
        {
            case true:
                if (CurrentView is View.AttackRanged)
                    break;
                ShowView(View.AttackRanged);
                break;
            
            case false:
                if (CurrentView is not View.AttackRanged)
                    break;
                ShowView(_previousView);
                break;
        }
    }

    private void OnNavigationBoxClicked()
    {
        ShowView(_previousView);
        AbilitiesClosed();
    }

    private void OnCancelAbilityClicked() => AbilityCancelled();

    private void OnTextResized() => AbilityTextResized();
}