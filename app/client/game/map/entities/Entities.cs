using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Factions;

/// <summary>
/// Parent of all entities (units & structures) and their rendering on the map.
/// </summary>
public class Entities : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    public event Action<EntityPlacedEvent> EntityPlaced = delegate { };
    public event Action<EntityNode> NewPositionOccupied = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action EntityDeselected = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityNode SelectedEntity { get; private set; } = null;
    public EntityNode EntityInPlacement { get; private set; } = null;
    public EntityNode HoveredEntity { get; private set; } = null;

    private bool _flattened = false;
    private EntityRenderers _renderers;
    private Node2D _units;
    private Node2D _structures;
    private Func<IList<Vector2>, IList<Tiles.TileInstance>> _getHighestTiles;
    private Func<Vector2, bool, Tiles.TileInstance> _getTile;

    private readonly System.Collections.Generic.Dictionary<Guid, EntityNode> _entitiesByIds = new System.Collections.Generic.Dictionary<Guid, EntityNode>();

    public override void _Ready()
    {
        _renderers = GetNode<EntityRenderers>(nameof(EntityRenderers));
        _units = GetNode<Node2D>("Units");
        _structures = GetNode<Node2D>("Structures");

        NewPositionOccupied += _renderers.UpdateSorting;
    }
    
    public void Initialize(Func<IList<Vector2>, IList<Tiles.TileInstance>> getHighestTiles, 
        Func<Vector2, bool, Tiles.TileInstance> getTile)
    {
        _getHighestTiles = getHighestTiles;
        _getTile = getTile;
    }

    public void SetupStartingEntities(IList<Vector2> startingPositions, FactionId factionId)
    {
        var startingEntities = Data.Instance.Blueprint.Factions.First(x => x.Id.Equals(factionId))
            .StartingEntities;

        for (var i = 0; i < startingPositions.Count; i++)
        {
            if (i >= startingEntities.Count)
                continue;

            var entityBlueprint = Data.Instance.GetEntityBlueprintById(startingEntities[i]);
            PlaceEntity(entityBlueprint, startingPositions[i]);
        }
    }
    
    public override void _ExitTree()
    {
        NewPositionOccupied -= _renderers.UpdateSorting;
        foreach (var unit in _units.GetChildren().OfType<UnitNode>())
        {
            unit.FinishedMoving -= OnEntityFinishedMoving;
            unit.Destroyed -= OnEntityDestroyed;
        }
        base._ExitTree();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed(Constants.Input.Rotate) && EntityInPlacement is ActorNode actor) 
            actor.Rotate();
        
        if (EntityMoving || EntityInPlacement != null)
            _renderers.UpdateSorting();
    }

    public EntityNode GetEntityByInstanceId(Guid instanceId) => _entitiesByIds.ContainsKey(instanceId)
        ? _entitiesByIds[instanceId]
        : null;

    public void AdjustGlobalPosition(EntityNode entity, Vector2 globalPosition) => entity.SnapTo(globalPosition);

    public void SelectEntity(EntityNode entity)
    {
        if (EntityMoving) 
            return;
        
        if (IsEntitySelected())
            SelectedEntity.SetSelected(false);

        SelectedEntity = entity;
        SelectedEntity.SetSelected(true);
        EntitySelected(entity);
    }

    public void DeselectEntity()
    {
        if (IsEntitySelected() is false) 
            return;
        
        SelectedEntity.SetSelected(false);
        SelectedEntity = null;
        EntityDeselected();
    }

    public bool IsEntitySelected() => SelectedEntity != null;

    public bool IsEntitySelected(EntityNode entity) => IsEntitySelected() 
                                                       && SelectedEntity.InstanceId == entity?.InstanceId;

    public bool IsEntityHovered() => HoveredEntity != null;

    public bool IsEntityHovered(EntityNode entity) => IsEntityHovered() 
                                                      && HoveredEntity.InstanceId == entity?.InstanceId;

    public bool TryHoveringEntityOn(Tiles.TileInstance tile)
    {
        if (EntityMoving)
            return false;

        var occupationExists = tile.Occupants.Any();
        var occupantEntity = tile.Occupants.LastOrDefault(); // TODO handle high-ground
        
        if (occupationExists && IsEntityHovered(occupantEntity))
            return true;

        HoveredEntity?.SetTileHovered(false);

        if (occupationExists)
        {
            occupantEntity?.SetTileHovered(true);
            HoveredEntity = occupantEntity;
            return true;
        }
        
        HoveredEntity = null;
        return false;
    }
    
    public EntityNode GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        EntityNode topEntity = null;

        var colliders = Colliders.GetAt(globalPosition, GetWorld2d());
        
        foreach (var collider in colliders)
        {
            if ((collider.GetParent().GetParent() is EntityNode entity) is false)
                continue;

            if (entity.Renderer.ContainsSpriteAt(globalPosition) is false)
                continue;

            if (entity.Renderer.ZIndex <= topZ) 
                continue;
            
            topZ = entity.Renderer.ZIndex;
            topEntity = entity;
        }
        
        if (DebugEnabled && topEntity != null)
            GD.Print($"{nameof(Entities)}.{nameof(GetTopEntity)}: entity found '{topEntity.DisplayName}'");

        return topEntity;
    }
    
    public void SetFlattened(bool to)
    {
        _flattened = to;
        foreach (var entity in _entitiesByIds.Values) 
            entity.SetFlattened(to);
        
        // TODO recalculate high ground offsets
    }
    
    public void MoveEntity(EntityNode entity, IEnumerable<Vector2> globalPath, ICollection<Point> path)
    {
        var targetPoint = path.Last();
        var startPoint = path.First();
        EntityMoving = true;
        entity.MoveUntilFinished(globalPath.ToList(), targetPoint);
    }
    
    public void RegisterRenderer(EntityNode entity)
    {
        _renderers.RegisterRenderer(entity.Renderer);
        _renderers.UpdateSorting();
    }

    public EntityNode SetEntityForPlacement(EntityId entityId, 
        bool canBePlacedOnTheWholeMap)
    {
        var newEntityBlueprint = Data.Instance.GetEntityBlueprintById(entityId);
        var newEntity = InstantiateEntity(newEntityBlueprint);
        
        newEntity.SetForPlacement(canBePlacedOnTheWholeMap);
        EntityInPlacement = newEntity;
        
        _renderers.RegisterRenderer(EntityInPlacement.Renderer);
        
        return newEntity;
    }

    public void UpdateEntityInPlacement(Vector2 mapPosition, Vector2 globalPosition)
    {
        EntityInPlacement.EntityPrimaryPosition = mapPosition;
        EntityInPlacement.SnapTo(globalPosition);
        EntityInPlacement.DeterminePlacementValidity(true);
    }

    public void CancelPlacement()
    {
        EntityInPlacement?.Destroy();
        EntityInPlacement = null;
    }

    public EntityNode PlaceEntity()
    {
        var entity = EntityInPlacement;
        EntityInPlacement = null;
        
        _renderers.UnregisterRenderer(entity.Renderer);
        if (entity is StructureNode)
            entity.Renderer.MakeStatic();
        
        entity.DeterminePlacementValidity(true);
        return PlaceEntity(entity, true);
    }

    public EntityNode PlaceEntity(EntityPlacedEvent @event)
    {
        var entity = GetEntityByInstanceId(@event.InstanceId);
        if (entity is null)
        {
            var entityBlueprint = Data.Instance.GetEntityBlueprintById(@event.BlueprintId);
            entity = InstantiateEntity(entityBlueprint, @event.InstanceId);
            entity.EntityPrimaryPosition = @event.MapPosition;
            entity.OverridePlacementValidity();
            if (entity is ActorNode actor)
                actor.SetActorRotation(@event.ActorRotation);
            // TODO this is getting quite extensive, think of a way to move the synchronization of entity state to be
            // handled inside the entity
        }

        return PlaceEntity(entity, false);
    }
    
    private EntityNode PlaceEntity(Entity entityBlueprint, Vector2 mapPosition)
    {
        var entity = InstantiateEntity(entityBlueprint);
        entity.EntityPrimaryPosition = mapPosition;
        entity.DeterminePlacementValidity(false);
        return PlaceEntity(entity, true);
    }

    private EntityNode PlaceEntity(EntityNode entity, bool placeAsCandidate)
    {
        var instanceId = entity.InstanceId;
        
        if (DebugEnabled) GD.Print($"{nameof(Entities)}: placing {entity.DisplayName} '{instanceId}' at " +
                                   $"{entity.EntityPrimaryPosition}.");

        if (TryPlaceEntity(entity, placeAsCandidate) is false)
            return null;
        
        if (placeAsCandidate)
            EntityPlaced(new EntityPlacedEvent(entity.BlueprintId, entity.EntityPrimaryPosition, instanceId, 
                entity is ActorNode actor ? actor.ActorRotation : ActorRotation.BottomRight));
        
        NewPositionOccupied(entity);
        
        return entity;
    }

    private static bool TryPlaceEntity(EntityNode entity, bool placeAsCandidate)
    {
        var placedSuccessfully = placeAsCandidate 
            ? entity.SetAsCandidate() 
            : entity.Place();
        
        return placedSuccessfully;
    }

    private EntityNode InstantiateEntity(Entity entityBlueprint, Guid? instanceId = null)
    {
        EntityNode entity;
        switch (entityBlueprint)
        {
            case Structure structure:
                entity = InstantiateStructure(structure);
                break;
            case Unit unit:
                entity = InstantiateUnit(unit);
                break;
            // TODO case Doodad doodad:
            default:
                throw new ArgumentOutOfRangeException($"No possible cast for entity '{entityBlueprint.Id}'");
        }

        entity.Destroyed += OnEntityDestroyed;

        if (instanceId != null)
            entity.InstanceId = (Guid)instanceId;
        _entitiesByIds[entity.InstanceId] = entity;
        
        entity.SetFlattened(_flattened);
        
        return entity;
    }

    private StructureNode InstantiateStructure(Structure structureBlueprint)
    {
        var structure = StructureNode.InstantiateAsChild(structureBlueprint, _structures);
        structure.GetHighestTiles = _getHighestTiles;
        structure.GetTile = _getTile;

        return structure;
    }

    private UnitNode InstantiateUnit(Unit unitBlueprint)
    {
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units);

        unit.GetHighestTiles = _getHighestTiles;
        unit.GetTile = _getTile;
        unit.FinishedMoving += OnEntityFinishedMoving;

        return unit;
    }

    private void OnEntityFinishedMoving(EntityNode entity)
    {
        EntityMoving = false;
        NewPositionOccupied(entity);
    }

    private void OnEntityDestroyed(EntityNode entity)
    {
        _renderers.UnregisterRenderer(entity.Renderer);
        
        entity.FinishedMoving -= OnEntityFinishedMoving;
        entity.Destroyed -= OnEntityDestroyed;

        if (_entitiesByIds.ContainsKey(entity.InstanceId))
            _entitiesByIds.Remove(entity.InstanceId);
    }
}
