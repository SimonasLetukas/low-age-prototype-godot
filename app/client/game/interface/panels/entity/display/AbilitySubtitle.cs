using Godot;
using LowAgeData.Domain.Common;

public partial class AbilitySubtitle : MarginContainer
{
    public void SetAbilitySubtitle(TurnPhase phase, EndsAtNode cooldown)
    {
        var newText = phase.ToDisplayValue().Capitalize();
        if (cooldown.HasCompleted() is false)
        {
            newText += $" (cooldown remaining: {cooldown.GetText()})";
        }
        else if (cooldown.HasDuration())
        {
            newText += $" (cooldown: {cooldown.GetText(false)})";
        }

        GetNode<Label>("Label").Text = newText;
    }

    public void SetSubtitle(string text)
    {
        GetNode<Label>("Label").Text = text;
    }
}
