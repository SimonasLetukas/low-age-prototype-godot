using System;
using Godot;

public partial class AttackTypeBox : MarginContainer
{
    [Export] public Texture2D Icon { get; set; }

    public event Action<bool> Hovering = delegate { };
    public event Action Clicked = delegate { };

    public bool IsSelected => _button.IsSelected;
    
    private AttackTypeButton _button = null!;
    
    public override void _Ready()
    {
        _button = GetNode<AttackTypeButton>(nameof(AttackTypeButton));

        _button.Clicked += OnButtonClicked;
        _button.Hovering += OnButtonHovering;
        
        _button.SetIcon(Icon);
    }

    public override void _ExitTree()
    {
        _button.Clicked -= OnButtonClicked;
        _button.Hovering -= OnButtonHovering;
        
        base._ExitTree();
    }
    
    public void SetSelected(bool to) => _button.SetSelected(to);

    private void OnButtonClicked()
    {
        Clicked();
    }

    private void OnButtonHovering(bool flag)
    {
        Hovering(flag);
    }
}