using System;

public class AbilityButton : BaseButton
{
    public const string ScenePath = @"res://app/client/game/interface/panels/entity/abilities/AbilityButton.tscn";

    public new event Action<bool, AbilityButton> Hovering = delegate { };
    public new event Action<AbilityButton> Clicked = delegate { };
    
    public AbilityNode Ability { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        base.Hovering += OnButtonHovering;
        base.Clicked += OnButtonClicked;
    }

    public override void _ExitTree()
    {
        base.Hovering -= OnButtonHovering;
        base.Clicked -= OnButtonClicked;
        base._ExitTree();
    }

    public void SetAbility(AbilityNode ability) => Ability = ability;

    private void OnButtonHovering(bool flag) => Hovering(flag, this);

    private void OnButtonClicked() => Clicked(this);
}
