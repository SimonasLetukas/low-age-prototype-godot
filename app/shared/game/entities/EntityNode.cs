using System;
using System.Collections.Generic;
using System.Linq;
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

    public event Action<EntityNode> Completed = delegate { };
    public event Action<EntityNode> Destroyed = delegate { };
    public event Action<EntityNode> FinishedMoving = delegate { };
    
    public EntityId BlueprintId { get; private set; } = null!;

    public Guid InstanceId { get; set; } = Guid.NewGuid();
    public int CreationToken { get; set; }

    public Player Player { get; protected set; } = null!;
    public EntityRenderer Renderer { get; private set; } = null!;
    public Vector2Int EntityPrimaryPosition { get; set; }
    public virtual Tiles.TileInstance EntityPrimaryTile => GetTile(EntityPrimaryPosition, false)!;
    public Vector2Int EntitySize { get; protected set; } = Vector2Int.One;
    public virtual Area RelativeSize => new(Vector2Int.Zero, EntitySize);
    public IList<Vector2Int> EntityOccupyingPositions => new Area(EntityPrimaryPosition, EntitySize).ToList();
    public virtual IList<Tiles.TileInstance> EntityOccupyingTiles => EntityOccupyingPositions
        .Select(position => GetTile(position, false)).WhereNotNull().ToList();
    public bool ProvidesHighGround => Behaviours.GetPathfindingUpdatables.IsEmpty() is false;
    public Dictionary<Vector2Int, int> ProvidedHighGroundHeightByOccupyingPosition =>
        _providingHighGroundHeightByLocalEntityPosition.ToDictionary(pair => pair.Key + EntityPrimaryPosition, 
            pair => pair.Value);
    public string DisplayName { get; private set; } = null!;
    public string? SpriteLocation => Blueprint.Sprite;
    public bool CanBePlaced { get; protected set; } = false;
    public bool HasCost { get; private set; } = true;
    public BuildableNode? CreationProgress { get; protected set; } 
    public Behaviours Behaviours { get; protected set; } = null!;
    public bool IsBeingDestroyed { get; private set; }
    
    protected Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> GetHighestTiles { get; } = GlobalRegistry.Instance.GetHighestTiles;
    protected Func<Vector2Int, bool, Tiles.TileInstance?> GetTile { get; } = GlobalRegistry.Instance.GetTile;
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
        base._Ready();
        
        Renderer = GetNode<EntityRenderer>($"{nameof(EntityRenderer)}");
        Behaviours = GetNode<Behaviours>(nameof(Behaviours));
        
        _movementDuration = GetDurationFromAnimationSpeed();
        
        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
        EventBus.Instance.PhaseStarted += OnPhaseStarted;
    }

    public override void _ExitTree()
    {
        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
        EventBus.Instance.PhaseStarted -= OnPhaseStarted;
        
        base._ExitTree();
    }

    public void SetBlueprint(Entity blueprint)
    {
        Blueprint = blueprint;
        BlueprintId = Blueprint.Id;
        DisplayName = blueprint.DisplayName;
    }

    public Vector2 GetTopCenterOffset()
    {
        const int quarterTileWidth = Constants.TileWidth / 4;
        const int halfTileWidth = Constants.TileWidth / 2;
        const int halfTileHeight = Constants.TileHeight / 2;
        
        var spriteSize = Renderer.SpriteSize;
        var offsetFromX = (RelativeSize.Size.X - 1) * new Vector2(quarterTileWidth, halfTileHeight) +
                          RelativeSize.Start.X * new Vector2(halfTileWidth, halfTileHeight);
        var offsetFromY = (RelativeSize.Size.Y - 1) * new Vector2(quarterTileWidth * -1, halfTileHeight) +
                          RelativeSize.Start.Y * new Vector2(halfTileWidth * -1, halfTileHeight);

        return new Vector2(0, spriteSize.Y * -1 - Renderer.YHighGroundOffset) + offsetFromX + offsetFromY;
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

    public bool IsCandidate() => EntityState is State.Candidate;

    public virtual void ForcePlace(EntityPlacedResponseEvent @event)
    {
        CreationToken = @event.CreationToken;
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
        
        if (HasCost is false)
            Complete();
        
        return true;
    }
    
    protected virtual void Complete()
    {
        Completed(this);
        EntityState = State.Completed;
        UpdateVisuals();
    }
    
    public bool IsCompleted() => EntityState is State.Completed;

    public virtual void SetCost(IList<Payment> cost)
    {
        if (cost.IsEmpty() || CreationProgress is null)
        {
            HasCost = false;
            CreationProgress = null;
            return;
        }
        
        CreationProgress.TotalCost = cost.ToList();
    }

    public virtual void DropDownToLowGround()
    {
        Renderer.ResetElevationOffset();
    }
    
    public virtual void MoveUntilFinished(IList<Vector2> globalPositionPath, IList<Point> path)
    {
        var resultingPoint = path.Last();
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

    public virtual ValidationResult CanBeTargetedBy(EntityNode entity) => Player.Team.IsEnemyTo(entity.Player.Team) 
                                                                          || Config.Instance.AllowSameTeamCombat 
        ? ValidationResult.Valid 
        : ValidationResult.Invalid("Target cannot be on the same team!");
    
    public bool HasHighGroundAt(Point point, Team forTeam)
    {
        if (point.IsHighGround is false)
            return false;

        if (ProvidesHighGround is false)
            return false;

        var position = point.Position;
        
        if (position.IsInBoundsOf(EntityPrimaryPosition, EntityPrimaryPosition + EntitySize) is false)
            return false;

        var result = Behaviours.GetPathfindingUpdatables.Any(x => 
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

    public virtual (int damage, bool isLethal) ReceiveAttack(EntityNode source, AttackType attackType, 
        bool isSimulation) => (0, false);

    protected virtual (int damage, bool isLethal) ReceiveDamage(EntityNode source, DamageType damageType, 
        int amount, bool isSimulation) => (0, false);
    
    public void Destroy()
    {
        if (IsBeingDestroyed)
            return;
        
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

    protected virtual void OnPhaseStarted(int turn, TurnPhase phase) { }
    
    private static bool IsPlacementGenerallyValid(IList<Tiles.TileInstance?> tiles, bool requiresTargetTiles)
    {
        if (tiles.Any(tile => tile is null))
            return false;

        if (requiresTargetTiles && tiles.All(tile => tile!.TargetType is TargetType.None))
            return false;
        
        if (tiles.Any(tile => tile!.IsOccupiedBy<UnitNode>()))
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

    public override bool Equals(object? obj) => NodeFromBlueprint.Equals(this, obj);
    public override int GetHashCode() => NodeFromBlueprint.GetHashCode(this);
}
