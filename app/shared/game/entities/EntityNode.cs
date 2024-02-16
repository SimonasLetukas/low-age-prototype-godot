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
    [Export] public Color PlacementColorSuccess { get; set; } = Colors.Green;
    [Export] public Color PlacementColorInvalid { get; set; } = Colors.Red;
    
    public event Action<EntityNode> FinishedMoving = delegate { };
    
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public Vector2 EntityPosition { get; set; }
    public Vector2 EntitySize { get; protected set; } = Vector2.One;
    public string DisplayName { get; protected set; }
    public bool CanBePlaced { get; protected set; } = true;
    public Behaviours Behaviours { get; protected set; }
    
    private Entity Blueprint { get; set; }
    protected Sprite Sprite { get; private set; }
    private IList<Vector2> _movePath;
    private bool _selected;
    private float _movementDuration;
    private Tween _movement;
    
    public override void _Ready()
    {
        Behaviours = GetNode<Behaviours>(nameof(Behaviours));
        
        _movePath = new List<Vector2>();
        _selected = false;
        _movementDuration = GetDurationFromAnimationSpeed();
        Sprite = GetNode<Sprite>(nameof(Godot.Sprite));
        _movement = GetNode<Tween>("Movement");

        _movement.Connect("tween_all_completed", this, nameof(OnMovementTweenAllCompleted));
    }
    
    public void SetBlueprint(Entity blueprint)
    {
        Blueprint = blueprint;
        DisplayName = blueprint.DisplayName;
        
        // TODO not sure why the below is needed... perhaps should be removed? (also from scene)
        var spriteSize = Sprite.Texture.GetSize();
        var area = GetNode<Area2D>(nameof(Area2D));
        
        var shape = new RectangleShape2D();
        shape.Extents = new Vector2(spriteSize.x / 2, spriteSize.y / 2);

        var collision = new CollisionShape2D();
        collision.Shape = shape;
        
        area.AddChild(collision);
        area.Position = new Vector2(Position.x, Position.y - spriteSize.y / 2);
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

    public virtual void SetOutline(bool to)
    {
        if (Sprite.Material is ShaderMaterial shaderMaterial) 
            shaderMaterial.SetShaderParam("draw_outline", to);
    }

    public void SnapTo(Vector2 globalPosition) => GlobalPosition = globalPosition;

    public void SetForPlacement(bool to, bool placementValid = false)
    {
        if ((Sprite.Material is ShaderMaterial shaderMaterial) is false) 
            return;
        
        shaderMaterial.SetShaderParam("tint_effect_factor", to ? 1 : 0);
        Sprite.Modulate = new Color(Colors.White, to ? 0.5f : 1);
        
        CanBePlaced = placementValid;
        SetPlacementValidityColor(placementValid);
    }

    public bool DeterminePlacementValidity(IList<Tiles.TileInstance> tiles)
    {
        CanBePlaced = IsPlacementGenerallyValid(tiles)
                      && Behaviours.GetBuildables().All(x => x.IsPlacementValid(tiles));
        
        // TODO check for masks
        // TODO figure out how the checks should be made different for "produce" ability
        
        SetPlacementValidityColor(CanBePlaced);
        return CanBePlaced;
    }

    public void Place()
    {
        if (CanBePlaced is false)
        {
            QueueFree();
            return;
        }
        
        SetForPlacement(false);
        Behaviours.RemoveAll<BuildableNode>(); // TODO should be left until building is finished (payment is completed)
    }
    
    public virtual void MoveUntilFinished(List<Vector2> globalPositionPath, Vector2 resultingPosition)
    {
        EntityPosition = resultingPosition;
        
        globalPositionPath.Remove(globalPositionPath.First());
        _movePath = globalPositionPath;
        MoveToNextTarget();
    }
    
    public IEnumerable<Vector2> GetOccupiedPositions()
    {
        var positions = new List<Vector2>();
        for (var x = 0; x < EntitySize.x; x++)
        {
            for (var y = 0; y < EntitySize.y; y++)
            {
                positions.Add(new Vector2(EntityPosition.x + x, EntityPosition.y + y));
            }
        }

        return positions;
    }

    protected void SetPlacementValidityColor(bool to)
    {
        if (Sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParam("tint_color", to ? PlacementColorSuccess : PlacementColorInvalid);
    }

    protected void AdjustSpriteOffset()
    {
        if (Blueprint.Sprite != null)
            Sprite.Texture = GD.Load<Texture>(Blueprint.Sprite);
        
        var offsetFromX = (int)(EntitySize.x - 1) * 
                          new Vector2((int)(Constants.TileWidth / 4), (int)(Constants.TileHeight / 4));
        var offsetFromY = (int)(EntitySize.y - 1) *
                          new Vector2((int)(Constants.TileWidth / 4) * -1, (int)(Constants.TileHeight / 4));
        Sprite.Offset = (Blueprint.CenterOffset.ToGodotVector2() * -1) + offsetFromX + offsetFromY;
    }
    
    private bool IsPlacementGenerallyValid(IList<Tiles.TileInstance> tiles)
    {
        if (tiles.Any(x => x is null))
            return false;

        if (tiles.All(x => x.IsTarget is false))
            return false;

        return true;
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

        Sprite.Scale = GlobalPosition.x > nextMoveTarget.x 
            ? new Vector2(-1, Sprite.Scale.y) 
            : new Vector2(1, Sprite.Scale.y);

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

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        return InstanceId == ((EntityNode)obj).InstanceId;
    }

    public override int GetHashCode()
    {
        // Instance ID might be set after instance is created to sync IDs between multiplayer clients.
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return InstanceId.GetHashCode();
    }
}
