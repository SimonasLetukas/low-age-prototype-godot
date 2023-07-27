using Godot;
using System;

public class TopText : VBoxContainer
{
    private Label _name;
    private Label _nameShadow;
    private AbilitySubtitle _subtitle;

    private const string RangedAttack = "Ranged attack";
    private const string MeleeAttack = "Melee attack";
    
    public override void _Ready()
    {
        _name = GetNode<Label>("Name/Label");
        _nameShadow = GetNode<Label>("Name/Label/Shadow");
        _subtitle = GetNode<AbilitySubtitle>($"{nameof(AbilitySubtitle)}");
    }

    public void SetNameText(string name)
    {
        _name.Text = name;
        _nameShadow.Text = name;
    }

    public void SetRanged()
    {
        _subtitle.SetSubtitle(RangedAttack);
    }

    public void SetMelee()
    {
        _subtitle.SetSubtitle(MeleeAttack);
    }
}
