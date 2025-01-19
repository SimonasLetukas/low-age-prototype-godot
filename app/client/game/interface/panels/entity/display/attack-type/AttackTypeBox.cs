using Godot;

public partial class AttackTypeBox : MarginContainer
{
    [Export] public Texture2D Icon { get; set; }
    
    [Signal] public delegate void HoveringEventHandler(bool started);
    [Signal] public delegate void ClickedEventHandler();

    public bool IsSelected { get; private set; } = false;
    
    private AttackTypePanel _attackTypePanel;

    public override void _Ready()
    {
        _attackTypePanel = GetNode<AttackTypePanel>(nameof(AttackTypePanel));
        _attackTypePanel.Connect("mouse_entered", new Callable(this, nameof(OnAttackTypePanelMouseEntered)));
        _attackTypePanel.Connect("mouse_exited", new Callable(this, nameof(OnAttackTypePanelMouseExited)));
        _attackTypePanel.Connect("gui_input", new Callable(this, nameof(OnAttackTypePanelGuiInput)));
        
        SetIcon(Icon);
    }

    public void SetIcon(Texture2D icon)
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
            shaderMaterial.SetShaderParameter("draw_outline", to);
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
        if (inputEvent.IsActionPressed(Constants.Input.MouseLeft))
            _attackTypePanel.SetClicked(true);
        
        if (inputEvent.IsActionReleased(Constants.Input.MouseLeft) is false) 
            return;
        
        _attackTypePanel.SetClicked(false);
        EmitSignal(nameof(Clicked));
        SetSelected(!IsSelected);
    }
}