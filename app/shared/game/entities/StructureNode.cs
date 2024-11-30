using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_prototype_common.Extensions;

public class StructureNode : ActorNode, INodeFromBlueprint<Structure>
{
    public const string ScenePath = @"res://app/shared/game/entities/StructureNode.tscn";
    public static StructureNode Instance() => (StructureNode) GD.Load<PackedScene>(ScenePath).Instance();
    public static StructureNode InstantiateAsChild(Structure blueprint, Node parentNode)
    {
        var structure = Instance();
        parentNode.AddChild(structure);
        structure.SetBlueprint(blueprint);
        return structure;
    }
    
    public override Rect2 RelativeSize => EntitySize.Except(WalkablePositionsBlueprint);
    public string FlattenedSprite { get; private set; }
    public Vector2? FlattenedCenterOffset { get; private set; }
    public Vector2 CenterPoint { get; protected set; }
    public IList<Rect2> WalkableAreasBlueprint { get; protected set; }
    public IEnumerable<Rect2> WalkableAreas => WalkableAreasBlueprint.Select(x => 
        new Rect2(x.Position + EntityPrimaryPosition, x.Size)).ToList();
    public IEnumerable<Vector2> WalkablePositionsBlueprint => WalkableAreasBlueprint.Select(walkableArea =>
        walkableArea.ToList()).SelectMany(walkablePositions => walkablePositions).ToHashSet();
    public IEnumerable<Vector2> WalkablePositions => WalkableAreas.Select(walkableArea => walkableArea.ToList())
        .SelectMany(walkablePositions => walkablePositions).ToHashSet();
    public IEnumerable<Vector2> NonWalkablePositions => EntityOccupyingPositions.Except(WalkablePositions);
    
    private Structure Blueprint { get; set; }
    
    public void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = blueprint.Size.ToGodotVector2();
        FlattenedSprite = blueprint.FlattenedSprite;
        FlattenedCenterOffset = Blueprint.FlattenedCenterOffset?.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint.ToGodotVector2();
        WalkableAreasBlueprint = blueprint.WalkableAreas.Select(area => area.ToGodotRect2().TrimTo(EntitySize)).ToList();
        
        Renderer.Initialize(this, false);
        UpdateSprite();
        UpdateVitalsPosition();
    }

    public override void Rotate()
    {
        var centerPointAssigned = false;
        
        for (var x = 0; x < EntitySize.x; x++)
        {
            if (centerPointAssigned)
                break;
            
            for (var y = 0; y < EntitySize.y; y++)
            {
                if (centerPointAssigned)
                    break;
                
                var currentPoint = new Vector2(x, y);
                var newX = EntitySize.y - 1 - y;
                var newY = x;

                if (CenterPoint.IsEqualApprox(currentPoint) is false)
                    continue;
                
                CenterPoint = new Vector2(newX, newY);
                centerPointAssigned = true;
            }
        }

        WalkableAreasBlueprint = WalkableAreasBlueprint.RotateClockwiseInside(EntitySize);
        EntitySize = new Vector2(EntitySize.y, EntitySize.x);
        Renderer.AdjustToRelativeSize(RelativeSize);
        
        base.Rotate();
    }

    public override bool CanBeMovedOnAt(Point point)
    {
        if (WalkablePositions.Any(point.Position.Equals))
            return true;

        return base.CanBeMovedOnAt(point);
    }

    public override bool CanBeMovedThroughAt(Point point) => CanBeMovedOnAt(point);
    
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
            ? new Vector2(Renderer.SpriteSize.x - offset.x, offset.y)
            : offset);
        
        Renderer.FlipSprite(needsFlipping);
    }
}
