using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;

public class HighGroundNode : BehaviourNode, INodeFromBlueprint<HighGround>, IPathfindingUpdatable
{
    public const string ScenePath = @"res://app/shared/game/behaviours/HighGroundNode.tscn";
    public static HighGroundNode Instance() => (HighGroundNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static HighGroundNode InstantiateAsChild(HighGround blueprint, Node parentNode, Effects history, 
        EntityNode parentEntity)
    {
        var behaviour = Instance();
        parentNode.AddChild(behaviour);
        behaviour.History = history;
        behaviour.Parent = parentEntity;
        behaviour.SetBlueprint(blueprint);
        return behaviour;
    }

    public List<(IList<Vector2>, int)> LeveledPositions { get; private set; } = new List<(IList<Vector2>, int)>();
    
    private HighGround Blueprint { get; set; }

    private HashSet<Vector2> FlattenedPositions { get; set; } = new HashSet<Vector2>();
    
    public void SetBlueprint(HighGround blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        
        SetupPositions();
        
        EventBus.Instance.RaisePathfindingUpdating(this, true);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.Instance.RaisePathfindingUpdating(this, false);
    }

    public bool CanBeMovedOnAt(Vector2 position) => FlattenedPositions.Any(x => x.IsEqualApprox(position));
    
    private void SetupPositions()
    {
        var primaryPosition = Parent.EntityPrimaryPosition;
        var outerCollection = new List<(IList<Vector2>, int)>();
        
        foreach (var step in Blueprint.HighGroundAreas)
        {
            // TODO factor in actor rotation
            var rect2 = step.Area.ToGodotRect2();
            rect2.Position += primaryPosition;
            var positions = rect2.ToList();
            outerCollection.Add((positions, step.SpriteOffset.Y));
            foreach (var pos in positions)
            {
                FlattenedPositions.Add(pos);
            }
        }

        LeveledPositions = outerCollection;
    }
}