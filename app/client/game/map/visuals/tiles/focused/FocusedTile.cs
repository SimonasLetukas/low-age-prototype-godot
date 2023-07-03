using Godot;

public class FocusedTile : AnimatedSprite
{
    public void MoveTo(Vector2 coordinates) => GlobalPosition = coordinates;
    public void Enable() => Visible = true;
    public void Disable() => Visible = false;
}
