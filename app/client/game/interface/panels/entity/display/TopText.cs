using Godot;

public partial class TopText : VBoxContainer
{
    private Label _name;
    private Label _subtitle;

    private const string RangedAttack = "Ranged attack";
    private const string MeleeAttack = "Melee attack";
    
    public override void _Ready()
    {
        _name = GetNode<Label>("Name/Label");
        _subtitle = GetNode<Label>($"Type/Label");
    }

    public void SetNameText(string name)
    {
        _name.Text = name;
    }

    public void SetRanged()
    {
        _subtitle.Text = RangedAttack;
    }

    public void SetMelee()
    {
        _subtitle.Text = MeleeAttack;
    }
}
