using Godot;
using System;

public class TopText : VBoxContainer
{
    private Label _name;
    private Label _nameShadow;
    private Label _subtitle;
    private Label _subtitleShadow;

    private const string RangedAttack = "Ranged attack";
    private const string MeleeAttack = "Melee attack";
    
    public override void _Ready()
    {
        _name = GetNode<Label>("Name/Label");
        _nameShadow = GetNode<Label>("Name/Label/Shadow");
        _subtitle = GetNode<Label>($"Type/Label");
        _subtitleShadow = GetNode<Label>($"Type/Label/Shadow");
    }

    public void SetNameText(string name)
    {
        _name.Text = name;
        _nameShadow.Text = name;
    }

    public void SetRanged()
    {
        _subtitle.Text = RangedAttack;
        _subtitleShadow.Text = RangedAttack;
    }

    public void SetMelee()
    {
        _subtitle.Text = MeleeAttack;
        _subtitleShadow.Text = MeleeAttack;
    }
}
