using Godot;
using System;

public class TopText : VBoxContainer
{
    private Label _name;
    private Label _nameShadow;
    private Label _type;
    private Label _typeShadow;

    private const string RangedAttack = "Ranged attack";
    private const string MeleeAttack = "Melee attack";
    
    public override void _Ready()
    {
        _name = GetNode<Label>("Name/Label");
        _nameShadow = GetNode<Label>("Name/Label/Shadow");
        _type = GetNode<Label>("Type/Label");
        _typeShadow = GetNode<Label>("Type/Label/Shadow");
    }

    public void SetNameText(string name)
    {
        _name.Text = name;
        _nameShadow.Text = name;
    }

    public void SetRanged()
    {
        _type.Text = RangedAttack;
        _typeShadow.Text = RangedAttack;
    }

    public void SetMelee()
    {
        _type.Text = MeleeAttack;
        _typeShadow.Text = MeleeAttack;
    }
}
