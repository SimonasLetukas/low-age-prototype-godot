using Godot;
using low_age_data.Domain.Common;

public partial class AbilitySubtitle : MarginContainer
{
    public void SetAbilitySubtitle(TurnPhase abilityType, EndsAtNode cooldown)
    {
        var newText = abilityType.ToDisplayValue().Capitalize();
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
