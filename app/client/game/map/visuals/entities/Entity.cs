using System.Collections.Generic;
using System.Linq;
using Godot;

public class Entity : Node2D
{
    [Export] public string Id { get; set; } = "slave";
    [Export(PropertyHint.Enum, "Fast,Medium,Slow")] public string AnimationSpeed { get; set; } = "Medium";

    [Signal] public delegate void FinishedMoving(Entity entity);
    
    private List<Vector2> _movePath;
    private bool _selected;
    private float _movementDuration;
    private Sprite _sprite;
    private TextureProgress _health;
    private Tween _movement;

    public override void _Ready()
    {
        _movePath = new List<Vector2>();
        _selected = false;
        _movementDuration = GetDurationFromAnimationSpeed();
        _sprite = GetNode<Sprite>(nameof(Sprite));
        _health = GetNode<TextureProgress>("Health");
        _movement = GetNode<Tween>("Movement");
        
        var spriteSize = _sprite.Texture.GetSize();
        var area = GetNode<Area2D>(nameof(Area2D));
        
        var shape = new RectangleShape2D();
        shape.Extents = new Vector2(spriteSize.x / 2, spriteSize.y / 2);

        var collision = new CollisionShape2D();
        collision.Shape = shape;
        
        area.AddChild(collision);
        area.Position = new Vector2(Position.x, Position.y - spriteSize.y / 2);
        
        _health.RectPosition = new Vector2(_health.RectPosition.x, (spriteSize.y * -1) - 2);
        _health.Visible = false;

        _movement.Connect("tween_all_completed", this, nameof(OnMovementTweenAllCompleted));
    }

    public void SetTileHovered(bool to)
    {
        if (_selected) 
            return;
        SetOutline(to);
    }
    
    public void SetSelected(bool to)
    {
        _selected = to;
        SetOutline(to);
    }

    public void SetOutline(bool to)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("enabled", to);
        _health.Visible = to;
    }

    public void MoveUntilFinished(List<Vector2> path)
    {
        path.Remove(path.First());
        _movePath = path;
        MoveToNextTarget();
    }

    private void MoveToNextTarget()
    {
        var nextMoveTarget = _movePath.First();
        _movePath.Remove(nextMoveTarget);
        var actualUnit = (Node2D)GetParent().GetParent();

        _sprite.Scale = actualUnit.GlobalPosition.x > nextMoveTarget.x 
            ? new Vector2(-1, _sprite.Scale.y) 
            : new Vector2(1, _sprite.Scale.y);

        _movement.InterpolateProperty(actualUnit, "global_position", actualUnit.GlobalPosition, 
            nextMoveTarget, _movementDuration, Tween.TransitionType.Quad, Tween.EaseType.InOut);
        _movement.Start();
    }

    private float GetDurationFromAnimationSpeed()
    {
        switch (AnimationSpeed)
        {
            case "Fast":
                return 0.5f;
            case "Medium":
                return 0.25f;
            case "Slow":
                return 0.1f;
            default:
                return 0.25f;
        }
    }

    private void OnMovementTweenAllCompleted()
    {
        if (_movePath.IsEmpty())
        {
            EmitSignal(nameof(FinishedMoving), this);
            return;
        }

        MoveToNextTarget();
    }
}
