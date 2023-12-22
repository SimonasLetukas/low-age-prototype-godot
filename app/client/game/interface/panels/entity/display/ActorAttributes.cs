using Godot;
using System;
using System.Collections.Generic;
using low_age_data.Domain.Common;

public class ActorAttributes : MarginContainer
{
    [Export] public ActorAttribute[] Attributes { get; set; } =
    {
        ActorAttribute.Light, 
        ActorAttribute.Biological
    };

    private const string Separator = " - ";

    public override void _Ready()
    {
        SetTypes(Attributes);
    }

    public void SetTypes(IEnumerable<ActorAttribute> types)
    {
        var count = 0;
        var label = GetNode<Label>("Label");
        label.Text = "";
        var shadow = GetNode<Label>("Label/Shadow");
        shadow.Text = "";

        foreach (var type in types)
        {
            var typeText = type.ToDisplayValue().Capitalize();
            
            if (count == 0)
            {
                label.Text += typeText;
                shadow.Text += typeText;
            }
            else
            {
                label.Text += Separator + typeText;
                shadow.Text += Separator + typeText;
            }

            count++;
        }
    }

}
