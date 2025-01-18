using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities;

public partial class ClientMap : Map
{
    public event Action FinishedInitializing = delegate { };
    public event Action<Vector2, Terrain, IList<EntityNode>> NewTileHovered = delegate { };
    public event Action<EntityNode> EntityIsBeingPlaced = delegate { };
    public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };

    public Entities Entities { get; private set; }

    private Player _currentPlayer;
    private IList<Rect2> _startingPositions = new List<Rect2>();
    private Vector2 _mapSize = Vector2.Inf;
    private Tiles.TileInstance _hoveredTile = null;
    private Tiles _tileMap;
    private SelectionOverlay _selectionOverlay = SelectionOverlay.None;
    private enum SelectionOverlay
    {
        None,
        Movement,
        Placement,
        Attack
    }

    private bool _tileMapIsInitialized = false;
    private bool _pathfindingIsInitialized = false;

    public override void _Ready()
    {
        base._Ready();
        _tileMap = GetNode<Tiles>($"{nameof(Tiles)}");
        Entities = GetNode<Entities>($"{nameof(Entities)}");

        Entities.NewPositionOccupied += OnEntitiesNewPositionOccupied;
        _tileMap.FinishedInitializing += OnTileMapFinishedInitializing;
        Pathfinding.FinishedInitializing += OnPathfindingFinishedInitializing; 
    }
    
    public override void _ExitTree()
    {
        Entities.NewPositionOccupied -= OnEntitiesNewPositionOccupied;
        _tileMap.FinishedInitializing -= OnTileMapFinishedInitializing;
        Pathfinding.FinishedInitializing -= OnPathfindingFinishedInitializing;
        base._ExitTree();
    }

    public void Initialize(MapCreatedEvent @event)
    {
        if (DebugEnabled) GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} {nameof(ClientMap)}.{nameof(Initialize)}");

        _currentPlayer = Data.Instance.Players.Single(x => x.Id.Equals(GetTree().GetUniqueId()));
        _mapSize = @event.MapSize;
        _startingPositions = @event.StartingPositions[_currentPlayer.Id];
        
        Position = new Vector2((Mathf.Max(_mapSize.x, _mapSize.y) * Constants.TileWidth) / 2, Position.y);
        _tileMap.Initialize(_mapSize, @event.Tiles);
        Pathfinding.Initialize(_mapSize, @event.Tiles);
    }

    private void OnTileMapFinishedInitializing()
    {
        if (DebugEnabled) GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                                   $"{nameof(ClientMap)}.{nameof(OnTileMapFinishedInitializing)}");
        _tileMapIsInitialized = true;
        FinishInitialization();
    }

    private void OnPathfindingFinishedInitializing()
    {
        if (DebugEnabled) GD.Print($"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()} " +
                                   $"{nameof(ClientMap)}.{nameof(OnPathfindingFinishedInitializing)}");
        _pathfindingIsInitialized = true;
        FinishInitialization();
    }

    private void FinishInitialization()
    {
        if (_tileMapIsInitialized is false || _pathfindingIsInitialized is false)
            return;
        
        Entities.Initialize(_tileMap.GetTiles);
        
        _tileMap.FillMapOutsideWithMountains();
        _tileMap.UpdateALlBitmaps();
        
        FinishedInitializing();
    }
    
    public override void _Process(float delta)
    {
        base._Process(delta);
        var mousePosition = GetGlobalMousePosition();
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
        UpdateHoveredTile(mapPosition);

        if (_selectionOverlay is SelectionOverlay.None)
        {
            UpdateHoveredEntity(mousePosition);
        }

        if (_selectionOverlay is SelectionOverlay.Movement)
        {
            UpdateHoveredEntity(mousePosition);

            // TODO optimization: only if hovered tile changed from above, display path
            if (_hoveredTile != null)
            {
                var size = (int)Entities.SelectedEntity.EntitySize.x;
                var path = Pathfinding.FindPath(
                    _hoveredTile.Position, 
                    size);
                _tileMap.SetPathTiles(path, size);
            }
        }

        if (_selectionOverlay is SelectionOverlay.Placement)
        {
            var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(mapPosition);
            Entities.UpdateEntityInPlacement(mapPosition, globalPosition, _tileMap.GetTiles);
        }
    }

    public EntityNode UpdateHoveredEntity(Vector2 mousePosition)
    {
        var entity = Entities.GetTopEntity(mousePosition);

        if (entity != null) // TODO GetTopEntity doesn't work so this is always skipped
        {
            var entityMapPosition = entity.EntityPrimaryPosition;
            if (_hoveredTile.Position == entityMapPosition) 
                return entity;
            
            UpdateHoveredTile(entityMapPosition);
            return entity;
        }
        
        if (_hoveredTile is null)
            return null;

        var entityWasHovered = Entities.TryHoveringEntityOn(_hoveredTile);
        if (entityWasHovered)
        {
            return Entities.HoveredEntity;
        }

        return null;
    }

    public void SetupFactionStart()
    {
        Entities.SetupStartingEntities(_startingPositions.First().ToList(), _currentPlayer.Faction);
    }

    public void HandleDeselecting()
    {
        _tileMap.ClearAvailableTiles(false);
        _tileMap.ClearPath();
        Entities.DeselectEntity();
        _selectionOverlay = SelectionOverlay.None;
    }

    public void MoveUnit(UnitMovedAlongPathEvent @event)
    {
        var selectedEntity = Entities.GetEntityByInstanceId(@event.EntityInstanceId);
        RemoveOccupation(selectedEntity);
        Entities.MoveEntity(selectedEntity, @event.GlobalPath, @event.Path3D.ToList());
    }
    
    private void HandleLeftClick()
    {
        if (_hoveredTile is null || _hoveredTile.IsInBoundsOf(_mapSize) is false)
            return;
        
        if (Entities.EntityMoving)
            return;
        
        var mapPosition = GetMapPositionFromMousePosition();
        UpdateHoveredTile(mapPosition);

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
            var size = (int)unit.EntitySize.x;
            var availableTiles = Pathfinding.GetAvailablePositions(
                entity.EntityPrimaryPosition, 
                unit.Movement, 
                size);
            _tileMap.ClearAvailableTiles(true);
            _tileMap.SetAvailableTiles(unit, availableTiles, size, false);
            _selectionOverlay = SelectionOverlay.Movement;
        }
    }

    private void ExecutePlacement()
    {
        Entities.PlaceEntity();
        Pathfinding.ClearCache();
        ExecuteCancellation();
    }
    
    private void HandleRightClick()
    {
        if (_hoveredTile is null || _hoveredTile.IsInBoundsOf(_mapSize) is false)
            return;
        
        if (Entities.EntityMoving)
            return;

        if (_selectionOverlay is SelectionOverlay.None)
            return;

        if (_selectionOverlay is SelectionOverlay.Movement)
        {
            var mapPosition = GetMapPositionFromMousePosition();
            UpdateHoveredTile(mapPosition);
            
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
        if (_tileMap.IsCurrentlyAvailable(_hoveredTile) is false 
            || Entities.SelectedEntity.EntityPrimaryPosition.Equals(_hoveredTile.Position))
            return;
        
        // TODO automatically move and melee attack enemy unit; ranged attacks are more tricky
        
        var selectedEntity = Entities.SelectedEntity;
        var path = Pathfinding.FindPath(_hoveredTile.Position, (int)selectedEntity.EntitySize.x).ToList();
        var globalPath = _tileMap.GetGlobalPositionsFromMapPositions(path);
        UnitMovementIssued(new UnitMovedAlongPathEvent(selectedEntity.InstanceId, globalPath, path));
        HandleDeselecting();
    }

    private void ExecuteCancellation()
    {
        _tileMap.ClearTargetTiles();
        Entities.CancelPlacement();
        ExecuteEntitySelection(true);
    }

    private void UpdateHoveredTile(Vector2 mapPosition)
    {
        if (_hoveredTile?.Position == mapPosition)
            return;
        
        // TODO handle hovered tile for entities bigger than 1x1 (2x2, 3x2...)
        _hoveredTile = _tileMap.GetTile(mapPosition);
        var hoveredTerrain = _tileMap.GetTerrain(_hoveredTile);
        NewTileHovered(mapPosition, hoveredTerrain, _hoveredTile?.Occupants);
        
        if (_selectionOverlay != SelectionOverlay.Placement)
            _tileMap.MoveFocusedTileTo(mapPosition);

        if (_hoveredTile is null)
            return;

        if (_hoveredTile.Occupants.IsEmpty())
        {
            _tileMap.ClearAvailableTiles(true);
            return;
        }

        if (_hoveredTile.Occupants.Last() is UnitNode unit)
        {
            if (Entities.IsEntitySelected() && unit.InstanceId == Entities.SelectedEntity.InstanceId)
                return;
            
            _tileMap.SetAvailableTiles(unit, Pathfinding.GetAvailablePositions(
                    unit.EntityPrimaryPosition,
                    unit.GetReach(),
                    (int)unit.EntitySize.x,
                    true),
                (int)unit.EntitySize.x,
                true);
        }
        else
            _tileMap.ClearAvailableTiles(true);
    }

    private Vector2 GetMapPositionFromMousePosition() 
        => _tileMap.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());

    private void AddOccupation(EntityNode entity)
    {
        _tileMap.AddOccupation(entity);
        Pathfinding.AddOccupation(entity);
    }

    private void RemoveOccupation(EntityNode entity)
    {
        _tileMap.RemoveOccupation(entity);
        Pathfinding.RemoveOccupation(entity);
        
        foreach (var position in entity.EntityOccupyingPositions)
        {
            var tile = _tileMap.GetTile(position);
            if (tile is null || _tileMap.IsOccupied(position) is false)
                continue;

            foreach (var occupant in tile.Occupants) 
                Pathfinding.AddOccupation(occupant);
        }
    }

    private void OnEntitiesNewPositionOccupied(EntityNode entity)
    {
        var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPrimaryPosition);
        Entities.AdjustGlobalPosition(entity, globalPosition);
        Entities.RegisterRenderer(entity);
        
        AddOccupation(entity);
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
        Entities.CancelPlacement();
        _tileMap.ClearAvailableTiles(false);
        _tileMap.ClearPath();
        _tileMap.DisableFocusedTile();

        var canBePlacedOnTheWholeMap = buildAbility.CanBePlacedOnTheWholeMap();
        _tileMap.SetTargetTiles(buildAbility.GetPlacementPositions(Entities.SelectedEntity, _mapSize), 
            canBePlacedOnTheWholeMap);
        
        var entity = Entities.SetEntityForPlacement(entityId, canBePlacedOnTheWholeMap); 
        // TODO pass in the cost to be tracked inside the buildableNode
        
        _selectionOverlay = SelectionOverlay.Placement;
        EntityIsBeingPlaced(entity);
    }
}