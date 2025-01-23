using System.Collections.Generic;
using System.Linq;
using Godot;
using LowAgeData.Domain.Entities.Actors.Structures;
using low_age_prototype_common;
using low_age_prototype_common.Extensions;
using multipurpose_pathfinding;
using Area = low_age_prototype_common.Area;

public partial class StructureNode : ActorNode, INodeFromBlueprint<Structure>
{
    public const string ScenePath = @"res://app/shared/game/entities/StructureNode.tscn";
    public static StructureNode Instance() => (StructureNode) GD.Load<PackedScene>(ScenePath).Instantiate();
    public static StructureNode InstantiateAsChild(Structure blueprint, Node parentNode)
    {
        var structure = Instance();
        parentNode.AddChild(structure);
        structure.SetBlueprint(blueprint);
        return structure;
    }
    
    public override Area RelativeSize => EntitySize.Except(WalkablePositionsBlueprint);
    public string FlattenedSprite { get; private set; }
    public Vector2? FlattenedCenterOffset { get; private set; }
    public Vector2<int> CenterPoint { get; protected set; }
    public IList<Area> WalkableAreasBlueprint { get; protected set; }
    public IEnumerable<Area> WalkableAreas => WalkableAreasBlueprint.Select(x => 
        new Area(x.Start + EntityPrimaryPosition, x.Size)).ToList();
    public IEnumerable<Vector2<int>> WalkablePositionsBlueprint => WalkableAreasBlueprint.Select(walkableArea =>
        walkableArea.ToList()).SelectMany(walkablePositions => walkablePositions).ToHashSet();
    public IEnumerable<Vector2<int>> WalkablePositions => WalkableAreas.Select(walkableArea => walkableArea.ToList())
        .SelectMany(walkablePositions => walkablePositions).ToHashSet();
    public IEnumerable<Vector2<int>> NonWalkablePositions => EntityOccupyingPositions.Except(WalkablePositions);
    
    private Structure Blueprint { get; set; }
    
    public void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = blueprint.Size;
        FlattenedSprite = blueprint.FlattenedSprite;
        FlattenedCenterOffset = Blueprint.FlattenedCenterOffset?.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint;
        WalkableAreasBlueprint = blueprint.WalkableAreas.Select(area => area.TrimTo(EntitySize)).ToList();
        
        Renderer.Initialize(this, false);
        UpdateSprite();
        UpdateVitalsPosition();
    }

    public override void Rotate()
    {
        var centerPointAssigned = false;
        
        for (var x = 0; x < EntitySize.X; x++)
        {
            if (centerPointAssigned)
                break;
            
            for (var y = 0; y < EntitySize.Y; y++)
            {
                if (centerPointAssigned)
                    break;
                
                var currentPoint = new Vector2<int>(x, y);
                var newX = EntitySize.Y - 1 - y;
                var newY = x;

                if (CenterPoint.Equals(currentPoint) is false)
                    continue;
                
                CenterPoint = new Vector2<int>(newX, newY);
                centerPointAssigned = true;
            }
        }

        WalkableAreasBlueprint = WalkableAreasBlueprint.RotateClockwiseInside(EntitySize);
        EntitySize = new Vector2<int>(EntitySize.Y, EntitySize.X);
        Renderer.AdjustToRelativeSize(RelativeSize);
        
        base.Rotate();
    }

    public override bool CanBeMovedOnAt(Point point, Team forTeam)
    {
        if (WalkablePositions.Any(point.Position.Equals))
            return true;

        return base.CanBeMovedOnAt(point, forTeam);
    }

    public override bool CanBeMovedThroughAt(Point point, Team forTeam) => CanBeMovedOnAt(point, forTeam);
    
    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();
        Renderer.SetSpriteVisibility(true);
        UpdateSprite();
        
        if (EntityState != State.Completed || ClientState.Instance.Flattened is false) 
            return;
        
        if (FlattenedSprite is null || FlattenedCenterOffset is null)
        {
            Renderer.SetSpriteVisibility(false); 
            Renderer.SetIconVisibility(true);
            return;
        }

        Renderer.SetSpriteTexture(FlattenedSprite);
        AdjustSpriteOffset(FlattenedCenterOffset);
    }

    protected override void UpdateSprite()
    {
        var needsBackSprite = ActorRotation == ActorRotation.TopLeft || ActorRotation == ActorRotation.TopRight;
        var needsFlipping = ActorRotation == ActorRotation.BottomLeft || ActorRotation == ActorRotation.TopRight;
        
        var spriteLocation = needsBackSprite ? Blueprint.BackSideSprite : Blueprint.Sprite;
        if (spriteLocation.IsNotNullOrEmpty())
            Renderer.SetSpriteTexture(spriteLocation);

        var offset = needsBackSprite
            ? Blueprint.BackSideCenterOffset.ToGodotVector2()
            : Blueprint.CenterOffset.ToGodotVector2();
        AdjustSpriteOffset(needsFlipping 
            ? new Vector2(Renderer.SpriteSize.X - offset.X, offset.Y)
            : offset);
        
        Renderer.FlipSprite(needsFlipping);
    }
}
