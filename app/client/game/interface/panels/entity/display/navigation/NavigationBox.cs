using Godot;
using System;

public class NavigationBox : MarginContainer
{
    [Export] public Texture Icon { get; set; }
    
    [Signal] public delegate void Clicked();

    private NavigationBoxPanel _navigationBoxPanel;

    public override void _Ready()
    {
        _navigationBoxPanel = GetNode<NavigationBoxPanel>(nameof(NavigationBoxPanel));
        _navigationBoxPanel.Connect("mouse_entered", this, nameof(OnNavigationBoxPanelMouseEntered));
        _navigationBoxPanel.Connect("mouse_exited", this, nameof(OnNavigationBoxPanelMouseExited));
        _navigationBoxPanel.Connect("gui_input", this, nameof(OnNavigationBoxPanelGuiInput));
        
        SetIcon(Icon);
    }

    public void SetIcon(Texture icon)
    {
        GetNode<TextureRect>($"{nameof(NavigationBoxPanel)}/NavigationBoxIcon").Texture = icon;
        GetNode<TextureRect>($"{nameof(NavigationBoxPanel)}/NavigationBoxIcon/Shadow").Texture = icon;
    }

    private void Highlight(bool to)
    {
        if (_navigationBoxPanel.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("draw_outline", to);
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
        EmitSignal(nameof(Clicked));
    }
}
