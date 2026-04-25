using System;
using System.Linq;
using Godot;
using LowAgeData.Domain.Common;

public partial class AbilityButton : BaseButton
{
    private const string ScenePath = @"res://app/client/game/interface/panels/entity/abilities/AbilityButton.tscn";
    private static AbilityButton Instance() => (AbilityButton) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static AbilityButton InstantiateAsChild(Node parentNode)
    {
        var abilityButton = Instance();
        parentNode.AddChild(abilityButton);
        return abilityButton;
    }
    
    [Export] public Texture2D TexturePassiveNormal { get; set; }
    [Export] public Texture2D TexturePassiveClicked { get; set; }

    public new event Action<bool, AbilityButton> Hovering = delegate { };
    public new event Action<AbilityButton> Clicked = delegate { };

    public IAbilityNode Ability { get; private set; } = null!;

    private Text _cooldown = null!;

    public override void _Ready()
    {
        base._Ready();
        
        _cooldown = GetNode<Text>(nameof(Text));

        base.Hovering += OnButtonHovering;
        base.Clicked += OnButtonClicked;
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
    }

    public override void _ExitTree()
    {
        if (Ability is not null)
            Ability.RemainingCooldown.Updated -= OnRemainingCooldownUpdated;
        
        base.Hovering -= OnButtonHovering;
        base.Clicked -= OnButtonClicked;
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;

        base._ExitTree();
    }

    private void OnPhaseStarted(int turn, TurnPhase phase) 
        => DisableVisually(Ability.TurnPhase.Equals(TurnPhase.Passive) is false 
                           && phase.Equals(Ability.TurnPhase) is false);

    public void SetAbility(IAbilityNode ability)
    {
        if (Ability is not null)
            Ability.RemainingCooldown.Updated -= OnRemainingCooldownUpdated;
        
        Ability = ability;
        if (Ability is PassiveNode)
        {
            Texture = TextureNormal = TexturePassiveNormal;
            TextureClicked = TexturePassiveClicked;
        }
        
        if (Ability.SpriteLocation != null)
            SetIcon(GD.Load<Texture2D>(Ability.SpriteLocation));
        
        _cooldown.Text = "[b]" + Ability.RemainingCooldown.GetCounterOrEmpty();

        Ability.RemainingCooldown.Updated += OnRemainingCooldownUpdated;

        var hasRequiredResearch = Ability.ResearchNeeded.All(r
            => GlobalRegistry.Instance.GetResearchByPlayer(Ability.OwnerActor.Player).Contains(r));
        
        DisableVisually(hasRequiredResearch is false
                        || (Ability.TurnPhase.Equals(TurnPhase.Passive) is false 
                            && GlobalRegistry.Instance.GetCurrentPhase().Equals(Ability.TurnPhase) is false)
                        || Ability.OwnerActor.AbilitiesDisabled);
        
        SetTint(Ability.OwnerActor.WorkingOn.Any(w => w.Ability.Equals(Ability)));
    }

    private void OnRemainingCooldownUpdated(EndsAtNode cooldown)
    {
        _cooldown.Text = "[b]" + cooldown.GetCounterOrEmpty();
    }

    private void OnButtonHovering(bool flag) => Hovering(flag, this);

    private void OnButtonClicked() => Clicked(this);
}
