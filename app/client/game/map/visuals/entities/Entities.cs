using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Array = Godot.Collections.Array;

/// <summary>
/// Parent of all entities (units & structures) on the map.
/// </summary>
public class Entities : YSort
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    [Signal] public delegate void NewEntityFound(Entity entity);

    private Vector2 _hoveredEntityPosition = Vector2.Inf;
    private Entity _selectedEntity = null;
    private bool _entityMoving = false;
    private Godot.Collections.Dictionary<Vector2, Entity> _entitiesByMapPositions = new Godot.Collections.Dictionary<Vector2, Entity>();
    private Godot.Collections.Dictionary<Entity, Vector2> _mapPositionsByEntities = new Godot.Collections.Dictionary<Entity, Vector2>();

    public void Initialize()
    {
        var units = GetNode<YSort>("Units");
        foreach (UnitBase unit in units.GetChildren())
        {
            if (DebugEnabled) GD.Print($"{nameof(Entities)}: initializing {unit.Name}.");

            var entityBase = unit.GetNode<Entity>("UnitBase/EntityBase");
            if (entityBase is null) 
                continue;

            entityBase.Connect(nameof(Entity.FinishedMoving), this, nameof(OnEntityFinishedMoving));
            EmitSignal(nameof(NewEntityFound), entityBase);
        }
    }

    public void RegisterEntity(Vector2 at, Entity entity)
    {
        _entitiesByMapPositions[at] = entity;
        _mapPositionsByEntities[entity] = at;
    }

    public void SelectEntity(Entity entity)
    {
        if (_entityMoving) 
            return;
        
        if (IsEntitySelected())
            _selectedEntity.SetSelected(false);

        _selectedEntity = entity;
        _selectedEntity.SetSelected(true);
    }

    public void DeselectEntity(Entity entity)
    {
        if (IsEntitySelected() is false) 
            return;
        
        _selectedEntity.SetSelected(false);
        _selectedEntity = null;
    }

    public bool IsEntitySelected() => _selectedEntity != null;

    public bool TryHoveringEntity(Vector2 at)
    {
        if (_entityMoving)
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

    public Entity GetHoveredEntity()
    {
        if (_entityMoving)
            return null;

        return _entitiesByMapPositions.ContainsKey(_hoveredEntityPosition) 
            ? _entitiesByMapPositions[_hoveredEntityPosition] 
            : null;
    }

    public Vector2 GetMapPositionOfEntity(Entity entity) => _mapPositionsByEntities.ContainsKey(entity) 
        ? _mapPositionsByEntities[entity] 
        : Vector2.Inf;

    public Entity GetEntityFromMapPosition(Vector2 mapPosition) => _entitiesByMapPositions.ContainsKey(mapPosition)
        ? _entitiesByMapPositions[mapPosition]
        : null;

    public void MoveEntity(Entity entity, Vector2[] globalPath, Vector2[] path)
    {
        var targetPosition = path.Last();
        var startPosition = path.First();
        _mapPositionsByEntities[entity] = targetPosition;
        _entitiesByMapPositions.Remove(startPosition);
        _entitiesByMapPositions[targetPosition] = entity;
        _entityMoving = true;
        entity.MoveUntilFinished(globalPath);
    }
    
    // TODO: seems like each entity has z_index of 0 and so this method doesn't work
    public Entity GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        Entity topEntity = null;
        
        var intersections = GetWorld2d().DirectSpaceState.IntersectPoint(globalPosition, 32, 
            new Array(), 0x7FFFFFFF, true, true);

        foreach (KinematicCollision2D node in intersections)
        {
            if ((node.Collider is Area2D area) is false 
                || (area.GetParent() is Entity entity) is false)
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

    private void OnEntityFinishedMoving(Entity entity)
    {
        _entityMoving = false;
    }
}
