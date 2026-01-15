using System;
using Godot;

public partial class AbilityButton : BaseButton
{
    public const string ScenePath = @"res://app/client/game/interface/panels/entity/abilities/AbilityButton.tscn";
    
    [Export] public Texture2D TexturePassiveNormal { get; set; }
    [Export] public Texture2D TexturePassiveClicked { get; set; }

    public new event Action<bool, AbilityButton> Hovering = delegate { };
    public new event Action<AbilityButton> Clicked = delegate { };
    
    public IAbilityNode Ability { get; private set; }

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

    public void SetAbility(IAbilityNode ability)
    {
        Ability = ability;
        if (Ability is PassiveNode)
        {
            Texture = TextureNormal = TexturePassiveNormal;
            TextureClicked = TexturePassiveClicked;
        }
    }

    private void OnButtonHovering(bool flag) => Hovering(flag, this);

    private void OnButtonClicked() => Clicked(this);
}
