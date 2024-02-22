using Godot;

public class Research : MarginContainer
{
    [Export] public string HintTemplate { get; set; } = "Research Needed: ";
    [Export] public string ResearchTemplate { get; set; } = "[R]";
    [Export] public string ResearchName { get; set; } = "Hardened Matrix";
    
    public override void _Ready()
    {
        SetResearch(ResearchName);
        GetNode<Label>($"{nameof(Label)}").Text = ResearchTemplate;
    }

    public void SetResearch(string name)
    {
        HintTooltip = HintTemplate + name + ".";
    }
}
