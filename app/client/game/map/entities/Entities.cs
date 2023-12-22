using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Entities.Actors.Units;
using Array = Godot.Collections.Array;

/// <summary>
/// Parent of all entities (units & structures) on the map.
/// </summary>
public class Entities : YSort
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    public event Action<EntityNode> NewEntityFound = delegate { };
    public event Action<EntityNode> EntitySelected = delegate { };
    public event Action EntityDeselected = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityNode SelectedEntity { get; private set; } = null;

    private YSort _units;
    private Vector2 _hoveredEntityPosition = Vector2.Inf;
    private Godot.Collections.Dictionary<Vector2, EntityNode> _entitiesByMapPositions = new Godot.Collections.Dictionary<Vector2, EntityNode>();
    private Godot.Collections.Dictionary<EntityNode, Vector2> _mapPositionsByEntities = new Godot.Collections.Dictionary<EntityNode, Vector2>();

    public override void _Ready()
    {
        _units = GetNode<YSort>("Units");
    }
    
    public void Initialize()
    {
        // Force-add units for testing
        var slaveBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.Slave));
        var mapPositionsToSpawn = new List<Vector2> { new Vector2(30, 40), new Vector2(32, 48), 
            new Vector2(34, 46), new Vector2(35, 35), new Vector2(36, 38) };
        foreach (var mapPosition in mapPositionsToSpawn)
        {
            InitializeUnit(slaveBlueprint, mapPosition);
        }
        
        var horriorBlueprint = Data.Instance.Blueprint.Entities.Units.Single(x => x.Id.Equals(UnitId.Horrior));
        InitializeUnit(horriorBlueprint, new Vector2(39, 39));
    }

    private void InitializeUnit(Unit unitBlueprint, Vector2 mapPosition)
    {
        if (DebugEnabled) GD.Print($"{nameof(Entities)}: initializing {unitBlueprint.Id} at {mapPosition}.");
        
        var unit = UnitNode.InstantiateAsChild(unitBlueprint, _units);
        unit.EntityPosition = mapPosition;
        unit.SetSpriteOffset(new Vector2(1, -6)); // TODO should be taken from data -- new IDisplayable property
                                                         // TODO should also be taken from top-left origin instead of
                                                         // center so it's easier to work with image editors
            
        _entitiesByMapPositions[mapPosition] = unit;
        _mapPositionsByEntities[unit] = mapPosition;
            
        unit.FinishedMoving += OnEntityFinishedMoving;
        NewEntityFound(unit);
    }


    public override void _ExitTree()
    {
        foreach (var unit in _units.GetChildren().OfType<UnitNode>())
        {
            unit.FinishedMoving -= OnEntityFinishedMoving;
        }
        base._ExitTree();
    }

    public void AdjustGlobalPosition(EntityNode entity, Vector2 globalPosition)
    {
        entity.GlobalPosition = globalPosition;
    }

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

    public bool TryHoveringEntity(Vector2 at)
    {
        if (EntityMoving)
            return false;

        if (_hoveredEntityPosition.Equals(at))
            return true;

        if (_hoveredEntityPosition != Vector2.Inf)
        {
            var entity = _entitiesByMapPositions[_hoveredEntityPosition];
            entity.SetTileHovered(false);
        }

        if (_entitiesByMapPositions.ContainsKey(at))
        {
            var entity = _entitiesByMapPositions[at];
            entity.SetTileHovered(true);
            _hoveredEntityPosition = at;
            return true;
        }
        
        _hoveredEntityPosition = Vector2.Inf;
        return false;
    }
    
    public EntityNode GetHoveredEntity()
    {
        if (EntityMoving)
            return null;

        return _entitiesByMapPositions.TryGetValue(_hoveredEntityPosition, out var position) 
            ? position 
            : null;
    }

    public Vector2 GetMapPositionOfEntity(EntityNode entity) => _mapPositionsByEntities.ContainsKey(entity) 
        ? _mapPositionsByEntities[entity] 
        : Vector2.Inf;

    public EntityNode GetEntityFromMapPosition(Vector2 mapPosition) => _entitiesByMapPositions.ContainsKey(mapPosition)
        ? _entitiesByMapPositions[mapPosition]
        : null;

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
    
    // TODO: seems like each entity has z_index of 0 and so this method doesn't work
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

    private void OnEntityFinishedMoving(EntityNode entity)
    {
        EntityMoving = false;
    }
}
