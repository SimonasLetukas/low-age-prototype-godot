using Godot;

public class AttackTypeBox : MarginContainer
{
    [Export] public Texture Icon { get; set; }
    
    [Signal] public delegate void Hovering(bool started);
    [Signal] public delegate void Clicked();

    private bool _isSelected = false;
    private AttackTypePanel _attackTypePanel;

    public override void _Ready()
    {
        _attackTypePanel = GetNode<AttackTypePanel>(nameof(AttackTypePanel));
        _attackTypePanel.Connect("mouse_entered", this, nameof(OnAttackTypePanelMouseEntered));
        _attackTypePanel.Connect("mouse_exited", this, nameof(OnAttackTypePanelMouseExited));
        _attackTypePanel.Connect("gui_input", this, nameof(OnAttackTypePanelGuiInput));
        
        SetIcon(Icon);
    }

    public void SetIcon(Texture icon)
    {
        GetNode<TextureRect>($"{nameof(AttackTypePanel)}/AttackTypeIcon").Texture = icon;
        GetNode<TextureRect>($"{nameof(AttackTypePanel)}/AttackTypeIcon/Shadow").Texture = icon;
    }
    
    public void SetSelected(bool to)
    {
        _isSelected = to;
        Highlight(_isSelected);
    }
    
    private void Highlight(bool to)
    {
        if (_attackTypePanel.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("enabled", to);
    }
    
    private void OnAttackTypePanelMouseEntered()
    {
        if (_isSelected)
            return;
        
        Highlight(true);
        EmitSignal(nameof(Hovering), true);
    }

    private void OnAttackTypePanelMouseExited()
    {
        _attackTypePanel.SetClicked(false);
        
        if (_isSelected)
            return;
        
        Highlight(false);
        EmitSignal(nameof(Hovering), false);
    }
    
    private void OnAttackTypePanelGuiInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("mouse_left"))
            _attackTypePanel.SetClicked(true);
        
        if (inputEvent.IsActionReleased("mouse_left") is false) 
            return;
        
        _attackTypePanel.SetClicked(false);
        EmitSignal(nameof(Clicked));
        SetSelected(!_isSelected);
    }
}
