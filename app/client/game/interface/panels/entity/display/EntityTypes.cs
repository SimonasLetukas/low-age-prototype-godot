using Godot;
using System;

public class EntityTypes : MarginContainer
{
    [Export] public Constants.Game.EntityType[] Types { get; set; } =
    {
        Constants.Game.EntityType.Light, 
        Constants.Game.EntityType.Biological
    };

    private const string Separator = " - ";

    public override void _Ready()
    {
        SetTypes(Types);
    }

    public void SetTypes(Constants.Game.EntityType[] types)
    {
        var count = 0;
        var label = GetNode<Label>("Types");
        label.Text = "";
        var shadow = GetNode<Label>("Types/Shadow");
        shadow.Text = "";

        foreach (var type in types)
        {
            var typeText = type.ToString().Capitalize();
            
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
