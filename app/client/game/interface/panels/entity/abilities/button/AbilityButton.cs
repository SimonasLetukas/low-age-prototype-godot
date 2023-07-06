using Godot;

public class AbilityButton : NinePatchRect
{
    [Export] public Texture Icon { get; set; }
    [Export] public Texture TextureNormal { get; set; }
    [Export] public Texture TextureClicked { get; set; }
    
    [Signal] public delegate void Hovering(bool started, AbilityButton abilityButton);
    [Signal] public delegate void Clicked(AbilityButton abilityButton);

    private bool _isSelected = false;
    private string _id = string.Empty;

    public override void _Ready()
    {
        SetIcon(Icon);

        Connect("mouse_entered", this, nameof(OnAbilityButtonMouseEntered));
        Connect("mouse_exited", this, nameof(OnAbilityButtonMouseExited));
        Connect("gui_input", this, nameof(OnAbilityButtonGuiInput));
    }

    public void SetId(string id) => _id = id;

    public void SetSelected(bool to)
    {
        _isSelected = to;
        Highlight(_isSelected);
    }

    public void SetClicked(bool to)
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

    private void SetIcon(Texture icon)
    {
        GetNode<TextureRect>(nameof(TextureRect)).Texture = icon;
        GetNode<TextureRect>($"{nameof(TextureRect)}/Shadow").Texture = icon;
    }

    private void Highlight(bool to)
    {
        if (Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("enabled", to);
    }

    private void OnAbilityButtonMouseEntered()
    {
        if (_isSelected)
            return;
        
        Highlight(true);
        EmitSignal(nameof(Hovering), true, this);
    }

    private void OnAbilityButtonMouseExited()
    {
        SetClicked(false);
        
        if (_isSelected)
            return;
        
        Highlight(false);
        EmitSignal(nameof(Hovering), false, this);
    }
    
    private void OnAbilityButtonGuiInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("mouse_left"))
            SetClicked(true);
        
        if (inputEvent.IsActionReleased("mouse_left") is false) 
            return;
        
        SetClicked(false);
        EmitSignal(nameof(Clicked), this);
    }
}
