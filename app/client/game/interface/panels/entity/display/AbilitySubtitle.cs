using Godot;
using System;
using low_age_data.Domain.Shared;

public class AbilitySubtitle : MarginContainer
{
    public void SetAbilitySubtitle(TurnPhase abilityType, 
        int cooldown = 0, TurnPhase cooldownType = null)
    {
        var newText = abilityType.ToDisplayValue().Capitalize();
        if (cooldown > 0 && cooldownType is null is false)
        {
            newText += $" (cooldown: {cooldown} {cooldownType.ToDisplayValue().Capitalize()} phases)";
        }

        GetNode<Label>("Label").Text = newText;
        GetNode<Label>("Label/Shadow").Text = newText;
    }

    public void SetSubtitle(string text)
    {
        GetNode<Label>("Label").Text = text;
        GetNode<Label>("Label/Shadow").Text = text;
    }
}
