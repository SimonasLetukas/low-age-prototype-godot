using Godot;
using System;

public partial class NavigationBoxPanel : TextureRect // TODO extract base class for all buttons (there's a lot of duplication)
{
    [Export] public Texture2D TextureNormal { get; set; }
    [Export] public Texture2D TextureClicked { get; set; }

    private TextureRect _navigationBoxIcon;
    
    public override void _Ready()
    {
        _navigationBoxIcon = GetNode<TextureRect>("NavigationBoxIcon");
        
        SetClicked(false);
    }

    public void SetClicked(bool to)
    {
        switch (to)
        {
            case true:
                Texture2D = TextureClicked;
                _navigationBoxIcon.Modulate = new Color(_navigationBoxIcon.Modulate, 0.7f);
                break;
            case false:
                Texture2D = TextureNormal;
                _navigationBoxIcon.Modulate = new Color(_navigationBoxIcon.Modulate, 1f);
                break;
        }
    }
}
