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
        var startingRotation = ActorRotation.BottomRight;
        var finalRotation = Parent is ActorNode actor ? actor.ActorRotation : startingRotation;
        var rotationCount = startingRotation.CountTo(finalRotation);

        var leveledRectsBlueprint = Blueprint.HighGroundAreas.Select(step => 
            (step.Area.ToGodotRect2(), step.SpriteOffset.Y)).ToList();
        
        var entityBoundsBeforeRotation = finalRotation is ActorRotation.BottomLeft 
                                         || finalRotation is ActorRotation.TopRight 
            ? new Vector2(Parent.EntitySize.y, Parent.EntitySize.x) 
            : Parent.EntitySize;
        
        var rotatedRectsBlueprint = leveledRectsBlueprint.Select(entry => entry.Item1).ToList();
        rotatedRectsBlueprint = rotatedRectsBlueprint.RotateClockwiseInside(entityBoundsBeforeRotation, rotationCount)
            .ToList();
        
        var primaryPosition = Parent.EntityPrimaryPosition;
        
        var leveledPositions = new List<(IList<Vector2>, int)>();
        for (var i = 0; i < leveledRectsBlueprint.Count; i++)
        {
            var rect2 = rotatedRectsBlueprint[i];
            rect2.Position += primaryPosition;
            var positions = rect2.ToList();
            var height = leveledRectsBlueprint[i].Item2;
            leveledPositions.Add((positions, height));
            foreach (var position in positions)
            {
                FlattenedPositions.Add(position);
            }
        }

        LeveledPositions = leveledPositions;
    }
}