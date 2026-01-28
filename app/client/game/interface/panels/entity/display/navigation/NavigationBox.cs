using System;
using Godot;

public partial class NavigationBox : MarginContainer
{
    [Export] public Texture2D Icon { get; set; } = null!;
    
    public event Action Clicked = delegate { };

    private NavigationBoxPanel _navigationBoxPanel = null!;

    public override void _Ready()
    {
        _navigationBoxPanel = GetNode<NavigationBoxPanel>(nameof(NavigationBoxPanel));
        _navigationBoxPanel.Connect("mouse_entered", new Callable(this, nameof(OnNavigationBoxPanelMouseEntered)));
        _navigationBoxPanel.Connect("mouse_exited", new Callable(this, nameof(OnNavigationBoxPanelMouseExited)));
        _navigationBoxPanel.Connect("gui_input", new Callable(this, nameof(OnNavigationBoxPanelGuiInput)));
        
        SetIcon(Icon);
    }

    public void SetIcon(Texture2D icon)
    {
        GetNode<TextureRect>($"{nameof(NavigationBoxPanel)}/NavigationBoxIcon").Texture = icon;
        GetNode<TextureRect>($"{nameof(NavigationBoxPanel)}/NavigationBoxIcon/Shadow").Texture = icon;
    }

    private void Highlight(bool to)
    {
        if (_navigationBoxPanel.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParameter("draw_outline", to);
    }
    
    private void OnNavigationBoxPanelMouseEntered()
    {
        Highlight(true);
    }

    private void OnNavigationBoxPanelMouseExited()
    {
        _navigationBoxPanel.SetClicked(false);
        
        Highlight(false);
    }
    
    private void OnNavigationBoxPanelGuiInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed(Constants.Input.MouseLeft))
            _navigationBoxPanel.SetClicked(true);
        
        if (inputEvent.IsActionReleased(Constants.Input.MouseLeft) is false) 
            return;
        
        _navigationBoxPanel.SetClicked(false);
        Highlight(false);
        Clicked();
    }
}
