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

    private GridContainer _possibleActions = null!;
    
    public override void _Ready()
    {
        base._Ready();
        
        _possibleActions = GetNode<GridContainer>(nameof(GridContainer));
        
        base.Clicked += OnButtonClicked;
        base.Hovering += OnButtonHovering;
        Actor.ActionEconomy.Updated += OnActorActionEconomyUpdated;
    }

    public override void _ExitTree()
    {
        base.Clicked -= OnButtonClicked;
        base.Hovering -= OnButtonHovering;
        Actor.ActionEconomy.Updated -= OnActorActionEconomyUpdated;
        
        base._ExitTree();
    }

    private void SetupInitiativeButton(ActorNode actor)
    {
        Actor = actor;

        if (actor.SpriteLocation != null)
            SetIcon(GD.Load<Texture2D>(actor.SpriteLocation));
        
        Callable.From(() => SetDynamicProperties(actor)).CallDeferred();
    }

    private void SetDynamicProperties(ActorNode actor)
    {
        var actorTypeText = actor is UnitNode ? "Unit" : "Structure";
        var numberOfActions = actor.ActionEconomy.NumberOfPossibleActions;

        TooltipText = $"{actorTypeText}: {actor.DisplayName}\n" +
                      $"Position: {actor.EntityPrimaryPosition}\n" +
                      $"Initiative: {actor.Initiative?.MaxAmount}\n" +
                      $"Player: {actor.Player.Name}\n" + 
                      $"Available Actions: {numberOfActions}";

        SetPossibleActions(numberOfActions);
    }

    private void SetPossibleActions(int number)
    {
        ResetPossibleActions();

        for (var i = 0; i < number; i++)
            InitiativePossibleActionItem.InstantiateAsChild(_possibleActions);
    }

    private void ResetPossibleActions()
    {
        foreach (var child in _possibleActions.GetChildren())
        {
            child.QueueFree();
        }
    }
    
    private void OnButtonClicked() => Clicked(Actor);

    private void OnButtonHovering(bool flag) => Hovering(flag, Actor);

    private void OnActorActionEconomyUpdated() => SetDynamicProperties(Actor);
}
