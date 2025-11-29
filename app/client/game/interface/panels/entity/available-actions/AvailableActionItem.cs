using Godot;
using System;

public partial class AvailableActionItem : NinePatchRect
{
    public const string ScenePath = @"res://app/client/game/interface/panels/entity/available-actions/AvailableActionItem.tscn";
    
    public Texture2D Icon { get; set; }
    public string Tooltip { get; set; }
    
    protected TextureRect TextureRect = null!;
    
    public override void _Ready()
    {
        TextureRect = GetNode<TextureRect>(nameof(Godot.TextureRect));
    }
    
    public void Set(Texture2D icon, string tooltip)
    {
        Icon = icon;
        TextureRect.Texture = icon;
        
        Tooltip = tooltip;
        TooltipText = tooltip;
    }
}
