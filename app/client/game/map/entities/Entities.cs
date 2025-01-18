using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Factions;
using Array = Godot.Collections.Array;

/// <summary>
/// Parent of all entities (units & structures) and their rendering on the map.
/// </summary>
public partial class Entities : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    public event Action<EntityPlacedEvent> EntityPlaced = delegate { };
    public event Action<EntityNode> NewPositionOccupied = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action EntityDeselected = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityNode SelectedEntity { get; private set; } = null;
    public EntityNode EntityInPlacement { get; private set; } = null;
    public EntityNode HoveredEntity { get; private set; } = null;

    private EntityRenderers _renderers;
    private Node2D _units;
    private Node2D _structures;
    private Func<IList<Vector2>, IList<Tiles.TileInstance>> _getTiles;

    private readonly Dictionary<Guid, EntityNode> _entitiesByIds = new Dictionary<Guid, EntityNode>();

    public override void _Ready()
    {
        _renderers = GetNode<EntityRenderers>(nameof(EntityRenderers));
        _units = GetNode<Node2D>("Units");
        _structures = GetNode<Node2D>("Structures");

        NewPositionOccupied += _renderers.UpdateSorting;
    }
    
    public void Initialize(Func<IList<Vector2>, IList<Tiles.TileInstance>> getTiles)
    {
        _getTiles = getTiles;
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

    public bool TryHoveringEntityOn(Tiles.TileInstance tile)
    {
        if (EntityMoving)
            return false;

        var occupationExists = tile.Occupants.Any();
        var occupantEntity = tile.Occupants.LastOrDefault(); // TODO handle high-ground
        
        if (occupationExists && HoveredEntity != null && HoveredEntity.InstanceId.Equals(occupantEntity?.InstanceId))
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
    
    // TODO: seems like each entity has z_index of 0 and so this method doesn't work
    // This method is supposed to return the entity that has its sprite visible where the mouse is hovering at,
    // could be considered not required and annoying in the future -- need to make it work and test it out UX
    // with high-ground entities
    public EntityNode GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        EntityNode topEntity = null;
        
        var intersections = GetWorld2d().DirectSpaceState.IntersectPoint(globalPosition, 32, 
            new Array(), 0x7FFFFFFF, true, true);

        foreach (var node in intersections.OfType<KinematicCollision2D>())
        {
            if ((node.Collider is Area2D area) is false 
                || (area.GetParent() is EntityNode entity) is false)
                continue;

            if (entity.ZIndex <= topZ) 
                continue;
            
            topZ = entity.ZIndex;
            topEntity = entity;
        }

        return topEntity;
    }

    public static int GetAbsoluteZIndex(Node target)
    {
        var node = target as Node2D;
        var zIndex = 0;
        while (node != null && node.IsClass(nameof(Node2D)))
        {
            zIndex += node.ZIndex;
            if (node.ZAsRelative is false)
                break;

            node = node.GetParent() as Node2D;
        }

        return zIndex;
    }
    
    public void MoveEntity(EntityNode entity, IEnumerable<Vector2> globalPath, ICollection<Vector2> path)
    {
        var targetPosition = path.Last();
        var startPosition = path.First();
        EntityMoving = true;
        entity.MoveUntilFinished(globalPath.ToList(), targetPosition);
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

    public void UpdateEntityInPlacement(Vector2 mapPosition, Vector2 globalPosition, 
        Func<IList<Vector2>, IList<Tiles.TileInstance>> getTiles)
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
            entity = InstantiateEntity(entityBlueprint);
            entity.InstanceId = @event.InstanceId;
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
        
        _entitiesByIds[instanceId] = entity;
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

    private EntityNode InstantiateEntity(Entity entityBlueprint)
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
        
        return entity;
    }

    private StructureNode InstantiateStructure(Structure structureBlueprint)
    {
        var structure = StructureNode.InstantiateAsChild(structureBlueprint, _structures);
        structure.GetTiles = _getTiles;

        return structure;
    }

    private UnitNode InstantiateUnit(Unit unitBlueprint)
    {
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units);

        unit.GetTiles = _getTiles;
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
    }
}
