using System;
using Godot;

public class BaseButton : NinePatchRect
{
    [Export] public Texture Icon { get; set; }
    [Export] public Texture TextureNormal { get; set; }
    [Export] public Texture TextureClicked { get; set; }
    
    public event Action<bool> Hovering = delegate { };
    public event Action Clicked = delegate { };
    
    public bool IsSelected { get; private set; } = false;
    public bool IsDisabled { get; private set; } = false;
    
    public override void _Ready()
    {
        SetIcon(Icon);

        Connect("mouse_entered", this, nameof(OnBaseButtonMouseEntered));
        Connect("mouse_exited", this, nameof(OnBaseButtonMouseExited));
        Connect("gui_input", this, nameof(OnBaseButtonGuiInput));
    }

    public void SetDisabled(bool to)
    {
        IsDisabled = to;
        Disable(IsDisabled);
    }
    
    public void SetSelected(bool to)
    {
        IsSelected = to;
        Highlight(IsSelected);
    }

    public void SetIcon(Texture icon)
    {
        Icon = icon;
        GetNode<TextureRect>(nameof(TextureRect)).Texture = icon;
        GetNode<TextureRect>($"{nameof(TextureRect)}/Shadow").Texture = icon;
    }
    
    protected void SetClicked(bool to)
    {
        switch (to)
        {
            case true:
                Texture = TextureClicked;
                break;
            case false:
                Texture = TextureNormal;
                break;
        }
    }

    protected void Highlight(bool to)
    {
        if (Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("draw_outline", to);
    }
    
    protected void Disable(bool to)
    {
        if (Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("disabled", to);

        var icon = GetNode<TextureRect>(nameof(TextureRect));
        if (icon.Material is ShaderMaterial iconMaterial) 
            iconMaterial.SetShaderParam("disabled", to);
    }

    private void OnBaseButtonMouseEntered()
    {
        if (IsSelected)
            return;
        
        Highlight(true);
        Hovering(true);
    }

    private void OnBaseButtonMouseExited()
    {
        SetClicked(false);
        
        if (IsSelected)
            return;
        
        Highlight(false);
        Hovering(false);
    }
    
    private void OnBaseButtonGuiInput(InputEvent inputEvent)
    {
        if (IsDisabled)
            return;
        
        if (inputEvent.IsActionPressed("mouse_left"))
            SetClicked(true);
        
        if (inputEvent.IsActionReleased("mouse_left") is false) 
            return;
        
        SetClicked(false);
        Clicked();
    }
}
