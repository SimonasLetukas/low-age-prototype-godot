using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Common;
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
    
    public Vector2 StructureSize { get; protected set; }
    public Vector2 CenterPoint { get; protected set; }
    public List<Rect2> WalkableAreas { get; protected set; }
    
    private Structure Blueprint { get; set; }

    public void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        Blueprint = blueprint;
        StructureSize = blueprint.Size.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint.ToGodotVector2();
        WalkableAreas = blueprint.WalkableAreas.Select(area => area.ToGodotRect2().TrimTo(StructureSize)).ToList();
        
        AdjustSpriteOffset();
    }

    public override bool DeterminePlacementValidity(Func<Rect2, IList<Tiles.TileInstance>> getTiles, bool isValid = false)
    {
        var tiles = getTiles(new Rect2(EntityPosition, StructureSize));

        if (tiles.Any(x => x is null))
            return base.DeterminePlacementValidity(getTiles, false);

        if (tiles.All(x => x.IsTarget is false))
            return base.DeterminePlacementValidity(getTiles, false);

        if (tiles.Any(x => x.Terrain.Equals(Terrain.Mountains)))
            return base.DeterminePlacementValidity(getTiles, false);
        
        // TODO most of these conditions should come from buildable behaviour (placementValidators)
        // TODO go through all existing placementValidators and add appropriate checks (some might be impossible for
        // now - e.g. masks; leave todo for such) 
        
        return base.DeterminePlacementValidity(getTiles, true);
    }
    
    public override IList<Vector2> GetOccupiedPositions()
    {
        var positions = new List<Vector2>();
        for (var x = 0; x < StructureSize.x; x++)
        {
            for (var y = 0; y < StructureSize.y; y++)
            {
                positions.Add(new Vector2(EntityPosition.x + x, EntityPosition.y + y));
            }
        }

        return positions;
    }

    public void Rotate()
    {
        var centerPointAssigned = false;
        var newWalkableAreas = new List<Rect2>();
        
        for (var x = 0; x < StructureSize.x; x++)
        {
            for (var y = 0; y < StructureSize.y; y++)
            {
                var currentPoint = new Vector2(x, y);
                var newX = StructureSize.y - 1 - y;
                var newY = x;
                
                if (CenterPoint.IsEqualApprox(currentPoint) 
                    && centerPointAssigned is false)
                {
                    CenterPoint = new Vector2(newX, newY);
                    centerPointAssigned = true;
                }

                newWalkableAreas.AddRange(WalkableAreas
                    .Where(walkableArea => walkableArea.Position.IsEqualApprox(currentPoint))
                    .Select(walkableArea => new Rect2(
                        new Vector2((newX - walkableArea.Size.y) + 1, newY),
                        new Vector2(walkableArea.Size.y, walkableArea.Size.x))));
            }
        }

        WalkableAreas = newWalkableAreas;
        StructureSize = new Vector2(StructureSize.y, StructureSize.x);
        
        switch (ActorRotation)
        {
            case ActorRotation.BottomRight:
                ActorRotation = ActorRotation.BottomLeft;
                break;
            case ActorRotation.BottomLeft:
                ActorRotation = ActorRotation.TopLeft;
                break;
            case ActorRotation.TopLeft:
                ActorRotation = ActorRotation.TopRight;
                break;
            case ActorRotation.TopRight:
                ActorRotation = ActorRotation.BottomRight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        AdjustSpriteOffset();
    }

    protected override void AdjustSpriteOffset()
    {
        base.AdjustSpriteOffset();
        var offsetFromX = (int)(StructureSize.x - 1) * 
                          new Vector2((int)(Constants.TileWidth / 4), (int)(Constants.TileHeight / 4));
        var offsetFromY = (int)(StructureSize.y - 1) *
                          new Vector2((int)(Constants.TileWidth / 4) * -1, (int)(Constants.TileHeight / 4));
        Sprite.Offset = Sprite.Offset + offsetFromX + offsetFromY;
    }
}
