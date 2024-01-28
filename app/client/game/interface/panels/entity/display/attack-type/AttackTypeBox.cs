using Godot;

public class AttackTypeBox : MarginContainer
{
    [Export] public Texture Icon { get; set; }
    
    [Signal] public delegate void Hovering(bool started);
    [Signal] public delegate void Clicked();

    public bool IsSelected { get; private set; } = false;
    
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
        IsSelected = to;
        Highlight(IsSelected);
    }
    
    private void Highlight(bool to)
    {
        if (_attackTypePanel.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("draw_outline", to);
    }
    
    private void OnAttackTypePanelMouseEntered()
    {
        if (IsSelected)
            return;
        
        Highlight(true);
        EmitSignal(nameof(Hovering), true);
    }

    private void OnAttackTypePanelMouseExited()
    {
        _attackTypePanel.SetClicked(false);
        
        if (IsSelected)
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
        SetSelected(!IsSelected);
    }
}