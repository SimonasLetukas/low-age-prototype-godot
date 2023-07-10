using Godot;
using System;

public class Type : MarginContainer
{
    public void SetType(Constants.Game.AbilityType abilityType, 
        int cooldown = 0, Constants.Game.AbilityType? cooldownType = null)
    {
        var newText = abilityType.ToString().Capitalize();
        if (cooldown > 0)
        {
            newText += $" (cooldown: {cooldown} {cooldownType.ToString().Capitalize()} phases)";
        }

        GetNode<Label>("Label").Text = newText;
        GetNode<Label>("Label/Shadow").Text = newText;
    }
}
