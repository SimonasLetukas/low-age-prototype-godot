using Godot;
using LowAgeData.Domain.Common;
using LowAgeCommon.Extensions;

public partial class BehaviourBox : NinePatchRect
{
    private const string ScenePath = "res://app/client/game/interface/panels/entity/behaviours/BehaviourBox.tscn";
    private static BehaviourBox Instance() => (BehaviourBox) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static BehaviourBox InstantiateAsChild(BehaviourNode behaviour, Node parentNode)
    {
        var behaviourBox = Instance();
        parentNode.AddChild(behaviourBox);
        behaviourBox.Initialize(behaviour);
        return behaviourBox;
    }
    
    private TextureRect _texture = null!;
    private Text _counter = null!;

    public override void _Ready()
    {
        base._Ready();
        _texture = GetNode<TextureRect>($"{nameof(TextureRect)}");
        _counter = GetNode<Text>($"Counter");
    }

    public void Initialize(BehaviourNode behaviour)
    {
        if (behaviour.SpriteLocation != null)
            _texture.Texture = GD.Load<Texture2D>(behaviour.SpriteLocation);

        var text = behaviour.Description;
        // TODO tooltip should probably also include the name of the behaviour (and remove the behaviour type from the name)

        if (behaviour.Stack.Count > 1)
        {
            _counter.Visible = true;
            _counter.Text = behaviour.Stack.Count.ToString();
            var stacksEndText = behaviour.CanResetDuration ? "All stacks end at" : "Earliest stack ends at";
            text += $"\n\nThis is currently stacked {behaviour.Stack.Count} times.\n\n" +
                    $"{stacksEndText} {behaviour.CurrentDuration.GetText(cooldownPhrasing: false)}.";
        }
        else
        {
            _counter.Visible = false;
            text += $"\n\nEnds at {behaviour.CurrentDuration.GetText(cooldownPhrasing: false)}.";
        }
        
        TooltipText = text.WrapToLines(Constants.MaxTooltipCharCount);
        
        if (behaviour.Alignment.Equals(Alignment.Negative))
            _texture.Modulate = Colors.Red; // TODO take colors from palette
        if (behaviour.Alignment.Equals(Alignment.Positive))
            _texture.Modulate = Colors.Green;
    }
}
