using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data.Domain.Entities.Actors.Structures;

public class StructureNode : ActorNode, INodeFromBlueprint<Structure>
{
    public const string ScenePath = @"res://app/shared/game/actors/StructureNode.tscn";
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
        StructureSize = blueprint.Size.ToGodotVector2();
        CenterPoint = blueprint.CenterPoint.ToGodotVector2();
        WalkableAreas = blueprint.WalkableAreas.Select(area => area.ToGodotRect2().TrimTo(StructureSize)).ToList();
    }

    public void Rotate()
    {
        StructureSize = new Vector2(StructureSize.y, StructureSize.x); // TODO
        switch (ActorRotation)
        {
            case ActorRotation.BottomRight:
                ActorRotation = ActorRotation.BottomLeft;
                WalkableAreas = WalkableAreas; // TODO
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
    }
}
