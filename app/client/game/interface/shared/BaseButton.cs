using System;
using Godot;

public partial class BaseButton : NinePatchRect
{
    [Export] public Texture2D Icon { get; set; }
    [Export] public Texture2D TextureNormal { get; set; }
    [Export] public Texture2D TextureClicked { get; set; }
    
    public event Action<bool> Hovering = delegate { };
    public event Action Clicked = delegate { };
    
    public bool IsSelected { get; private set; } = false;
    public bool IsDisabled { get; private set; } = false;
    
    public override void _Ready()
    {
        SetIcon(Icon);

        Connect("mouse_entered", new Callable(this, nameof(OnBaseButtonMouseEntered)));
        Connect("mouse_exited", new Callable(this, nameof(OnBaseButtonMouseExited)));
        Connect("gui_input", new Callable(this, nameof(OnBaseButtonGuiInput)));
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

    public void SetIcon(Texture2D icon)
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
            shaderMaterial.SetShaderParameter("draw_outline", to);
    }
    
    protected void Disable(bool to)
    {
        if (Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParameter("disabled", to);

        var icon = GetNode<TextureRect>(nameof(TextureRect));
        icon.Modulate = new Color(Colors.White, to ? 0.5f : 1);
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
        
        if (inputEvent.IsActionPressed(Constants.Input.MouseLeft))
            SetClicked(true);
        
        if (inputEvent.IsActionReleased(Constants.Input.MouseLeft) is false) 
            return;
        
        SetClicked(false);
        Clicked();
    }
}
