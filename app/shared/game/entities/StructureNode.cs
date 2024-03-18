using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;

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
    
    public Vector2 CenterPoint { get; protected set; }
    public IList<Rect2> WalkableAreasBlueprint { get; protected set; }
    public IEnumerable<Rect2> WalkableAreas => WalkableAreasBlueprint.Select(x => 
        new Rect2(x.Position + EntityPrimaryPosition, x.Size)).ToList();
    public IEnumerable<Vector2> WalkablePositions => WalkableAreas.Select(walkableArea => walkableArea.ToList())
        .SelectMany(walkablePositions => walkablePositions).ToHashSet();
    
    private Structure Blueprint { get; set; }

    public void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        EntitySize = blueprint.Size.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint.ToGodotVector2();
        WalkableAreasBlueprint = blueprint.WalkableAreas.Select(area => area.ToGodotRect2().TrimTo(EntitySize)).ToList();
        
        UpdateSprite();
    }

    public override void Rotate()
    {
        var centerPointAssigned = false;
        var newWalkableAreas = new List<Rect2>();
        
        for (var x = 0; x < EntitySize.x; x++)
        {
            for (var y = 0; y < EntitySize.y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = EntitySize.y - 1 - y;
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
                        new Vector2((newX - walkableArea.Size.y) + 1, newY),
                        new Vector2(walkableArea.Size.y, walkableArea.Size.x))));
            }
        }

        WalkableAreasBlueprint = newWalkableAreas;
        EntitySize = new Vector2(EntitySize.y, EntitySize.x);
        
        base.Rotate();
    }

    protected override void UpdateSprite()
    {
        var needsBackSprite = ActorRotation == ActorRotation.TopLeft || ActorRotation == ActorRotation.TopRight;
        var needsFlipping = ActorRotation == ActorRotation.BottomLeft || ActorRotation == ActorRotation.TopRight;
        
        var spriteLocation = needsBackSprite ? Blueprint.BackSideSprite : Blueprint.Sprite;
        if (spriteLocation.IsNotNullOrEmpty())
            Sprite.Texture = GD.Load<Texture>(spriteLocation);

        AdjustSpriteOffset(needsBackSprite 
            ? Blueprint.BackSideCenterOffset.ToGodotVector2() 
            : Blueprint.CenterOffset.ToGodotVector2());
        
        Sprite.Scale = needsFlipping ? new Vector2(-1, Scale.y) : new Vector2(1, Scale.y);
    }
}
