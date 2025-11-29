using Godot;
using System;

public partial class AvailableActionItem : NinePatchRect
{
    public const string ScenePath = @"res://app/client/game/interface/panels/entity/available-actions/AvailableActionItem.tscn";
    
    public void Set(Texture2D icon, string tooltip)
    {
        GetNode<TextureRect>(nameof(TextureRect)).Texture = icon;
        TooltipText = tooltip;
    }
}
