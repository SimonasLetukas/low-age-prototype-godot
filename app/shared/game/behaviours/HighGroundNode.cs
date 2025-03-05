using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Behaviours;
using LowAgeCommon;
using LowAgeCommon.Extensions;
using MultipurposePathfinding;

public partial class HighGroundNode : BehaviourNode, INodeFromBlueprint<HighGround>, IPathfindingUpdatable
{
    public const string ScenePath = @"res://app/shared/game/behaviours/HighGroundNode.tscn";
    public static HighGroundNode Instance() => (HighGroundNode) GD.Load<PackedScene>(ScenePath).Instantiate();
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
    
    public IList<(IEnumerable<Vector2Int>, int)> LeveledPositions => LeveledLocalPositions
        .Select(x => (x.Item1.Select(y => y + Parent.EntityPrimaryPosition), x.Item2))
        .ToList();
    public IList<IEnumerable<Vector2Int>> LeveledPositionsWithoutSpriteOffset => LeveledLocalPositions
        .Select(x => x.Item1.Select(y => y + Parent.EntityPrimaryPosition))
        .ToList();
    public IList<(IEnumerable<Vector2Int>, int)> LeveledLocalPositions { get; private set; } = new List<(IEnumerable<Vector2Int>, int)>();
    public Dictionary<Vector2Int, int> FlattenedPositions => FlattenedLocalPositions
        .ToDictionary(pair => pair.Key + Parent.EntityPrimaryPosition, pair => pair.Value);
    public IEnumerable<Vector2Int> FlattenedPositionsWithoutSpriteOffset => FlattenedLocalPositions
        .Select(x => x.Key + Parent.EntityPrimaryPosition);
    public Dictionary<Vector2Int, int> FlattenedLocalPositions { get; set; } = new();

    private HighGround Blueprint { get; set; } = null!;

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

    public bool CanBeMovedOnAt(Vector2Int position, Team forTeam) => FlattenedPositions.ContainsKey(position);

    public bool AllowsConnectionBetweenPoints(Point fromPoint, Point toPoint, Team forTeam) => true;
    
    private void SetupPositions()
    {
        var startingRotation = ActorRotation.BottomRight;
        var finalRotation = Parent is ActorNode actor ? actor.ActorRotation : startingRotation;
        var rotationCount = startingRotation.CountTo(finalRotation);

        var leveledRectsBlueprint = Blueprint.HighGroundAreas.Select(step => 
            (step.Area, step.SpriteOffset.Y)).ToList();
        
        var entityBoundsBeforeRotation = finalRotation is ActorRotation.BottomLeft 
                                         || finalRotation is ActorRotation.TopRight 
            ? new Vector2Int(Parent.EntitySize.Y, Parent.EntitySize.X) 
            : Parent.EntitySize;
        
        var rotatedRectsBlueprint = leveledRectsBlueprint.Select(entry => entry.Item1).ToList();
        rotatedRectsBlueprint = rotatedRectsBlueprint.RotateClockwiseInside(entityBoundsBeforeRotation, rotationCount)
            .ToList();
        
        var leveledPositions = new List<(IEnumerable<Vector2Int>, int)>();
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