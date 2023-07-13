using Godot;

public class AbilityButton : NinePatchRect
{
    public const string ScenePath = @"res://app/client/game/interface/panels/entity/abilities/button/AbilityButton.tscn";
    
    [Export] public Texture Icon { get; set; }
    [Export] public Texture TextureNormal { get; set; }
    [Export] public Texture TextureClicked { get; set; }
    
    [Signal] public delegate void Hovering(bool started, AbilityButton abilityButton);
    [Signal] public delegate void Clicked(AbilityButton abilityButton);

    public bool IsSelected { get; private set; } = false;
    public string Id { get; private set; } = string.Empty;

    public override void _Ready()
    {
        SetIcon(Icon);

        Connect("mouse_entered", this, nameof(OnAbilityButtonMouseEntered));
        Connect("mouse_exited", this, nameof(OnAbilityButtonMouseExited));
        Connect("gui_input", this, nameof(OnAbilityButtonGuiInput));
    }

    public void SetId(string id) => Id = id;

    public void SetSelected(bool to)
    {
        IsSelected = to;
        Highlight(IsSelected);
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

    public void SetIcon(Texture icon)
    {
        Icon = icon;
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
        if (IsSelected)
            return;
        
        Highlight(true);
        EmitSignal(nameof(Hovering), true, this);
    }

    private void OnAbilityButtonMouseExited()
    {
        SetClicked(false);
        
        if (IsSelected)
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
