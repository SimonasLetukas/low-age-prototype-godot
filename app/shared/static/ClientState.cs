using Godot;

public partial class ClientState : Node
{
    public static ClientState Instance = null!;

    public bool Flattened { get; private set; } = false;
    public bool MovementToggled { get; private set; } = true;
    public bool AttackToggled => MovementToggled is false;
    public bool UiLoading { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }

    public bool ToggleFlattened()
    {
        Flattened = !Flattened;
        EventBus.Instance.RaiseWhenFlattenedChanged(Flattened);
        EventBus.Instance.RaiseAfterFlattenedChanged(Flattened);
        return Flattened;
    }

    public void ToggleMovementAttackOverlay(EntityNode selectedEntity)
    {
        MovementToggled = !MovementToggled;
        EventBus.Instance.RaiseMovementAttackOverlayChanged(selectedEntity);
    }

    public void ResetMovementAttackOverlayToggle(bool movementToggled, EntityNode selectedEntity)
    {
        MovementToggled = movementToggled;
        EventBus.Instance.RaiseMovementAttackOverlayChanged(selectedEntity);
    }
    
    public void SetUiLoading(bool value)
    {
        if (value is false)
            Callable.From(() => UiLoading = value).CallDeferred();
        else
            UiLoading = value;
    }
}