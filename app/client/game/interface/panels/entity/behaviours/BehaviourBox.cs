using Godot;
using low_age_data.Domain.Common;

public class BehaviourBox : NinePatchRect
{
    public const string ScenePath = "res://app/client/game/interface/panels/entity/behaviours/BehaviourBox.tscn";
    public static BehaviourBox Instance() => (BehaviourBox) GD.Load<PackedScene>(ScenePath).Instance();
    public static BehaviourBox InstantiateAsChild(BehaviourNode behaviour, Node parentNode)
    {
        var behaviourBox = Instance();
        parentNode.AddChild(behaviourBox);
        behaviourBox.Initialize(behaviour);
        return behaviourBox;
    }
    
    private TextureRect _texture;

    public override void _Ready()
    {
        base._Ready();
        _texture = GetNode<TextureRect>($"{nameof(TextureRect)}");
    }

    public void Initialize(BehaviourNode behaviour)
    {
        // TODO also change icon
        
        HintTooltip = behaviour.Description.WrapToLines(Constants.MaxTooltipCharCount);
        // TODO tooltip should probably also include the name of the behaviour (and remove the behaviour type from the name)

        if (behaviour.Alignment.Equals(Alignment.Negative))
            _texture.Modulate = Colors.Red; // TODO take colors from palette
        if (behaviour.Alignment.Equals(Alignment.Positive))
            _texture.Modulate = Colors.Green;
    }
}
