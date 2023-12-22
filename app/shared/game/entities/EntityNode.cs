using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities;

/// <summary>
/// Selectable object that has a presence and is interactable on the map: <see cref="ActorNode"/>
/// (<see cref="StructureNode"/> or <see cref="UnitNode"/>) with abilities and stats, or a simple
/// <see cref="DoodadNode"/>.
/// </summary>
public class EntityNode : Node2D, INodeFromBlueprint<Entity>
{
    public event Action<EntityNode> FinishedMoving = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public Vector2 EntityPosition { get; set; }
    
    private Entity Blueprint { get; set; }
    private IList<Vector2> _movePath;
    private bool _selected;
    private float _movementDuration;
    protected Sprite _sprite;
    private Tween _movement;
    
    public override void _Ready()
    {
        _movePath = new List<Vector2>();
        _selected = false;
        _movementDuration = GetDurationFromAnimationSpeed();
        _sprite = GetNode<Sprite>(nameof(Sprite));
        _movement = GetNode<Tween>("Movement");

        _movement.Connect("tween_all_completed", this, nameof(OnMovementTweenAllCompleted));
    }
    
    public void SetBlueprint(Entity blueprint)
    {
        Blueprint = blueprint;

        _sprite.Texture = GD.Load<Texture>(blueprint.Sprite);
        var spriteSize = _sprite.Texture.GetSize();
        var area = GetNode<Area2D>(nameof(Area2D));
        
        var shape = new RectangleShape2D();
        shape.Extents = new Vector2(spriteSize.x / 2, spriteSize.y / 2);

        var collision = new CollisionShape2D();
        collision.Shape = shape;
        
        area.AddChild(collision);
        area.Position = new Vector2(Position.x, Position.y - spriteSize.y / 2);
    }

    public void SetSpriteOffset(Vector2 to) => _sprite.Offset = to;
    
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

    public virtual void SetOutline(bool to)
    {
        if (_sprite.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("enabled", to);
    }
    
    public virtual void MoveUntilFinished(List<Vector2> globalPositionPath, Vector2 resultingPosition)
    {
        EntityPosition = resultingPosition;
        
        globalPositionPath.Remove(globalPositionPath.First());
        _movePath = globalPositionPath;
        MoveToNextTarget();
    }
    
    private void OnMovementTweenAllCompleted()
    {
        if (_movePath.IsEmpty())
        {
            FinishedMoving(this);
            return;
        }

        MoveToNextTarget();
    }
    
    private void MoveToNextTarget()
    {
        var nextMoveTarget = _movePath.First();
        _movePath.Remove(nextMoveTarget);

        _sprite.Scale = GlobalPosition.x > nextMoveTarget.x 
            ? new Vector2(-1, _sprite.Scale.y) 
            : new Vector2(1, _sprite.Scale.y);

        _movement.InterpolateProperty(this, "global_position", GlobalPosition, 
            nextMoveTarget, _movementDuration, Tween.TransitionType.Quad, Tween.EaseType.InOut);
        _movement.Start();
    }
    
    private static float GetDurationFromAnimationSpeed()
    {
        switch (Config.Instance.AnimationSpeed)
        {
            case Config.AnimationSpeeds.Fast:
                return 0.1f;
            case Config.AnimationSpeeds.Medium:
                return 0.25f;
            case Config.AnimationSpeeds.Slow:
                return 0.5f;
            default:
                return 0.25f;
        }
    }
}
