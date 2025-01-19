using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;

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
    
    public Vector2 CenterPoint { get; protected set; }
    public IList<Rect2> WalkableAreasBlueprint { get; protected set; }
    public IEnumerable<Rect2> WalkableAreas => WalkableAreasBlueprint.Select(x => 
        new Rect2(x.Position + EntityPrimaryPosition, x.Size)).ToList();
    public IEnumerable<Vector2> WalkablePositions => WalkableAreas.Select(walkableArea => walkableArea.ToList())
        .SelectMany(walkablePositions => walkablePositions).ToHashSet();
    public IEnumerable<Vector2> WalkablePositionsBlueprint => WalkableAreasBlueprint.Select(walkableArea =>
        walkableArea.ToList()).SelectMany(walkablePositions => walkablePositions).ToHashSet();
    
    protected override Rect2 RelativeSize => EntitySize.Except(WalkablePositionsBlueprint);
    
    private Structure Blueprint { get; set; }

    public void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = blueprint.Size.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint.ToGodotVector2();
        WalkableAreasBlueprint = blueprint.WalkableAreas.Select(area => area.ToGodotRect2().TrimTo(EntitySize)).ToList();
        
        Renderer.Initialize(InstanceId, Blueprint.DisplayName, false, RelativeSize);
        UpdateSprite();
        UpdateVitalsPosition();
    }

    public override void Rotate()
    {
        var centerPointAssigned = false;
        var newWalkableAreas = new List<Rect2>();
        
        for (var x = 0; x < EntitySize.X; x++)
        {
            for (var y = 0; y < EntitySize.Y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = EntitySize.Y - 1 - y;
                var newY = x;
                
                if (CenterPoint.IsEqualApprox(currentPoint) 
                    && centerPointAssigned is false)
                {
                    CenterPoint = new Vector2(newX, newY);
                    centerPointAssigned = true;
                }

                newWalkableAreas.AddRange(WalkableAreasBlueprint
                    .Where(walkableArea => walkableArea.Position.IsEqualApprox(currentPoint))
                    .Select(walkableArea => new Rect2(
                        new Vector2((newX - walkableArea.Size.Y) + 1, newY),
                        new Vector2(walkableArea.Size.Y, walkableArea.Size.X))));
            }
        }

        WalkableAreasBlueprint = newWalkableAreas;
        EntitySize = new Vector2(EntitySize.Y, EntitySize.X);
        Renderer.AdjustToRelativeSize(RelativeSize);
        
        base.Rotate();
    }

    public override bool CanBeMovedOnAt(Vector2 position)
    {
        if (WalkablePositions.Any(position.Equals))
            return true;

        return base.CanBeMovedOnAt(position);
    }

    public override bool CanBeMovedThroughAt(Vector2 position) => CanBeMovedOnAt(position);

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
