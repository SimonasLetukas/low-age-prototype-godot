using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Array = Godot.Collections.Array;

/// <summary>
/// Parent of all entities (units & structures) on the map.
/// </summary>
public class Entities : YSort
{
    [Export] public bool DebugEnabled { get; set; } = true;
    
    public event Action<EntityLegacy> NewEntityFound = delegate { };

    public bool EntityMoving { get; private set; } = false;
    public EntityLegacy SelectedEntity { get; private set; } = null;

    private Vector2 _hoveredEntityPosition = Vector2.Inf;
    private Godot.Collections.Dictionary<Vector2, EntityLegacy> _entitiesByMapPositions = new Godot.Collections.Dictionary<Vector2, EntityLegacy>();
    private Godot.Collections.Dictionary<EntityLegacy, Vector2> _mapPositionsByEntities = new Godot.Collections.Dictionary<EntityLegacy, Vector2>();

    public void Initialize()
    {
        var units = GetNode<YSort>("Units");
        foreach (var unit in units.GetChildren().OfType<Node2D>())
        {
            if (DebugEnabled) GD.Print($"{nameof(Entities)}: initializing {unit.Name}.");

            var entityBase = unit.GetNode<EntityLegacy>("UnitBase/EntityBase");
            if (entityBase is null) 
                continue;

            entityBase.FinishedMoving += OnEntityFinishedMoving;
            NewEntityFound(entityBase);
        }
    }
    
    public override void _ExitTree()
    {
        // TODO foreach
        //entityBase.FinishedMoving -= OnEntityFinishedMoving;
        base._ExitTree();
    }

    public void RegisterEntity(Vector2 at, EntityLegacy entity)
    {
        _entitiesByMapPositions[at] = entity;
        _mapPositionsByEntities[entity] = at;
    }

    public void SelectEntity(EntityLegacy entity)
    {
        if (EntityMoving) 
            return;
        
        if (IsEntitySelected())
            SelectedEntity.SetSelected(false);

        SelectedEntity = entity;
        SelectedEntity.SetSelected(true);
    }

    public void DeselectEntity()
    {
        if (IsEntitySelected() is false) 
            return;
        
        SelectedEntity.SetSelected(false);
        SelectedEntity = null;
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

    public EntityLegacy GetHoveredEntity()
    {
        if (EntityMoving)
            return null;

        return _entitiesByMapPositions.ContainsKey(_hoveredEntityPosition) 
            ? _entitiesByMapPositions[_hoveredEntityPosition] 
            : null;
    }

    public Vector2 GetMapPositionOfEntity(EntityLegacy entity) => _mapPositionsByEntities.ContainsKey(entity) 
        ? _mapPositionsByEntities[entity] 
        : Vector2.Inf;

    public EntityLegacy GetEntityFromMapPosition(Vector2 mapPosition) => _entitiesByMapPositions.ContainsKey(mapPosition)
        ? _entitiesByMapPositions[mapPosition]
        : null;

    public void MoveEntity(EntityLegacy entity, ICollection<Vector2> globalPath, ICollection<Vector2> path)
    {
        var targetPosition = path.Last();
        var startPosition = path.First();
        _mapPositionsByEntities[entity] = targetPosition;
        _entitiesByMapPositions.Remove(startPosition);
        _entitiesByMapPositions[targetPosition] = entity;
        EntityMoving = true;
        entity.MoveUntilFinished(globalPath.ToList());
    }
    
    // TODO: seems like each entity has z_index of 0 and so this method doesn't work
    public EntityLegacy GetTopEntity(Vector2 globalPosition)
    {
        var topZ = float.NegativeInfinity;
        EntityLegacy topEntity = null;
        
        var intersections = GetWorld2d().DirectSpaceState.IntersectPoint(globalPosition, 32, 
            new Array(), 0x7FFFFFFF, true, true);

        foreach (var node in intersections.OfType<KinematicCollision2D>())
        {
            if ((node.Collider is Area2D area) is false 
                || (area.GetParent() is EntityLegacy entity) is false)
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

    private void OnEntityFinishedMoving(EntityLegacy entity)
    {
        EntityMoving = false;
    }
}
