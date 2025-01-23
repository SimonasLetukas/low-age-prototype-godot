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
        CurrentValue = Mathf.Snapped(currentValue, 0.1f);
        MaxValue = maxValue;
        ShowCurrentValue = showCurrentValue;

        string newLabelValue = null;
        switch (ShowCurrentValue)
        {
            case true:
                newLabelValue = $"{currentValue:0.#}/{maxValue.ToString()}";
                break;
            case false:
                newLabelValue = maxValue.ToString();
                break;
        }

        GetNode<Label>($"{nameof(HBoxContainer)}/{nameof(MarginContainer)}/{nameof(Label)}").Text = newLabelValue;
    }
}
