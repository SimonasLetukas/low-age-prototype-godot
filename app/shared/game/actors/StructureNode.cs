using System;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;

public class StructureNode : ActorNode<Structure>
{
    public override Structure Blueprint { get; protected set; }
    public Vector2 StructureSize { get; protected set; }
    public ActorRotation StructureRotation { get; protected set; }
    public Vector2 ActualCenterPoint { get; protected set; }
    public Rect2 ActualWalkableArea { get; protected set; }

    public override void SetBlueprint(Structure blueprint)
    {
        base.SetBlueprint(blueprint);
        StructureSize = blueprint.Size.ToGodotVector2();
        StructureRotation = ActorRotation.BottomRight;
        ActualCenterPoint = blueprint.CenterPoint.ToGodotVector2();
        ActualWalkableArea = blueprint.WalkableArea.ToGodotRect2().TrimTo(StructureSize);
    }

    public void Rotate()
    {
        StructureSize = new Vector2(StructureSize.y, StructureSize.x);
        switch (StructureRotation)
        {
            case ActorRotation.BottomRight:
                StructureRotation = ActorRotation.BottomLeft;
                ActualWalkableArea = ActualWalkableArea;
                break;
            case ActorRotation.BottomLeft:
                StructureRotation = ActorRotation.TopLeft;
                break;
            case ActorRotation.TopLeft:
                StructureRotation = ActorRotation.TopRight;
                break;
            case ActorRotation.TopRight:
                StructureRotation = ActorRotation.BottomRight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
