using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Entities.Actors.Structures;
using LowAgeData.Domain.Entities.Actors.Units;
using LowAgeData.Domain.Factions;
using LowAgeCommon;
using MultipurposePathfinding;

/// <summary>
/// Parent of all entities (units & structures) and their rendering on the map.
/// </summary>
public partial class Entities : Node2D
{
    [Export] public bool DebugEnabled { get; set; } = false;
    
    public event Action<EntityPlacedEvent> EntityPlaced = delegate { };
    public event Action<EntityNode> NewPositionOccupied = delegate { };
    public event Action<EntityNode> Destroyed = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action EntityDeselected = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityNode? SelectedEntity { get; private set; } = null;
    public EntityNode? EntityInPlacement { get; private set; } = null;
    public EntityNode? HoveredEntity { get; private set; } = null;

    private EntityRenderers _renderers = null!;
    private Node2D _units = null!;
    private Node2D _structures = null!;
    private Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> _getHighestTiles = null!;
    private Func<Vector2Int, bool, Tiles.TileInstance?> _getTile = null!;

    private readonly Dictionary<Guid, EntityNode> _entitiesByIds = new();

    public override void _Ready()
    {
        _renderers = GetNode<EntityRenderers>(nameof(EntityRenderers));
        _units = GetNode<Node2D>("Units");
        _structures = GetNode<Node2D>("Structures");

        NewPositionOccupied += _renderers.UpdateSorting;
    }
    
    public void Initialize(Func<IList<Vector2Int>, IList<Tiles.TileInstance?>> getHighestTiles, 
        Func<Vector2Int, bool, Tiles.TileInstance?> getTile)
    {
        _getHighestTiles = getHighestTiles;
        _getTile = getTile;
    }

    public void SetupStartingEntities(IList<Vector2Int> startingPositions, FactionId factionId)
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

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(Constants.Input.Rotate) && EntityInPlacement is ActorNode actor) 
            actor.Rotate();
        
        if (EntityMoving || EntityInPlacement != null)
            _renderers.UpdateSorting();
    }

    public EntityNode? GetEntityByInstanceId(Guid instanceId) => _entitiesByIds.ContainsKey(instanceId)
        ? _entitiesByIds[instanceId]
        : null;

    public void AdjustGlobalPosition(EntityNode entity, Vector2 globalPosition) => entity.SnapTo(globalPosition);

    public void SelectEntity(EntityNode entity)
    {
        if (EntityMoving) 
            return;
        
        if (IsEntitySelected())
            SelectedEntity!.SetSelected(false);

        SelectedEntity = entity;
        SelectedEntity.SetSelected(true);
        EntitySelected(entity);
    }

    public void DeselectEntity()
    {
        if (IsEntitySelected() is false) 
            return;
        
        SelectedEntity!.SetSelected(false);
        SelectedEntity = null;
        EntityDeselected();
    }

    public bool IsEntitySelected() => SelectedEntity != null;

    public bool IsEntitySelected(EntityNode entity) => IsEntitySelected() 
                                                       && SelectedEntity!.InstanceId == entity.InstanceId;

    public bool IsEntityHovered() => HoveredEntity is { IsBeingDestroyed: false };

    public bool IsEntityHovered(EntityNode entity) => IsEntityHovered() 
                                                      && HoveredEntity!.InstanceId == entity.InstanceId;

    public bool TryHoveringEntityOn(Tiles.TileInstance tile)
    {
        if (EntityMoving)
            return false;

        var occupationExists = tile.Occupants.Any();
        var occupantEntity = tile.Occupants.LastOrDefault(); // TODO handle high-ground
        
        if (occupationExists && IsEntityHovered(occupantEntity!))
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
    
    public EntityNode? GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        EntityNode? topEntity = null;

        var colliders = Colliders.GetAt(globalPosition, GetWorld2D());
        
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
        var playerId = Players.Instance.Current.Id;
        var newEntityBlueprint = Data.Instance.GetEntityBlueprintById(entityId);
        var newEntity = InstantiateEntity(newEntityBlueprint, playerId);
        
        newEntity.SetForPlacement(canBePlacedOnTheWholeMap);
        EntityInPlacement = newEntity;
        
        _renderers.RegisterRenderer(EntityInPlacement.Renderer);
        
        return newEntity;
    }

    public void UpdateEntityInPlacement(Vector2Int mapPosition, Vector2 globalPosition)
    {
        if (EntityInPlacement is null)
            return;
        
        EntityInPlacement.EntityPrimaryPosition = mapPosition;
        EntityInPlacement.SnapTo(globalPosition);
        EntityInPlacement.DeterminePlacementValidity(true);
    }

    public void CancelPlacement()
    {
        EntityInPlacement?.Destroy();
        EntityInPlacement = null;
    }

    public void HandleEvent(EntityAttackedEvent @event)
    {
        var source = GetEntityByInstanceId(@event.SourceId);
        var target = GetEntityByInstanceId(@event.TargetId);

        if (source is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(EntityAttackedEvent)} because " +
                     $"{nameof(source)} '{@event.SourceId}' entity was null.");
            return;
        }
        
        if (target is null)
        {
            GD.Print($"{nameof(Entities)} could not apply {nameof(EntityAttackedEvent)} because " +
                     $"{nameof(target)} '{@event.TargetId}' entity was null.");
            return;
        }

        target.ReceiveAttack(source, @event.AttackType, false);
    }

    public void HandleEvent(EntityPlacedEvent @event)
    {
        var entity = GetEntityByInstanceId(@event.InstanceId);
        if (entity != null)
        {
            PlaceEntity(entity, false);
            return;
        }
        
        var entityBlueprint = Data.Instance.GetEntityBlueprintById(@event.BlueprintId);
        entity = InstantiateEntity(entityBlueprint, @event.PlayerId, @event.InstanceId);
        entity.ForcePlace(@event);
        
        NewPositionOccupied(entity);
    }

    public EntityNode? PlaceEntity()
    {
        if (EntityInPlacement is null)
            return null;
        
        var entity = EntityInPlacement;
        EntityInPlacement = null;
        
        _renderers.UnregisterRenderer(entity.Renderer);
        if (entity is StructureNode)
            entity.Renderer.MakeStatic();
        
        entity.DeterminePlacementValidity(true);
        return PlaceEntity(entity, true);
    }
    
    private EntityNode? PlaceEntity(Entity entityBlueprint, Vector2Int mapPosition)
    {
        var playerId = Players.Instance.Current.Id;
        var entity = InstantiateEntity(entityBlueprint, playerId);
        entity.EntityPrimaryPosition = mapPosition;
        entity.DeterminePlacementValidity(false);
        return PlaceEntity(entity, true);
    }

    private EntityNode? PlaceEntity(EntityNode entity, bool placeAsCandidate)
    {
        var instanceId = entity.InstanceId;
        
        if (DebugEnabled) GD.Print($"{nameof(Entities)}: placing {entity.DisplayName} '{instanceId}' at " +
                                   $"{entity.EntityPrimaryPosition}.");

        if (TryPlaceEntity(entity, placeAsCandidate) is false)
            return null;
        
        if (placeAsCandidate)
            EntityPlaced(new EntityPlacedEvent
            {
                BlueprintId = entity.BlueprintId,
                MapPosition = entity.EntityPrimaryPosition,
                InstanceId = instanceId,
                ActorRotation = entity is ActorNode actor ? actor.ActorRotation : ActorRotation.BottomRight,
                PlayerId = entity.Player.Id
            });
        
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

    private EntityNode InstantiateEntity(Entity entityBlueprint, int playerId, Guid? instanceId = null)
    {
        var player = Players.Instance.Get(playerId);

        EntityNode entity = entityBlueprint switch
        {
            Structure structure => InstantiateStructure(structure, player),
            Unit unit => InstantiateUnit(unit, player),
            _ => throw new ArgumentOutOfRangeException($"No possible cast for entity '{entityBlueprint.Id}'")
        };

        entity.Destroyed += OnEntityDestroyed;

        if (instanceId != null)
            entity.InstanceId = (Guid)instanceId;
        _entitiesByIds[entity.InstanceId] = entity;
        
        return entity;
    }

    private StructureNode InstantiateStructure(Structure structureBlueprint, Player player)
    {
        var structure = StructureNode.InstantiateAsChild(structureBlueprint, _structures, player, 
            _getTile, _getHighestTiles);

        return structure;
    }

    private UnitNode InstantiateUnit(Unit unitBlueprint, Player player)
    {
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units, player, 
            _getTile, _getHighestTiles);

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
        Destroyed(entity);
        
        _renderers.UnregisterRenderer(entity.Renderer);
        
        entity.FinishedMoving -= OnEntityFinishedMoving;
        entity.Destroyed -= OnEntityDestroyed;

        _entitiesByIds.Remove(entity.InstanceId);
    }
}
