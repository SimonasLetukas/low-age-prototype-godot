using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Behaviours;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using multipurpose_pathfinding;

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
    public IList<(IEnumerable<Vector2<int>>, int)> LeveledPositions => LeveledLocalPositions
        .Select(x => (x.Item1.Select(y => y + Parent.EntityPrimaryPosition), x.Item2))
        .ToList();
    public IList<IEnumerable<Vector2<int>>> LeveledPositionsWithoutSpriteOffset => LeveledLocalPositions
        .Select(x => x.Item1.Select(y => y + Parent.EntityPrimaryPosition))
        .ToList();
    public IList<(IEnumerable<Vector2<int>>, int)> LeveledLocalPositions { get; private set; } = new List<(IEnumerable<Vector2<int>>, int)>();
    public Dictionary<Vector2<int>, int> FlattenedPositions => FlattenedLocalPositions
        .ToDictionary(pair => pair.Key + Parent.EntityPrimaryPosition, pair => pair.Value);
    public IEnumerable<Vector2<int>> FlattenedPositionsWithoutSpriteOffset => FlattenedLocalPositions
        .Select(x => x.Key + Parent.EntityPrimaryPosition);
    public Dictionary<Vector2<int>, int> FlattenedLocalPositions { get; set; } = new Dictionary<Vector2<int>, int>();
    
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
    
    public bool CanBeMovedOnAt(Vector2<int> position, Team forTeam)
    {
        return FlattenedPositions.ContainsKey(position);
    }

    public bool AllowsConnectionBetweenPoints(Point fromPoint, Point toPoint, Team forTeam)
    {
        var isSameTeam = Blueprint.ClosingEnabled is false || (forTeam == Parent.Team || Opened);
        
        return FlattenedPositions.ContainsKey(toPoint.Position)
               && ((isSameTeam && fromPoint.IsLowGround) || fromPoint.IsHighGround);
    }

    private void SetupPositions()
    {
        var startingRotation = ActorRotation.BottomRight;
        var finalRotation = Parent is ActorNode actor ? actor.ActorRotation : startingRotation;
        var rotationCount = startingRotation.CountTo(finalRotation);

        var leveledRectsBlueprint = Blueprint.Path.Select(step => 
            (step.Area, step.SpriteOffset.Y)).ToList();
        
        var entityBoundsBeforeRotation = finalRotation is ActorRotation.BottomLeft 
                                         || finalRotation is ActorRotation.TopRight 
            ? new Vector2<int>(Parent.EntitySize.Y, Parent.EntitySize.X) 
            : Parent.EntitySize;
        
        var rotatedRectsBlueprint = leveledRectsBlueprint.Select(entry => entry.Item1).ToList();
        rotatedRectsBlueprint = rotatedRectsBlueprint.RotateClockwiseInside(entityBoundsBeforeRotation, rotationCount)
            .ToList();
        
        var leveledPositions = new List<(IEnumerable<Vector2<int>>, int)>();
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
