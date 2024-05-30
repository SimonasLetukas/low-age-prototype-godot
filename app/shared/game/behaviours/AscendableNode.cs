using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;

public class AscendableNode : BehaviourNode, INodeFromBlueprint<Ascendable>, IPathfindingUpdatable
{
    public const string ScenePath = @"res://app/shared/game/behaviours/AscendableNode.tscn";
    public static AscendableNode Instance() => (AscendableNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static AscendableNode InstantiateAsChild(Ascendable blueprint, Node parentNode, Effects history, 
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
    
    private Ascendable Blueprint { get; set; }

    private HashSet<Vector2> FlattenedPositions { get; set; } = new HashSet<Vector2>();
    
    public void SetBlueprint(Ascendable blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;

        SetupPositions();
        
        EventBus.Instance.RaisePathfindingUpdated(this, true);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.Instance.RaisePathfindingUpdated(this, false);
    }
    
    public bool CanBeMovedOnAt(Vector2 position) => FlattenedPositions.Any(x => x.IsEqualApprox(position));

    private void SetupPositions()
    {
        var primaryPosition = Parent.EntityPrimaryPosition;
        var outerCollection = new List<(IList<Vector2>, int)>();
        
        foreach (var step in Blueprint.Path)
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
