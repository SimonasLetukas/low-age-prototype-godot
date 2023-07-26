using Godot;
using System;
using System.Collections.Generic;
using low_age_data.Domain.Shared;

public class ClientMap : Map
{
    public event Action<Vector2, Terrain> NewTileHovered = delegate { };
    [Signal] public delegate void UnitMovementIssued(Vector2 entityPosition, Vector2[] globalPath, Vector2[] path);

    private readonly ICollection<Vector2> _startingPositions = new List<Vector2>();
    private Vector2 _mapSize = Vector2.Inf;
    private Vector2 _tileHovered = Vector2.Zero;
    private Tiles _tileMap;
    private Entities _entities;
    private Data _data;

    public override void _Ready()
    {
        base._Ready();
        _tileMap = GetNode<Tiles>($"Visual/{nameof(Tiles)}");
        _entities = GetNode<Entities>($"Visual/{nameof(Entities)}");
        _data = Data.Instance;

        _entities.Connect(nameof(Entities.NewEntityFound), this, nameof(OnEntitiesNewEntityFound));
        _data.Connect(nameof(Data.Synchronised), this, nameof(OnDataSynchronized));
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

    public Entity GetHoveredEntity(Vector2 mousePosition, Vector2 mapPosition)
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
        EmitSignal(nameof(UnitMovementIssued), entityPosition, globalPath, path);
        HandleDeselecting();
    }

    public void HandleSelecting(Entity hoveredEntity)
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

        const float tempRange = 12.5f; // TODO
        var entityPosition = _entities.GetMapPositionOfEntity(hoveredEntity);
        var availableTiles = Pathfinding.GetAvailablePositions(entityPosition, tempRange);
        _tileMap.SetAvailableTiles(availableTiles);
        _entities.SelectEntity(hoveredEntity);
    }

    public void HandleDeselecting()
    {
        _tileMap.ClearAvailableTiles();
        _entities.DeselectEntity();
    }

    public void MoveUnit(Vector2 entityPosition, Vector2[] globalPath, Vector2[] path)
    {
        var selectedEntity = _entities.GetEntityFromMapPosition(entityPosition);
        _entities.MoveEntity(selectedEntity, globalPath, path);
    }
    
    private bool HoverTile()
    {
        var hoveredTerrain = _data.GetTerrain(_tileHovered);
        NewTileHovered(_tileHovered, hoveredTerrain);
        _tileMap.MoveFocusedTileTo(_tileHovered);
        var entityHovered = _entities.TryHoveringEntity(_tileHovered);
        return entityHovered;
    }

    private void OnDataSynchronized()
    {
        if (DebugEnabled) GD.Print($"{nameof(ClientMap)}.{nameof(OnDataSynchronized)}: event received.");

        OnMapCreatorMapSizeDeclared(_data.MapSize);
        
        for (var y = 0; y < _mapSize.y; y++)
        {
            for (var x = 0; x < _mapSize.x; x++)
            {
                var coordinates = new Vector2(x, y);
                var terrain = _data.GetTerrain(coordinates);
                _tileMap.SetCell(coordinates, terrain);
                if (terrain.Equals(Terrain.Marsh)
                    || terrain.Equals(Terrain.Mountains))
                {
                    Pathfinding.SetTerrainForPoint(coordinates, terrain);
                }
            }
        }
        
        OnMapCreatorGenerationEnded();
    }
    
    public void OnMapCreatorMapSizeDeclared(Vector2 mapSize)
    {
        _mapSize = mapSize;
        Position = new Vector2((Mathf.Max(_mapSize.x, mapSize.y) * Constants.TileWidth) / 2, Position.y);
        _tileMap.Initialize(_mapSize);
        Pathfinding.Initialize(_mapSize);
        _entities.Initialize();
    }

    private void OnMapCreatorGenerationEnded()
    {
        _tileMap.FillMapOutsideWithMountains();
        _tileMap.UpdateALlBitmaps();
        EmitSignal(nameof(StartingPositionsDeclared), _startingPositions);
    }

    private void OnMapCreatorStartingPositionFound(Vector2 coordinates)
    {
        _startingPositions.Add(coordinates);
    }

    private void OnEntitiesNewEntityFound(Entity entity)
    {
        var entityPosition = entity.GetGlobalTransform().origin;
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(entityPosition);
        _entities.RegisterEntity(mapPosition, entity);
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
