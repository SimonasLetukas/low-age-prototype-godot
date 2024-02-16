using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities;

public class ClientMap : Map
{
    public event Action FinishedInitializing = delegate { };
    public event Action<Vector2, Terrain, IList<EntityNode>> NewTileHovered = delegate { };
    public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };

    public Entities Entities { get; private set; }
    
    private ICollection<Vector2> _startingPositions = new List<Vector2>();
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

    public override void _Ready()
    {
        base._Ready();
        _tileMap = GetNode<Tiles>($"{nameof(Tiles)}");
        Entities = GetNode<Entities>($"{nameof(Entities)}");

        Entities.NewPositionOccupied += OnEntitiesNewPositionOccupied;
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
                var path = Pathfinding.FindPath(_hoveredTile.Position);
                _tileMap.SetPathTiles(path);
            }
        }

        if (_selectionOverlay is SelectionOverlay.Placement)
        {
            var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(mapPosition);
            Entities.UpdateEntityInPlacement(mapPosition, globalPosition, _tileMap.GetTiles);
        }
    }
    
    public override void _ExitTree()
    {
        Entities.NewPositionOccupied -= OnEntitiesNewPositionOccupied;
        base._ExitTree();
    }

    public void Initialize(MapCreatedEvent @event)
    {
        if (DebugEnabled) GD.Print($"{nameof(ClientMap)}.{nameof(Initialize)}");
        
        _mapSize = @event.MapSize;
        _startingPositions = @event.StartingPositions;
        
        Position = new Vector2((Mathf.Max(_mapSize.x, _mapSize.y) * Constants.TileWidth) / 2, Position.y);
        _tileMap.Initialize(_mapSize, @event.Tiles);
        Pathfinding.Initialize(_mapSize, @event.Tiles);
        Entities.Initialize();
        
        _tileMap.FillMapOutsideWithMountains();
        _tileMap.UpdateALlBitmaps();
        
        FinishedInitializing();
    }

    public EntityNode UpdateHoveredEntity(Vector2 mousePosition)
    {
        var entity = Entities.GetTopEntity(mousePosition);

        if (entity != null) // TODO GetTopEntity doesn't work so this is always skipped
        {
            var entityMapPosition = Entities.GetMapPositionOfEntity(entity);
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

    public void HandleDeselecting()
    {
        _tileMap.ClearAvailableTiles();
        _tileMap.ClearPath();
        Entities.DeselectEntity();
        _selectionOverlay = SelectionOverlay.None;
    }

    public void MoveUnit(UnitMovedAlongPathEvent @event)
    {
        var selectedEntity = Entities.GetEntityFromMapPosition(@event.CurrentEntityPosition);
        _tileMap.RemoveOccupation(selectedEntity);
        Entities.MoveEntity(selectedEntity, @event.GlobalPath, @event.Path);
    }
    
    private void HandleLeftClick()
    {
        if (_hoveredTile.IsInBoundsOf(_mapSize) is false)
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
            var entityPosition = Entities.GetMapPositionOfEntity(entity);
            var availableTiles = Pathfinding.GetAvailablePositions(entityPosition, unit.Movement);
            _tileMap.SetAvailableTiles(availableTiles);
            _selectionOverlay = SelectionOverlay.Movement;
        }
    }

    private void ExecutePlacement()
    {
        Entities.PlaceEntity();
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
        if (_tileMap.IsCurrentlyAvailable(_hoveredTile) is false)
            return;
        
        // TODO automatically move and melee attack enemy unit; ranged attacks are more tricky
        
        var path = Pathfinding.FindPath(_hoveredTile.Position);
        var globalPath = _tileMap.GetGlobalPositionsFromMapPositions(path);
        var selectedEntity = Entities.SelectedEntity;
        var entityPosition = Entities.GetMapPositionOfEntity(selectedEntity);
        UnitMovementIssued(new UnitMovedAlongPathEvent(entityPosition, globalPath, path));
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
    }

    private Vector2 GetMapPositionFromMousePosition() 
        => _tileMap.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());

    private void OnEntitiesNewPositionOccupied(EntityNode entity)
    {
        var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPosition);
        Entities.AdjustGlobalPosition(entity, globalPosition);
        
        _tileMap.AddOccupation(entity);
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
        _tileMap.ClearAvailableTiles();
        _tileMap.ClearPath();
        _tileMap.DisableFocusedTile();
        
        Entities.SetEntityForPlacement(entityId);
        
        _tileMap.SetTargetTiles(buildAbility.PlacementArea.ToPositions(
            Entities.SelectedEntity.EntityPosition, 
            _mapSize,
            Entities.SelectedEntity));
        
        _selectionOverlay = SelectionOverlay.Placement;
        
        // TODO make multiplayer work with a new event (also figure out duplication during initialization)
        // TODO show buildable description as tooltip while in placement: https://www.reddit.com/r/godot/comments/jg6dtt/new_custom_tooltip_node_turn_any_control_node/
    }
}