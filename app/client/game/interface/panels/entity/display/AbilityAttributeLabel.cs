using Godot;

public partial class AbilityAttributeLabel : MarginContainer
{
	[Export] public string TooltipPrefixText { get; set; } = "Research Needed: ";
	[Export] public string DisplayedValue { get; set; } = "[R]";
	[Export] public string Text { get; set; } = "Hardened Matrix";
    
	public override void _Ready()
	{
		SetText(Text);
		GetNode<Label>($"{nameof(Label)}").Text = DisplayedValue;
	}

	public void SetText(string text)
	{
		TooltipText = TooltipPrefixText + text + ".";
	}
}