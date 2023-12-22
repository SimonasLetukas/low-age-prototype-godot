using Godot;
using System;
using System.Collections.Generic;
using low_age_data.Domain.Common;

public class ClientMap : Map
{
    public event Action FinishedInitializing = delegate { };
    public event Action<Vector2, Terrain> NewTileHovered = delegate { };
    public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };

    private ICollection<Vector2> _startingPositions = new List<Vector2>();
    private Vector2 _mapSize = Vector2.Inf;
    private Vector2 _tileHovered = Vector2.Zero;
    private Tiles _tileMap;
    private Entities _entities;

    public override void _Ready()
    {
        base._Ready();
        _tileMap = GetNode<Tiles>($"{nameof(Tiles)}");
        _entities = GetNode<Entities>($"{nameof(Entities)}");

        _entities.NewEntityFound += OnEntitiesNewEntityFound;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        var mousePosition = GetGlobalMousePosition();
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
        GetHoveredEntity(mousePosition, mapPosition);
        if (_entities.IsEntitySelected())
        {
            // TODO optimization: only if hovered tile changed from above, display path
            var path = Pathfinding.FindPath(_tileHovered);
            _tileMap.SetPathTiles(path);
        }
        else
        {
            _tileMap.ClearPath();
        }
    }
    
    public override void _ExitTree()
    {
        _entities.NewEntityFound -= OnEntitiesNewEntityFound;
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
        _entities.Initialize();
        
        _tileMap.FillMapOutsideWithMountains();
        _tileMap.UpdateALlBitmaps();
        
        FinishedInitializing();
    }

    public EntityNode GetHoveredEntity(Vector2 mousePosition, Vector2 mapPosition)
    {
        var entity = _entities.GetTopEntity(mousePosition);

        if (entity != null)
        {
            var entityMapPosition = _entities.GetMapPositionOfEntity(entity);
            if (_tileHovered == entityMapPosition) 
                return entity;
            
            _tileHovered = entityMapPosition;
            HoverTile();
        }
        else if (_tileHovered != mapPosition)
        {
            _tileHovered = mapPosition;
            var entityHovered = HoverTile();
            if (entityHovered)
            {
                entity = _entities.GetHoveredEntity();
            }
        }
        else
        {
            entity = _entities.GetHoveredEntity();
        }

        return entity;
    }

    public void HandleExecute(Vector2 mapPosition)
    {
        if (_entities.EntityMoving)
            return;

        if (_entities.IsEntitySelected() is false) 
            return;

        if (_tileMap.IsCurrentlyAvailable(_tileHovered) is false)
            return;
        
        var path = Pathfinding.FindPath(_tileHovered);
        var globalPath = _tileMap.GetGlobalPositionsFromMapPositions(path);
        var selectedEntity = _entities.SelectedEntity;
        var entityPosition = _entities.GetMapPositionOfEntity(selectedEntity);
        UnitMovementIssued(new UnitMovedAlongPathEvent(entityPosition, globalPath, path));
        HandleDeselecting();
    }

    public void HandleSelecting(EntityNode hoveredEntity)
    {
        if (_tileHovered.IsInBoundsOf(_mapSize) is false)
            return;

        if (hoveredEntity is null)
        {
            HandleDeselecting();
            return;
        }

        if (_entities.EntityMoving)
            return;
        
        if (hoveredEntity is UnitNode hoveredUnit)
        {
            var entityPosition = _entities.GetMapPositionOfEntity(hoveredEntity);
            var availableTiles = Pathfinding.GetAvailablePositions(entityPosition, hoveredUnit.Movement);
            _tileMap.SetAvailableTiles(availableTiles);
        }
        
        _entities.SelectEntity(hoveredEntity);
    }

    public void HandleDeselecting()
    {
        _tileMap.ClearAvailableTiles();
        _entities.DeselectEntity();
    }

    public void MoveUnit(UnitMovedAlongPathEvent @event)
    {
        var selectedEntity = _entities.GetEntityFromMapPosition(@event.CurrentEntityPosition);
        _entities.MoveEntity(selectedEntity, @event.GlobalPath, @event.Path);
    }
    
    private bool HoverTile()
    {
        var hoveredTerrain = _tileMap.GetTerrain(_tileHovered);
        NewTileHovered(_tileHovered, hoveredTerrain);
        _tileMap.MoveFocusedTileTo(_tileHovered);
        var entityHovered = _entities.TryHoveringEntity(_tileHovered);
        return entityHovered;
    }

    private void OnEntitiesNewEntityFound(EntityNode entity)
    {
        var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPosition);
        _entities.AdjustGlobalPosition(entity, globalPosition);
    }

    internal void OnMouseLeftReleasedWithoutDrag()
    {
        var mousePosition = GetGlobalMousePosition();
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
        var entity = GetHoveredEntity(mousePosition, mapPosition);
        HandleSelecting(entity);
    }

    internal void OnMouseRightReleasedWithoutExamine()
    {
        var mousePosition = GetGlobalMousePosition();
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
        HandleExecute(mapPosition);
    }
}