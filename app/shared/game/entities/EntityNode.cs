using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Godot;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using LowAgeData.Domain.Common;
using MultipurposePathfinding;
using Area = LowAgeCommon.Area;

/// <summary>
/// Selectable object that has a presence and is interactable on the map: <see cref="ActorNode"/>
/// (<see cref="StructureNode"/> or <see cref="UnitNode"/>) with abilities and stats, or a simple
/// <see cref="DoodadNode"/>.
/// </summary>
public partial class EntityNode : Node2D, INodeFromBlueprint<Entity>
{
    [Export] public Color PlacementColorSuccess { get; set; } = Colors.Green;
    [Export] public Color PlacementColorInvalid { get; set; } = Colors.Red;

    public event Action<EntityNode> Destroyed = delegate { };
    public event Action<EntityNode> FinishedMoving = delegate { };
    
    public EntityId BlueprintId { get; private set; } = null!;

    public Guid InstanceId { get; set; } = Guid.NewGuid();

    public Player Player { get; protected set; } = null!;
    public EntityRenderer Renderer { get; private set; } = null!;
    public Vector2Int EntityPrimaryPosition { get; set; }
    public Vector2Int EntitySize { get; protected set; } = Vector2Int.One;
    public virtual Area RelativeSize => new Area(Vector2Int.Zero, EntitySize);
    public IList<Vector2Int> EntityOccupyingPositions => new Area(EntityPrimaryPosition, EntitySize).ToList();
    public Dictionary<Vector2Int, int> ProvidedHighGroundHeightByOccupyingPosition =>
        _providingHighGroundHeightByLocalEntityPosition.ToDictionary(pair => pair.Key + EntityPrimaryPosition, 
            pair => pair.Value);
    public string DisplayName { get; private set; } = null!;
    public bool CanBePlaced { get; protected set; } = false;
    public Behaviours Behaviours { get; protected set; } = null!;
    public bool IsBeingDestroyed { get; private set; }
    
    protected Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> GetHighestTiles { get; set; } = null!;
    protected Func<Vector2Int, bool, Tiles.TileInstance?> GetTile { get; set; } = null!;
    protected bool Selected { get; private set; } = false;
    protected bool Hovered { get; private set; } = false;
    protected State EntityState { get; private set; }
    protected enum State
    { // Flow (one-way): InPlacement -> Candidate -> Placed -> Completed
        InPlacement, // transient and local to client, available while selecting a valid placement location
        Candidate, // transient and local to client, to wait for the server until planning phase ends 
        Placed, // visible for all clients, in process of being paid for so cannot use abilities, but can be attacked
        Completed // visible for all clients and fully functional
    }

    private Entity Blueprint { get; set; } = null!;

    private IList<Vector2> _movePath = new List<Vector2>();

    private readonly Dictionary<Vector2Int, int> _providingHighGroundHeightByLocalEntityPosition = new();
    private float _movementDuration;
    private bool _canBePlacedOnTheWholeMap = false;
    
    public override void _Ready()
    {
        Renderer = GetNode<EntityRenderer>($"{nameof(EntityRenderer)}");
        Behaviours = GetNode<Behaviours>(nameof(Behaviours));
        
        _movementDuration = GetDurationFromAnimationSpeed();
        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
    }

    public void SetBlueprint(Entity blueprint)
    {
        Blueprint = blueprint;
        BlueprintId = Blueprint.Id;
        DisplayName = blueprint.DisplayName;
    }
    
    public void SetTileHovered(bool to)
    {
        Hovered = to;
        if (Selected) 
            return;
        SetOutline(to);
        UpdateVisuals();
    }
    
    public void SetSelected(bool to)
    {
        Selected = to;
        SetOutline(to);
        UpdateVisuals();
    }

    public virtual void SetOutline(bool to) => Renderer.SetOutline(to, Players.Instance.IsCurrentPlayerEnemyTo(Player));

    public virtual void SnapTo(Vector2 globalPosition)
    {
        GlobalPosition = globalPosition;
        Renderer.AdjustElevationOffset();
        Renderer.AdjustToRelativeSize(RelativeSize);
        Renderer.UpdateSpriteBounds();
    }

    public void SetForPlacement(bool canBePlacedOnTheWholeMap)
    {
        EntityState = State.InPlacement;
        
        CanBePlaced = false;
        _canBePlacedOnTheWholeMap = canBePlacedOnTheWholeMap;
        Renderer.MakeDynamic();
        
        UpdateVisuals();
    }

    public bool DeterminePlacementValidity(bool requiresTargetTiles)
    {
        requiresTargetTiles = requiresTargetTiles && _canBePlacedOnTheWholeMap is false;
        var tiles = GetHighestTiles(EntityOccupyingPositions);
        CanBePlaced = IsPlacementGenerallyValid(tiles, requiresTargetTiles)
                      && Behaviours.GetBuildables().All(x => x.IsPlacementValid(tiles));
        
        // TODO check for masks
        
        UpdateVisuals();
        return CanBePlaced;
    }
    
    public bool SetAsCandidate()
    {
        if (EntityState != State.InPlacement)
            return false;
        
        if (CanBePlaced is false)
        {
            Destroy();
            return false;
        }

        EntityState = State.Candidate;
        CanBePlaced = true;
        UpdateVisuals();
        
        return true;
    }

    public virtual void ForcePlace(EntityPlacedEvent @event)
    {
        EntityPrimaryPosition = @event.MapPosition;
        CanBePlaced = true;
        EntityState = State.Candidate;

        Place();
    }

    public bool Place()
    {
        if (EntityState != State.Candidate && CanBePlaced is false)
        {
            Destroy();
            return false;
        }
        
        EntityState = State.Placed;
        EventBus.Instance.RaiseEntityPlaced(this);
        UpdateVisuals();
        
        Complete(); // TODO should be called outside of this class when e.g. building is finished (payment is completed)
        return true;
    }

    public virtual void Complete()
    {
        if (EntityState != State.Placed)
            return;

        EntityState = State.Completed;
        UpdateVisuals();
        
        Behaviours.RemoveAll<BuildableNode>();
    }
    
    public virtual void MoveUntilFinished(List<Vector2> globalPositionPath, Point resultingPoint)
    {
        EntityPrimaryPosition = resultingPoint.Position;
        Renderer.ResetElevationOffset();
        
        globalPositionPath.Remove(globalPositionPath.First());
        _movePath = globalPositionPath;
        MoveToNextTarget();
    }

    public virtual bool CanBeMovedOnAt(Point point, Team forTeam)
    {
        if (HasHighGroundAt(point, forTeam))
            return true;

        if (point.Position.IsInBoundsOf(EntityPrimaryPosition, EntityPrimaryPosition + EntitySize))
            return false;
        
        return true;
    }

    public virtual bool CanBeMovedThroughAt(Point point, Team forTeam) => true;

    public virtual bool CanBeTargetedBy(EntityNode entity) => Player.Team.IsEnemyTo(entity.Player.Team);

    public bool HasHighGroundAt(Point point, Team forTeam)
    {
        if (point.IsHighGround is false)
            return false;

        var position = point.Position;
        
        if (position.IsInBoundsOf(EntityPrimaryPosition, EntityPrimaryPosition + EntitySize) is false)
            return false;
        
        var pathfindingUpdatableBehaviours = Behaviours.GetPathfindingUpdatables;
        if (pathfindingUpdatableBehaviours.IsEmpty())
            return false;

        var result = pathfindingUpdatableBehaviours.Any(x => 
            x.CanBeMovedOnAt(position, forTeam));
        
        return result;
    }

    public bool AllowsConnectionBetweenPoints(Point fromPoint, Point toPoint, Team forTeam)
    {
        var pathfindingUpdatableBehaviours = Behaviours.GetPathfindingUpdatables;
        if (pathfindingUpdatableBehaviours.IsEmpty())
            return true;
        
        var result = pathfindingUpdatableBehaviours.All(x => 
            x.AllowsConnectionBetweenPoints(fromPoint, toPoint, forTeam));

        return result;
    }

    public virtual void ReceiveAttack(EntityNode source, AttackType attackType) { }
    
    protected virtual void ReceiveDamage(EntityNode source, DamageType damageType, int amount) { }
    
    public void Destroy()
    {
        IsBeingDestroyed = true;
        Destroyed(this);
        QueueFree();
    }

    protected virtual void UpdateVisuals()
    {
        switch (EntityState)
        {
            case State.InPlacement:
            case State.Candidate:
                SetTint(true);
                SetTransparency(true);
                SetPlacementValidityColor(CanBePlaced);
                break;
            case State.Placed:
                SetTint(false);
                SetTransparency(true);
                break;
            case State.Completed:
                SetTransparency(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Renderer.SetIconVisibility(false);
    }
    
    protected virtual void UpdateSprite() // TODO fuse with UpdateVisuals?
    {
        if (Blueprint.Sprite != null)
            Renderer.SetSpriteTexture(Blueprint.Sprite);
        
        AdjustSpriteOffset();
    }

    protected void SetPlacementValidityColor(bool to) 
        => Renderer.SetTintColor(to ? PlacementColorSuccess : PlacementColorInvalid);

    protected void SetTint(bool to) => Renderer.SetTintAmount(to ? 1 : 0);

    protected void SetTransparency(bool to) => Renderer.SetAlphaAmount(to ? 0.5f : 1);

    protected void AdjustSpriteOffset(Vector2? centerOffset = null)
    {
        if (centerOffset is null)
            centerOffset = Blueprint.CenterOffset.ToGodotVector2();
        
        Renderer.UpdateSpriteOffset(EntitySize, (Vector2)centerOffset);
    }
    
    private static bool IsPlacementGenerallyValid(IList<Tiles.TileInstance?> tiles, bool requiresTargetTiles)
    {
        if (tiles.Any(x => x is null))
            return false;

        if (requiresTargetTiles && tiles.All(x => x!.TargetType is TargetType.None))
            return false;
        
        if (tiles.Any(x => x!.Occupants.Any(y => y is UnitNode)))
            return false;

        return true;
    }
    
    private void OnMovementTweenAllCompleted()
    {
        Renderer.UpdateOrigins();
        Renderer.UpdateSpriteBounds();
        
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

        Renderer.AimSprite(nextMoveTarget);

        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", nextMoveTarget, _movementDuration)
            .FromCurrent()
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);
        tween.TweenCallback(Callable.From(OnMovementTweenAllCompleted));
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
    
    private void OnWhenFlattenedChanged(bool to) => UpdateVisuals();
    
    private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded)
    {
        if (isAdded is false || data.IsParentEntity(this) is false)
            return;

        foreach (var pair in data.FlattenedLocalPositions) 
            _providingHighGroundHeightByLocalEntityPosition[pair.Key] = pair.Value;
    }

    public override bool Equals(object? obj)
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
