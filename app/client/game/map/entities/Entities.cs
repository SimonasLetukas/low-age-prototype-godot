using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using Array = Godot.Collections.Array;

/// <summary>
/// Parent of all entities (units & structures) on the map.
/// </summary>
public class Entities : YSort
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    public event Action<EntityNode> NewPositionOccupied = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action EntityDeselected = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityNode SelectedEntity { get; private set; } = null;
    public EntityNode EntityInPlacement { get; private set; } = null;
    public EntityNode HoveredEntity { get; private set; } = null;

    private YSort _units;
    private YSort _structures;
    private readonly Godot.Collections.Dictionary<Vector2, EntityNode> _entitiesByMapPositions = 
        new Godot.Collections.Dictionary<Vector2, EntityNode>();
    private readonly Godot.Collections.Dictionary<EntityNode, Vector2> _mapPositionsByEntities = 
        new Godot.Collections.Dictionary<EntityNode, Vector2>();

    public override void _Ready()
    {
        _units = GetNode<YSort>("Units");
        _structures = GetNode<YSort>("Structures");
    }
    
    public void Initialize()
    {
        // Force-add units for testing
        var slaveBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.Slave));
        var mapPositionsToSpawn = new List<Vector2> { new Vector2(30, 40), new Vector2(32, 48), 
            new Vector2(34, 46), new Vector2(35, 35), new Vector2(36, 38), new Vector2(1, 13) };
        foreach (var mapPosition in mapPositionsToSpawn)
        {
            PlaceEntity(slaveBlueprint, mapPosition);
        }
        
        var horriorBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.Horrior));
        PlaceEntity(horriorBlueprint, new Vector2(39, 39));
        
        var marksmanBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.Marksman));
        PlaceEntity(marksmanBlueprint, new Vector2(38, 42));
        
        var bigBadBullBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.BigBadBull));
        PlaceEntity(bigBadBullBlueprint, new Vector2(38, 46));
        
        var obeliskBlueprint = Data.Instance.Blueprint.Entities.Structures.Single(x => x.Id.Equals(StructureId.Obelisk));
        PlaceEntity(obeliskBlueprint, new Vector2(31, 36));
        
        var citadelBlueprint = Data.Instance.Blueprint.Entities.Structures.Single(x => x.Id.Equals(StructureId.Citadel));
        PlaceEntity(citadelBlueprint, new Vector2(33, 41));
    }
    
    public override void _ExitTree()
    {
        foreach (var unit in _units.GetChildren().OfType<UnitNode>())
        {
            unit.FinishedMoving -= OnEntityFinishedMoving;
        }
        base._ExitTree();
    }
    
    public Vector2 GetMapPositionOfEntity(EntityNode entity) => _mapPositionsByEntities.ContainsKey(entity) 
        ? _mapPositionsByEntities[entity] 
        : Vector2.Inf;

    public EntityNode GetEntityFromMapPosition(Vector2 mapPosition) => _entitiesByMapPositions.ContainsKey(mapPosition)
        ? _entitiesByMapPositions[mapPosition]
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
        var occupantEntity = tile.Occupants.FirstOrDefault(); // TODO handle high-ground
        
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
    
    public void MoveEntity(EntityNode entity, ICollection<Vector2> globalPath, ICollection<Vector2> path)
    {
        var targetPosition = path.Last();
        var startPosition = path.First();
        _mapPositionsByEntities[entity] = targetPosition;
        _entitiesByMapPositions.Remove(startPosition);
        _entitiesByMapPositions[targetPosition] = entity;
        EntityMoving = true;
        entity.MoveUntilFinished(globalPath.ToList(), targetPosition);
    }
    
    public EntityNode SetEntityForPlacement(EntityId entityId)
    {
        var newEntityBlueprint = Data.Instance.GetEntityBlueprintById(entityId);
        var newEntity = InstantiateEntity(newEntityBlueprint);
        
        newEntity.SetForPlacement(true);
        EntityInPlacement = newEntity;
        
        return newEntity;
    }

    public void UpdateEntityInPlacement(Vector2 mapPosition, Vector2 globalPosition, 
        Func<Rect2, IList<Tiles.TileInstance>> getTiles)
    {
        EntityInPlacement.EntityPosition = mapPosition;
        EntityInPlacement.SnapTo(globalPosition);
        EntityInPlacement.DeterminePlacementValidity(getTiles);
    }

    public void CancelPlacement()
    {
        EntityInPlacement?.QueueFree();
        EntityInPlacement = null;
    }

    public EntityNode PlaceEntity()
    {
        var entity = EntityInPlacement;
        EntityInPlacement = null;
        return PlaceEntity(entity);
    }
    
    private EntityNode PlaceEntity(Entity entityBlueprint, Vector2 mapPosition)
    {
        var entity = InstantiateEntity(entityBlueprint);
        return PlaceEntity(entity, mapPosition);
    }

    private EntityNode PlaceEntity(EntityNode entity, Vector2 mapPosition)
    {
        entity.EntityPosition = mapPosition;
        return PlaceEntity(entity);
    }

    private EntityNode PlaceEntity(EntityNode entity)
    {
        var position = entity.EntityPosition;
        
        if (DebugEnabled) GD.Print($"{nameof(Entities)}: placing {entity.DisplayName} at {position}.");
        
        entity.Place();
        
        _entitiesByMapPositions[position] = entity;
        _mapPositionsByEntities[entity] = position;
        
        NewPositionOccupied(entity);
        return entity;
    }

    private EntityNode InstantiateEntity(Entity entityBlueprint)
    {
        switch (entityBlueprint)
        {
            case Structure structure:
                return InstantiateStructure(structure);
            case Unit unit:
                return InstantiateUnit(unit);
            // TODO case Doodad doodad:
            default:
                throw new ArgumentOutOfRangeException($"No possible cast for entity '{entityBlueprint.Id}'");
        }
    }

    private StructureNode InstantiateStructure(Structure structureBlueprint)
    {
        var structure = StructureNode.InstantiateAsChild(structureBlueprint, _structures);

        return structure;
    }

    private UnitNode InstantiateUnit(Unit unitBlueprint)
    {
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units);
        
        unit.FinishedMoving += OnEntityFinishedMoving;

        return unit;
    }

    private void OnEntityFinishedMoving(EntityNode entity)
    {
        EntityMoving = false;
        NewPositionOccupied(entity);
    }
}
