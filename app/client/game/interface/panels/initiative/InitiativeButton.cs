using System;
using Godot;

public partial class InitiativeButton : BaseButton
{
    public const string ScenePath = @"res://app/client/game/interface/panels/initiative/InitiativeButton.tscn";
    public static InitiativeButton Instance() => (InitiativeButton) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static InitiativeButton InstantiateAsChild(ActorNode actor, Node parentNode)
    {
        var initiativeButton = Instance();
        initiativeButton.SetupInitiativeButton(actor);
        parentNode.AddChild(initiativeButton);
        return initiativeButton;
    }
    
    public new event Action<ActorNode> Clicked = delegate { };
    public new event Action<bool, ActorNode> Hovering = delegate { };

    public ActorNode Actor { get; private set; } = null!;
    
    public override void _Ready()
    {
        base._Ready();
        base.Clicked += OnButtonClicked;
        base.Hovering += OnButtonHovering;
    }

    public override void _ExitTree()
    {
        base.Clicked -= OnButtonClicked;
        base.Hovering -= OnButtonHovering;
        base._ExitTree();
    }

    private void SetupInitiativeButton(ActorNode actor)
    {
        Actor = actor;

        TooltipText = $"{actor.DisplayName}\n" +
                      $"{actor.EntityPrimaryPosition}\n" +
                      $"{actor.Initiative?.MaxAmount}\n" +
                      $"{actor.Player.Name}";

        if (actor.SpriteLocation != null)
            SetIcon(GD.Load<Texture2D>(actor.SpriteLocation));
    }
    
    private void OnButtonClicked() => Clicked(Actor);

    private void OnButtonHovering(bool flag) => Hovering(flag, Actor);
}
