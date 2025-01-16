using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Tiles;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using multipurpose_pathfinding;
using Area = low_age_prototype_common.Area;

public class ClientMap : Map
{
    [Export] public bool DebugLinesEnabled { get; set; } = false;

    public event Action FinishedInitializing = delegate { };
    public event Action<EntityNode> EntityIsBeingPlaced = delegate { };
    public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };

    public Entities Entities { get; private set; }
    private IMultipurposePathfinding Pathfinding { get; set; } = new MultipurposePathfinding();

    private Player _currentPlayer;
    private IList<Area> _startingPositions = new List<Area>();
    private Vector2<int> _mapSize = Vector2Int.Max;
    private Tiles _tileMap;
    private FocusedTile _focusedTile;
    private SelectionOverlay _selectionOverlay = SelectionOverlay.None;

    private enum SelectionOverlay
    {
        None,
        Movement,
        Placement,
        Attack
    }

    private (BuildNode, EntityId) _previousBuildSelection = (null, null);
    private Node2D _lines;

    private bool _tileMapIsInitialized = false;
    private bool _pathfindingIsInitialized = false;
    private bool _tileMapPointsStartedInitialization = false;
    private bool _tileMapPointsInitialized = false;

    public override void _Ready()
    {
        base._Ready();
        _tileMap = GetNode<Tiles>($"{nameof(Tiles)}");
        Entities = GetNode<Entities>($"{nameof(Entities)}");
        _lines = GetNode<Node2D>($"Lines");

        Entities.NewPositionOccupied += OnEntitiesNewPositionOccupied;
        _tileMap.FinishedInitialInitializing += OnTileMapFinishedInitialInitializing;
        _tileMap.FinishedPointInitialization += OnTileMapFinishedPointInitialization;
        Pathfinding.FinishedInitializing += OnPathfindingFinishedInitializing;
        Pathfinding.PointAdded += OnPathfindingPointAdded;
        Pathfinding.PointRemoved += OnPathfindingPointRemoved;
        EventBus.Instance.NewTileFocused += OnNewTileFocused;
        EventBus.Instance.PathfindingUpdating += OnPathfindingUpdating;
    }

    public override void _ExitTree()
    {
        Entities.NewPositionOccupied -= OnEntitiesNewPositionOccupied;
        _tileMap.FinishedInitialInitializing -= OnTileMapFinishedInitialInitializing;
        _tileMap.FinishedPointInitialization -= OnTileMapFinishedPointInitialization;
        Pathfinding.FinishedInitializing -= OnPathfindingFinishedInitializing;
        Pathfinding.PointAdded += OnPathfindingPointAdded;
        Pathfinding.PointRemoved += OnPathfindingPointRemoved;
        EventBus.Instance.NewTileFocused -= OnNewTileFocused;
        EventBus.Instance.PathfindingUpdating -= OnPathfindingUpdating;
        base._ExitTree();
    }

    public void Initialize(MapCreatedEvent @event)
    {
        if (DebugEnabled)
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} {nameof(ClientMap)}.{nameof(Initialize)}");

        _currentPlayer = Data.Instance.Players.Single(x => x.Id.Equals(GetTree().GetNetworkUniqueId()));
        _mapSize = @event.MapSize;
        _startingPositions = @event.StartingPositions[_currentPlayer.Id];

        Position = new Vector2(((float)Mathf.Max(_mapSize.X, _mapSize.Y) * Constants.TileWidth) / 2, Position.y);
        _tileMap.Initialize(_mapSize.ToGodotVector2(), @event.Tiles);

        InitializePathfinding(@event.Tiles);

        _focusedTile = _tileMap.Elevatable.Focused;

        _lines.Visible = DebugEnabled && DebugLinesEnabled;
        ResetLines();
    }

    private void InitializePathfinding(ICollection<(Vector2<int>, TileId)> tiles)
    {
        var blueprint = Data.Instance.Blueprint;
        var initialPositionsAndTerrainIndexes = tiles
            .Select(x => (x.Item1, new low_age_dijkstra.Terrain(blueprint.Tiles
                .First(y => y.Id.Equals(x.Item2)).Terrain.ToIndex())))
            .ToList();
        Pathfinding.Initialize(initialPositionsAndTerrainIndexes, new Configuration
        {
            MapSize = _mapSize,
            TerrainWeights = blueprint.Tiles.ToDictionary(
                tile => tile.Terrain.ToIndex(),
                tile => tile.MovementCost),
            MaxSizeForPathfinding = blueprint.Entities.Units.Max(x => x.Size),
            MaxNumberOfTeams = 2, // TODO
            DiagonalCost = Mathf.Sqrt2,
            DebugEnabled = false
        });
    }

    private void OnTileMapFinishedInitialInitializing()
    {
        if (DebugEnabled)
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(ClientMap)}.{nameof(OnTileMapFinishedInitialInitializing)}");
        _tileMapIsInitialized = true;
        FinishInitialization();
    }

    private void OnPathfindingFinishedInitializing()
    {
        if (DebugEnabled)
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(ClientMap)}.{nameof(OnPathfindingFinishedInitializing)}");
        _pathfindingIsInitialized = true;
        FinishInitialization();
    }

    private void OnTileMapFinishedPointInitialization()
    {
        if (DebugEnabled)
            GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                     $"{nameof(ClientMap)}.{nameof(OnTileMapFinishedPointInitialization)}");
        _tileMapPointsInitialized = true;
        FinishInitialization();
    }

    private void FinishInitialization()
    {
        if (_tileMapIsInitialized is false || _pathfindingIsInitialized is false)
            return;

        if (_tileMapPointsStartedInitialization is false)
        {
            _tileMap.AddPoints(Pathfinding.GetTerrainPoints());
            _tileMapPointsStartedInitialization = true;
            return;
        }

        if (_tileMapPointsInitialized is false)
            return;

        Entities.Initialize(_tileMap.GetHighestTiles, _tileMap.GetTile);

        _tileMap.FillMapOutsideWithMountains();
        _tileMap.UpdateALlBitmaps();

        FinishedInitializing();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (_pathfindingIsInitialized is false)
        {
            Pathfinding.IterateInitialization(delta);
            return;
        }

        var mousePosition = GetGlobalMousePosition();

        if (_selectionOverlay is SelectionOverlay.None)
        {
            UpdateHoveredEntity(mousePosition);
        }

        if (_selectionOverlay is SelectionOverlay.Movement)
        {
            UpdateHoveredEntity(mousePosition);

            // TODO optimization: only if focused tile changed from above, display path
            if (_focusedTile.IsWithinTheMap)
            {
                var size = Entities.SelectedEntity.EntitySize.X;
                var path = Pathfinding.FindPath(
                    _focusedTile.CurrentTile.Point,
                    Entities.SelectedEntity.Team,
                    size);
                _tileMap.Elevatable.SetPathTiles(path, size);
            }
        }

        if (_selectionOverlay is SelectionOverlay.Placement)
        {
            var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
            var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(mapPosition);
            Entities.UpdateEntityInPlacement(mapPosition, globalPosition);
        }

        HandleFlattenInput();
    }

    public void SetupFactionStart()
    {
        Entities.SetupStartingEntities(_startingPositions.First().ToList(), _currentPlayer.Faction);
    }

    public void HandleDeselecting()
    {
        _tileMap.Elevatable.ClearAvailableTiles(false);
        _tileMap.Elevatable.ClearPath();
        Entities.DeselectEntity();
        _selectionOverlay = SelectionOverlay.None;
    }

    public void MoveUnit(UnitMovedAlongPathEvent @event)
    {
        var selectedEntity = Entities.GetEntityByInstanceId(@event.EntityInstanceId);
        RemoveOccupation(selectedEntity);
        Entities.MoveEntity(selectedEntity, @event.GlobalPath, @event.Path.ToList());
    }

    private void HandleFlattenInput()
    {
        if (Input.IsActionJustReleased(Constants.Input.Flatten) is false)
            return;

        ClientState.Instance.ToggleFlattened();
    }

    private void HandleLeftClick()
    {
        if (_focusedTile.IsWithinTheMap is false)
            return;

        if (Entities.EntityMoving)
            return;

        _focusedTile.UpdateTile();

        if (_selectionOverlay is SelectionOverlay.None
            || _selectionOverlay is SelectionOverlay.Movement)
        {
            ExecuteEntitySelection();
            return;
        }

        if (_selectionOverlay is SelectionOverlay.Placement)
        {
            ExecutePlacement();
            return;
        }
    }

    private void ExecuteEntitySelection(bool maintainSelection = false)
    {
        var mousePosition = GetGlobalMousePosition();
        var entity = maintainSelection
            ? Entities.SelectedEntity
            : UpdateHoveredEntity(mousePosition);

        HandleDeselecting();

        if (entity is null)
            return;

        Entities.SelectEntity(entity);

        if (entity is UnitNode unit)
        {
            var size = unit.EntitySize.X;
            var availablePoints = Pathfinding.GetAvailablePoints(
                entity.EntityPrimaryPosition,
                unit.Movement,
                unit.IsOnHighGround,
                unit.Team,
                size);
            _tileMap.Elevatable.ClearAvailableTiles(true);
            _tileMap.Elevatable.SetAvailableTiles(unit, availablePoints, size, false);
            _selectionOverlay = SelectionOverlay.Movement;
        }
    }

    private void ExecutePlacement()
    {
        Entities.PlaceEntity();

        if (Input.IsActionPressed(Constants.Input.RepeatPlacement) && _previousBuildSelection.Item1 != null)
        {
            OnSelectedToBuild(_previousBuildSelection.Item1, _previousBuildSelection.Item2);
            return;
        }

        ExecuteCancellation();
    }

    private void HandleRightClick()
    {
        if (_focusedTile.IsWithinTheMap is false)
            return;

        if (Entities.EntityMoving)
            return;

        if (_selectionOverlay is SelectionOverlay.None)
            return;

        if (_selectionOverlay is SelectionOverlay.Movement)
        {
            _focusedTile.UpdateTile();
            ExecuteMovement();
            return;
        }

        if (_selectionOverlay is SelectionOverlay.Placement
            || _selectionOverlay is SelectionOverlay.Attack)
        {
            ExecuteCancellation();
            return;
        }
    }

    private void ExecuteMovement()
    {
        if (_tileMap.Elevatable.IsCurrentlyAvailable(_focusedTile.CurrentTile) is false
            || Entities.SelectedEntity.EntityPrimaryPosition.Equals(_focusedTile.CurrentTile.Position))
            return;

        // TODO automatically move and melee attack enemy unit; ranged attacks are more tricky

        var selectedEntity = Entities.SelectedEntity;
        var path = Pathfinding.FindPath(
            _focusedTile.CurrentTile.Point,
            selectedEntity.Team,
            selectedEntity.EntitySize.X).ToList();
        var globalPath = _tileMap.GetGlobalPositionsFromMapPoints(path);
        UnitMovementIssued(new UnitMovedAlongPathEvent(selectedEntity.InstanceId, globalPath, path));
        HandleDeselecting();
    }

    private void ExecuteCancellation()
    {
        Pathfinding.ClearCache();
        _tileMap.Elevatable.ClearCache();
        _tileMap.Elevatable.ClearTargetTiles();
        Entities.CancelPlacement();
        _previousBuildSelection = (null, null);
        _focusedTile.Enable();
        ExecuteEntitySelection(true);
    }

    private EntityNode UpdateHoveredEntity(Vector2 mousePosition)
    {
        if (_focusedTile.IsWithinTheMap is false)
            return null;

        var topEntity = Entities.GetTopEntity(mousePosition);

        if (topEntity != null && Input.IsActionPressed(Constants.Input.FocusSelection) is false)
        {
            _focusedTile.FocusEntity(topEntity);
            _focusedTile.UpdateTile();

            if (Entities.IsEntityHovered(topEntity))
                return topEntity;
        }

        var entityWasHovered = Entities.TryHoveringEntityOn(_focusedTile.CurrentTile);
        _focusedTile.StopEntityFocus(); // TODO remove flashing when focusing from one entity to another

        return entityWasHovered ? Entities.HoveredEntity : null;
    }

    private Vector2<int> GetMapPositionFromMousePosition()
        => _tileMap.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());

    private void AddOccupation(EntityNode entity)
    {
        _tileMap.AddOccupation(entity);

        var pathfindingEntity = new PathfindingEntity(entity.InstanceId, entity.EntityPrimaryPosition,
            entity.EntitySize, entity.Team, entity is UnitNode unit && unit.IsOnHighGround,
            entity.CanBeMovedThroughAt, entity.AllowsConnectionBetweenPoints);
        Pathfinding.AddOrUpdateEntity(pathfindingEntity);
    }

    private void RemoveOccupation(EntityNode entity)
    {
        _tileMap.RemoveOccupation(entity);

        Pathfinding.RemoveEntity(entity.InstanceId);

        foreach (var tile in _tileMap.GetEntityTiles(entity))
        {
            if (tile is null || _tileMap.IsOccupied(tile) is false)
                continue;

            foreach (var occupant in tile.Occupants)
            {
                //Pathfinding.AddOccupation(occupant); TODO test if this is still needed
            }
        }
    }

    private void ResetLines()
    {
        foreach (var node in _lines.GetChildren().OfType<Node>())
            node.QueueFree();
    }

    private void UpdateLines()
    {
        if (DebugEnabled is false || DebugLinesEnabled is false)
            return;

        ResetLines();

        const int size = 2;
        const int team = 1;

        for (var x = 0; x < 10; x++)
        {
            for (var y = 0; y < 10; y++)
            {
                var position = new Vector2<int>(x, y);
                var tile = _tileMap.GetHighestTile(position);
                if (tile.Point.IsImpassable)
                    continue;

                for (var offsetX = -1; offsetX < 2; offsetX++)
                {
                    for (var offsetY = -1; offsetY < 2; offsetY++)
                    {
                        var adjacentPosition = position + new Vector2<int>(offsetX, offsetY);
                        var adjacentTile = _tileMap.GetHighestTile(adjacentPosition);
                        if (adjacentTile is null)
                            continue;

                        if (Pathfinding.HasConnection(tile.Point, adjacentTile.Point, team, size) is false
                            || tile.Point.IsImpassable)
                            continue;

                        var line = new Line2D();
                        line.Width = 1;
                        line.ZIndex = 4000;
                        line.Points = new[]
                        {
                            _tileMap.GetGlobalPositionFromMapPosition(position) - Position,
                            _tileMap.GetGlobalPositionFromMapPosition(adjacentPosition) - Position
                        };
                        _lines.AddChild(line);
                    }
                }
            }
        }
    }

    private void OnPathfindingUpdating(IPathfindingUpdatable data, bool isAdded)
    {
        Pathfinding.ClearCache();
        var entityId = data.GetParentEntity().InstanceId;
        switch (data)
        {
            case AscendableNode _:
                if (isAdded)
                    Pathfinding.AddAscendableHighGround(entityId,
                        data.LeveledPositionsWithoutSpriteOffset);
                else
                    Pathfinding.RemoveAscendableHighGround(entityId);
                return;
            case HighGroundNode _:
                if (isAdded)
                    Pathfinding.AddHighGround(entityId,
                        data.FlattenedPositionsWithoutSpriteOffset);
                else
                    Pathfinding.RemoveHighGround(entityId);
                return;
            default:
                return;
        }
    }

    private static void OnPathfindingPointAdded(Point point)
        => EventBus.Instance.RaiseHighGroundPointCreated(point);

    private static void OnPathfindingPointRemoved(Point point)
        => EventBus.Instance.RaiseHighGroundPointRemoved(point);

    private void OnNewTileFocused(Vector2<int> mapPosition, Terrain terrain, IList<EntityNode> occupants)
    {
        if (_focusedTile.IsWithinTheMap is false)
            return;

        if (_focusedTile.CurrentTile.Occupants.IsEmpty())
        {
            _tileMap.Elevatable.ClearAvailableTiles(true);
            return;
        }

        if (_focusedTile.CurrentTile.Occupants.Last() is UnitNode unit)
        {
            if (Entities.IsEntitySelected(unit))
                return;

            var temporaryAvailablePoints = Pathfinding.GetAvailablePoints(
                unit.EntityPrimaryPosition,
                unit.GetReach(),
                unit.IsOnHighGround,
                unit.Team,
                unit.EntitySize.X,
                true);

            _tileMap.Elevatable.SetAvailableTiles(unit, temporaryAvailablePoints, (int)unit.EntitySize.X, true);
        }
        else
            _tileMap.Elevatable.ClearAvailableTiles(true);
    }

    private void OnEntitiesNewPositionOccupied(EntityNode entity)
    {
        var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPrimaryPosition);
        Entities.AdjustGlobalPosition(entity, globalPosition);
        Entities.RegisterRenderer(entity);

        AddOccupation(entity);
        UpdateLines();
    }

    internal void OnMouseLeftReleasedWithoutDrag()
    {
        HandleLeftClick();
    }

    internal void OnMouseRightReleasedWithoutExamine()
    {
        HandleRightClick();
    }

    internal void OnSelectedToBuild(BuildNode buildAbility, EntityId entityId)
    {
        _previousBuildSelection = (buildAbility, entityId);

        Entities.CancelPlacement();
        _tileMap.Elevatable.ClearAvailableTiles(false);
        _tileMap.Elevatable.ClearPath();
        _focusedTile.Disable();

        var canBePlacedOnTheWholeMap = buildAbility.CanBePlacedOnTheWholeMap();
        _tileMap.Elevatable.SetTargetTiles(buildAbility.GetPlacementPositions(Entities.SelectedEntity, _mapSize),
            canBePlacedOnTheWholeMap);

        var entity = Entities.SetEntityForPlacement(entityId, canBePlacedOnTheWholeMap);
        // TODO pass in the cost to be tracked inside the buildableNode

        _selectionOverlay = SelectionOverlay.Placement;
        EntityIsBeingPlaced(entity);
    }

    internal void OnEntityPlaced()
    {
        if (_selectionOverlay is SelectionOverlay.Placement)
            return;

        ExecuteCancellation();
    }
}