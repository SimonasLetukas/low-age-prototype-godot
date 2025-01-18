using Godot;

public partial class FocusedTile : AnimatedSprite2D
{
    public void MoveTo(Vector2 coordinates) => GlobalPosition = coordinates;
    public void Enable() => Visible = true;
    public void Disable() => Visible = false;
}
