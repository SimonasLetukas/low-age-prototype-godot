using Godot;
using LowAgeData.Domain.Common;

public partial class AbilitySubtitle : MarginContainer
{
    private Text _text = null!;
    
    public override void _Ready()
    {
        base._Ready();

        _text = GetNode<Text>($"{nameof(Text)}");
        
        _text.IsBlue = true;
    }

    public void SetAbilitySubtitle(TurnPhase phase, EndsAtNode cooldown)
    {
        var newText = "[i]" + phase.ToDisplayValue().Capitalize();
        if (cooldown.HasCompleted() is false)
        {
            newText += $" (cooldown remaining: {cooldown.GetText()})";
        }
        else if (cooldown.HasDuration())
        {
            newText += $" (cooldown: {cooldown.GetText(false)})";
        }

        _text.Text = newText;
    }

    public void SetSubtitle(string text)
    {
        _text.Text = text;
    }
}
