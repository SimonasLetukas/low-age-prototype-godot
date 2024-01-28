using Godot;
using System;
using System.Collections.Generic;
using low_age_data.Domain.Common;
using low_age_data.Domain.Entities;

public class ClientMap : Map
{
    public event Action FinishedInitializing = delegate { };
    public event Action<Vector2, Terrain> NewTileHovered = delegate { };
    public event Action<UnitMovedAlongPathEvent> UnitMovementIssued = delegate { };

    public Entities Entities { get; private set; }
    
    private ICollection<Vector2> _startingPositions = new List<Vector2>();
    private Vector2 _mapSize = Vector2.Inf;
    private Vector2 _hoveredTile = Vector2.Zero;
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

        Entities.NewEntityFound += OnEntitiesNewEntityFound;
    }

    public override void _Process(float delta)
    {
        // TODO when in placement or target mode, should still be able to hover other actors
        
        base._Process(delta);
        var mousePosition = GetGlobalMousePosition();
        var mapPosition = _tileMap.GetMapPositionFromGlobalPosition(mousePosition);
        UpdateHoveredTile(mapPosition);

        if (_selectionOverlay is SelectionOverlay.None)
        {
            GetHoveredEntity(mousePosition);
        }

        if (_selectionOverlay is SelectionOverlay.Movement)
        {
            GetHoveredEntity(mousePosition);

            // TODO optimization: only if hovered tile changed from above, display path
            var path = Pathfinding.FindPath(_hoveredTile);
            _tileMap.SetPathTiles(path);
        }
    }
    
    public override void _ExitTree()
    {
        Entities.NewEntityFound -= OnEntitiesNewEntityFound;
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

    public EntityNode GetHoveredEntity(Vector2 mousePosition)
    {
        var entity = Entities.GetTopEntity(mousePosition);

        if (entity != null)
        {
            var entityMapPosition = Entities.GetMapPositionOfEntity(entity);
            if (_hoveredTile == entityMapPosition) 
                return entity;
            
            UpdateHoveredTile(entityMapPosition);
            return entity;
        }

        var entityWasHovered = Entities.TryHoveringEntity(_hoveredTile);
        if (entityWasHovered)
        {
            entity = Entities.GetHoveredEntity();
        }

        return entity;
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
            : GetHoveredEntity(mousePosition);
        
        if (entity is null)
        {
            HandleDeselecting();
            return;
        }
        
        if (entity is UnitNode unit)
        {
            var entityPosition = Entities.GetMapPositionOfEntity(entity);
            var availableTiles = Pathfinding.GetAvailablePositions(entityPosition, unit.Movement);
            _tileMap.SetAvailableTiles(availableTiles);
        }
        
        Entities.SelectEntity(entity);
        _selectionOverlay = SelectionOverlay.Movement;
    }

    private void ExecutePlacement()
    {
        // TODO place entity that was set for placement
        ExecuteCancellation();
    }
    
    private void HandleRightClick()
    {
        if (_hoveredTile.IsInBoundsOf(_mapSize) is false)
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
        
        var path = Pathfinding.FindPath(_hoveredTile);
        var globalPath = _tileMap.GetGlobalPositionsFromMapPositions(path);
        var selectedEntity = Entities.SelectedEntity;
        var entityPosition = Entities.GetMapPositionOfEntity(selectedEntity);
        UnitMovementIssued(new UnitMovedAlongPathEvent(entityPosition, globalPath, path));
        HandleDeselecting();
    }

    private void ExecuteCancellation()
    {
        _tileMap.ClearTargetTiles();
        // TODO hide any entities that were set for placement
        ExecuteEntitySelection(true);
    }

    private void UpdateHoveredTile(Vector2 mapPosition)
    {
        if (_hoveredTile == mapPosition)
            return;
        
        // TODO handle hovered tile for entities bigger than 1x1 (2x2, 3x2...)
        _hoveredTile = mapPosition;
        var hoveredTerrain = _tileMap.GetTerrain(_hoveredTile);
        NewTileHovered(_hoveredTile, hoveredTerrain);
        _tileMap.MoveFocusedTileTo(_hoveredTile);
    }

    private Vector2 GetMapPositionFromMousePosition() 
        => _tileMap.GetMapPositionFromGlobalPosition(GetGlobalMousePosition());

    private void OnEntitiesNewEntityFound(EntityNode entity)
    {
        // TODO change naming and intention of this method to fit not only new entities but also those in placement
        var globalPosition = _tileMap.GetGlobalPositionFromMapPosition(entity.EntityPosition);
        Entities.AdjustGlobalPosition(entity, globalPosition);
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
        _tileMap.ClearAvailableTiles();
        _tileMap.ClearPath();
        
        // TODO add full list of arguments to ToPositions call (get the entity->actor from entityId)
        _tileMap.SetTargetTiles(buildAbility.PlacementArea.ToPositions(
            Entities.SelectedEntity.EntityPosition, 
            _mapSize));
        
        _selectionOverlay = SelectionOverlay.Placement;
        
        // TODO create structure for tiles to know the list of entities that occupies it, and implement adding and removing entities from it 
        // TODO create new entity, set it for placement, make sure placement color is determined in process (feed tiles to the actor so it decides availability)
        // TODO upon creation the new entity has to go through all of its passives and add on birth behaviours
        // TODO during placement the method inside entity makes sure that all build behaviours are correct, otherwise returns false and shows red placement
    }
}