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

    protected Color TintColor = new("e0d1bf");

    protected TextureRect TextureRect = null!;
    
    public override void _Ready()
    {
        TextureRect = GetNode<TextureRect>(nameof(Godot.TextureRect));
        
        SetIcon(Icon);

        Connect("mouse_entered", new Callable(this, nameof(OnBaseButtonMouseEntered)));
        Connect("mouse_exited", new Callable(this, nameof(OnBaseButtonMouseExited)));
    }

    public override void _GuiInput(InputEvent inputEvent)
    {
        base._GuiInput(inputEvent);
        
        if (IsDisabled)
            return;
        
        if (inputEvent.IsActionPressed(Constants.Input.MouseLeft))
            SetClicked(true);
        
        if (inputEvent.IsActionReleased(Constants.Input.MouseLeft) is false) 
            return;
        
        SetClicked(false);
        Clicked();
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

    public void SetTint(bool to)
    {
        TextureRect.SelfModulate = to ? TintColor : Colors.White;
    }

    public void SetIcon(Texture2D icon)
    {
        Icon = icon;
        GetNode<TextureRect>(nameof(Godot.TextureRect)).Texture = icon;
        GetNode<TextureRect>($"{nameof(Godot.TextureRect)}/Shadow").Texture = icon;
    }
    
    protected virtual void SetClicked(bool to)
    {
        Texture = to switch
        {
            true => TextureClicked,
            false => TextureNormal
        };
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

        TextureRect.Modulate = new Color(Colors.White, to ? 0.5f : 1);
    }

    private void OnBaseButtonMouseEntered()
    {
        if (ClientState.Instance.UiLoading)
            return;
        
        if (IsSelected)
            return;
        
        Highlight(true);
        Hovering(true);
    }

    private void OnBaseButtonMouseExited()
    {
        if (ClientState.Instance.UiLoading)
            return;
        
        SetClicked(false);
        
        if (IsSelected)
            return;
        
        Highlight(false);
        Hovering(false);
    }
}
