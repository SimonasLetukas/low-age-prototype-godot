using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Common;

public partial class ActorAttributes : MarginContainer
{
    private ActorAttribute[] Attributes { get; set; } =
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

        foreach (var type in types)
        {
            var typeText = type.ToDisplayValue().Capitalize();
            
            if (count == 0)
            {
                label.Text += typeText;
            }
            else
            {
                label.Text += Separator + typeText;
            }

            count++;
        }
    }

}
