using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;
using low_age_prototype_common.Extensions;

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

    public bool Opened { get; private set; } = true; // TODO connect with logic that tracks when allies or enemies
                                                     // end movement on any of the ascendable positions
    public IList<(IEnumerable<Vector2>, int)> LeveledPositions => LeveledLocalPositions
        .Select(x => (x.Item1.Select(y => y + Parent.EntityPrimaryPosition), x.Item2))
        .ToList();
    public IList<(IEnumerable<Vector2>, int)> LeveledLocalPositions { get; private set; } = new List<(IEnumerable<Vector2>, int)>();
    public Dictionary<Vector2, int> FlattenedPositions => FlattenedLocalPositions
        .ToDictionary(pair => pair.Key + Parent.EntityPrimaryPosition, pair => pair.Value);
    public Dictionary<Vector2, int> FlattenedLocalPositions { get; set; } = new Dictionary<Vector2, int>();
    
    private Ascendable Blueprint { get; set; }
    
    public void SetBlueprint(Ascendable blueprint)
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
    
    public bool CanBeMovedOnAt(Vector2 position, int team)
    {
        var isSameTeam = Blueprint.ClosingEnabled is false || (team == Parent.Team || Opened);
        return isSameTeam && FlattenedPositions.ContainsKey(position);
    }

    private void SetupPositions()
    {
        var startingRotation = ActorRotation.BottomRight;
        var finalRotation = Parent is ActorNode actor ? actor.ActorRotation : startingRotation;
        var rotationCount = startingRotation.CountTo(finalRotation);

        var leveledRectsBlueprint = Blueprint.Path.Select(step => 
            (step.Area.ToGodotRect2(), step.SpriteOffset.Y)).ToList();
        
        var entityBoundsBeforeRotation = finalRotation is ActorRotation.BottomLeft 
                                         || finalRotation is ActorRotation.TopRight 
            ? new Vector2(Parent.EntitySize.y, Parent.EntitySize.x) 
            : Parent.EntitySize;
        
        var rotatedRectsBlueprint = leveledRectsBlueprint.Select(entry => entry.Item1).ToList();
        rotatedRectsBlueprint = rotatedRectsBlueprint.RotateClockwiseInside(entityBoundsBeforeRotation, rotationCount)
            .ToList();
        
        var leveledPositions = new List<(IEnumerable<Vector2>, int)>();
        for (var i = 0; i < leveledRectsBlueprint.Count; i++)
        {
            var rect2 = rotatedRectsBlueprint[i];
            var positions = rect2.ToList();
            var height = leveledRectsBlueprint[i].Item2;
            leveledPositions.Add((positions, height));
            foreach (var position in positions)
            {
                FlattenedLocalPositions[position] = height;
            }
        }

        LeveledLocalPositions = leveledPositions;
    }
}
