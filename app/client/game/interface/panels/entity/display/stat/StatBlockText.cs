using Godot;

public class StatBlockText : MarginContainer
{
    [Export] public Texture Icon { get; set; }
    [Export] public int Value { get; set; } = 999;
    [Export] public string Text { get; set; } = "Biological";

    private StatBlock _statBlock;

    public override void _Ready()
    {
        _statBlock = GetNode<StatBlock>($"{nameof(StatBlock)}");
        
        SetIcon(Icon);
        SetValue(Value);
        SetText(Text);
    }
    
    public void SetIcon(Texture icon)
    {
        _statBlock.SetIcon(icon);
    }

    public void SetValue(int value)
    {
        _statBlock.SetValue(value);
    }

    public void SetText(string text)
    {
        GetNode<Label>($"{nameof(MarginContainer)}/TextType").Text = text;
    }

    public void SetEmpty()
    {
        _statBlock.Visible = false;
        if (FindNode("TextVs") is Label vs) vs.Visible = false;
        if (FindNode("TextType") is Label type) type.Visible = false;
    }
}
