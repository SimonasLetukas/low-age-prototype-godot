using Godot;

public partial class StatBlock : MarginContainer
{
    [Export] public Texture2D Icon { get; set; }
    [Export] public float CurrentValue { get; set; } = 999f;
    [Export] public int MaxValue { get; set; } = 999;
    [Export] public bool ShowCurrentValue { get; set; } = false;
    
    public override void _Ready()
    {
        SetIcon(Icon);
        SetValue(MaxValue, ShowCurrentValue, CurrentValue);
    }
    
    public void SetIcon(Texture2D icon)
    {
        GetNode<TextureRect>($"{nameof(HBoxContainer)}/{nameof(Icon)}").Texture = icon;
        GetNode<TextureRect>($"{nameof(HBoxContainer)}/{nameof(Icon)}/Shadow").Texture = icon;
    }

    public void SetValue(int maxValue, bool showCurrentValue = false, float currentValue = -1f)
    {
        CurrentValue = currentValue;
        MaxValue = maxValue;
        ShowCurrentValue = showCurrentValue;

        var newLabelValue = ShowCurrentValue switch
        {
            true => $"{CurrentValue.ToDisplayFormat()}/{maxValue.ToString()}",
            false => maxValue.ToString()
        };

        GetNode<Label>($"{nameof(HBoxContainer)}/{nameof(MarginContainer)}/{nameof(Label)}").Text = newLabelValue;
    }
}
