using Godot;

public class ClientState : Node
{
    public static ClientState Instance = null;

    public bool Flattened { get; private set; } = false;

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }

    public bool ToggleFlattened()
    {
        Flattened = !Flattened;
        EventBus.Instance.RaiseWhenFlattenedChanged(Flattened);
        EventBus.Instance.RaiseAfterFlattenedChanged(Flattened);
        return Flattened;
    }
}