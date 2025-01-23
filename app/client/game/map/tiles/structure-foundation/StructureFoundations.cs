using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

// TODO: Probably needs to be moved under Entities.cs instead of Tiles.cs
public class StructureFoundations : Node2D
{
    private readonly Dictionary<Guid, StructureFoundation> _foundationsByInstanceId =
        new Dictionary<Guid, StructureFoundation>();

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        foreach (var child in GetChildren().OfType<Node>()) 
            child.QueueFree();

        EventBus.Instance.WhenFlattenedChanged += OnWhenFlattenedChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        EventBus.Instance.WhenFlattenedChanged -= OnWhenFlattenedChanged;
    }

    public void AddOccupation(StructureNode structure)
    {
        if (_foundationsByInstanceId.ContainsKey(structure.InstanceId))
            return;
        var foundation = StructureFoundation.InstantiateAsChild(this);
        foundation.Initialize(structure);
        _foundationsByInstanceId.Add(structure.InstanceId, foundation);
    }

    public void RemoveOccupation(StructureNode structure)
    {
        if (_foundationsByInstanceId.TryGetValue(structure.InstanceId, out var foundation) is false)
            return;
        foundation.QueueFree();
        _foundationsByInstanceId.Remove(structure.InstanceId);
    }

    private void Enable()
    {
        Visible = true;
    }

    private void Disable()
    {
        Visible = false;
    }

    private void OnWhenFlattenedChanged(bool to)
    {
        if (to)
        {
            Enable();
            return;
        }
        
        Disable();
    }
}
