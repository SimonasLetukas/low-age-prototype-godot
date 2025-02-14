using Godot;

public partial class AttackTypePanel : TextureRect
{
    [Export] public Texture2D TextureNormal { get; set; }
    [Export] public Texture2D TextureClicked { get; set; }

    private TextureRect _attackTypeIcon;
    
    public override void _Ready()
    {
        _attackTypeIcon = GetNode<TextureRect>("AttackTypeIcon");
        
        SetClicked(false);
    }

    public void SetClicked(bool to)
    {
        switch (to)
        {
            case true:
                Texture = TextureClicked;
                _attackTypeIcon.Modulate = new Color(_attackTypeIcon.Modulate, 0.7f);
                break;
            case false:
                Texture = TextureNormal;
                _attackTypeIcon.Modulate = new Color(_attackTypeIcon.Modulate, 1f);
                break;
        }
    }
}
