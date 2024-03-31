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
    
    public EntityId BlueprintId { get; set; }
    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public EntityRenderer Renderer { get; private set; }
    public Vector2 EntityPrimaryPosition { get; set; }
    public Vector2 EntitySize { get; protected set; } = Vector2.One;
    public IList<Vector2> EntityOccupyingPositions => new Rect2(EntityPrimaryPosition, EntitySize).ToList();
    public string DisplayName { get; protected set; }
    public bool CanBePlaced { get; protected set; } = false;
    public Behaviours Behaviours { get; protected set; }
    public Func<IList<Vector2>, IList<Tiles.TileInstance>> GetTiles { protected get; set; }
    
    private Entity Blueprint { get; set; }
    protected Sprite Sprite { get; private set; }
    protected State EntityState { get; private set; }
    protected enum State
    { // Flow (one-way): InPlacement -> Candidate -> Placed -> Completed
        InPlacement, // transient and local to client, available while selecting a valid placement location
        Candidate, // transient and local to client, to wait for the server until planning phase ends 
        Placed, // visible for all clients, in process of being paid for so cannot use abilities, but can be attacked
        Completed // visible for all clients and fully functional
    }
    private IList<Vector2> _movePath;
    private bool _selected;
    private float _movementDuration;
    private Tween _movement;
    private bool _canBePlacedOnTheWholeMap = false;
    
    public override void _Ready()
    {
        Renderer = GetNode<EntityRenderer>($"{nameof(EntityRenderer)}");
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
        BlueprintId = Blueprint.Id;
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

    public virtual void SnapTo(Vector2 globalPosition) 
        => GlobalPosition = globalPosition;

    public void SetForPlacement(bool canBePlacedOnTheWholeMap)
    {
        EntityState = State.InPlacement;
        SetTint(true);
        SetTransparency(true);
        
        CanBePlaced = false;
        SetPlacementValidityColor(CanBePlaced);
        _canBePlacedOnTheWholeMap = canBePlacedOnTheWholeMap;
    }

    public void OverridePlacementValidity() => CanBePlaced = true;

    public bool DeterminePlacementValidity(bool requiresTargetTiles)
    {
        requiresTargetTiles = requiresTargetTiles && _canBePlacedOnTheWholeMap is false;
        var tiles = GetTiles(EntityOccupyingPositions);
        CanBePlaced = IsPlacementGenerallyValid(tiles, requiresTargetTiles)
                      && Behaviours.GetBuildables().All(x => x.IsPlacementValid(tiles));
        
        // TODO check for masks
        
        SetPlacementValidityColor(CanBePlaced);
        return CanBePlaced;
    }
    
    public bool SetAsCandidate()
    {
        if (EntityState != State.InPlacement)
            return false;
        
        if (CanBePlaced is false)
        {
            QueueFree();
            return false;
        }

        EntityState = State.Candidate;
        SetTint(true);
        SetTransparency(true);
        
        CanBePlaced = true;
        SetPlacementValidityColor(CanBePlaced);
        return true;
    }

    public bool Place()
    {
        if (EntityState != State.Candidate && CanBePlaced is false)
        {
            QueueFree();
            return false;
        }
        
        EntityState = State.Placed;
        SetTint(false);
        SetTransparency(true);
        UpdateSprite();
        
        Complete(); // TODO should be called outside of this class when e.g. building is finished (payment is completed)
        return true;
    }

    public void Complete()
    {
        if (EntityState != State.Placed)
            return;

        EntityState = State.Completed;
        SetTransparency(false);
        
        Behaviours.RemoveAll<BuildableNode>();
    }
    
    public virtual void MoveUntilFinished(List<Vector2> globalPositionPath, Vector2 resultingPosition)
    {
        EntityPrimaryPosition = resultingPosition;
        
        globalPositionPath.Remove(globalPositionPath.First());
        _movePath = globalPositionPath;
        MoveToNextTarget();
    }

    public virtual bool CanBeMovedOnAt(Vector2 position)
    {
        if (position.IsInBoundsOf(EntityPrimaryPosition, EntityPrimaryPosition + EntitySize))
            return false;
        
        return true;
    }

    public virtual bool CanBeMovedThroughAt(Vector2 position) => true;

    protected void SetPlacementValidityColor(bool to)
    {
        if (Sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParam("tint_color", to ? PlacementColorSuccess : PlacementColorInvalid);
    }

    protected void SetTint(bool to)
    {
        if (Sprite.Material is ShaderMaterial shaderMaterial)
            shaderMaterial.SetShaderParam("tint_effect_factor", to ? 1 : 0);
    }

    protected void SetTransparency(bool to)
    {
        Sprite.Modulate = new Color(Colors.White, to ? 0.5f : 1);
    }

    protected void AdjustSpriteOffset(Vector2? centerOffset = null)
    {
        if (centerOffset is null)
            centerOffset = Blueprint.CenterOffset.ToGodotVector2();
        
        var offsetFromX = (int)(EntitySize.x - 1) * 
                          new Vector2((int)(Constants.TileWidth / 4), (int)(Constants.TileHeight / 4));
        var offsetFromY = (int)(EntitySize.y - 1) *
                          new Vector2((int)(Constants.TileWidth / 4) * -1, (int)(Constants.TileHeight / 4));
        Sprite.Offset = ((Vector2)centerOffset * -1) + offsetFromX + offsetFromY;
    }

    protected virtual void UpdateSprite()
    {
        if (Blueprint.Sprite != null)
            Sprite.Texture = GD.Load<Texture>(Blueprint.Sprite);
        
        AdjustSpriteOffset();
    }
    
    private bool IsPlacementGenerallyValid(IList<Tiles.TileInstance> tiles, bool requiresTargetTiles)
    {
        if (tiles.Any(x => x is null))
            return false;

        if (requiresTargetTiles && tiles.All(x => x.IsTarget is false))
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
