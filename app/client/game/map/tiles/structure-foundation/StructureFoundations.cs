using System;
using System.Collections.Generic;
using Godot;

// TODO: Probably needs to be moved under Entities.cs instead of Tiles.cs
public partial class StructureFoundations : Node2D
{
    private readonly Dictionary<StructureNode, StructureFoundation> _foundationsByStructure = new();

    public override void _Ready()
    {
        base._Ready();
        foreach (var child in GetChildren()) 
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
        if (_foundationsByStructure.ContainsKey(structure))
            return;
        var foundation = StructureFoundation.InstantiateAsChild(this);
        foundation.Visible = ClientState.Instance.Flattened;
        foundation.Initialize(structure);
        _foundationsByStructure.Add(structure, foundation);
    }

    public void RemoveOccupation(StructureNode structure)
    {
        if (_foundationsByStructure.TryGetValue(structure, out var foundation) is false)
            return;
        foundation.QueueFree();
        _foundationsByStructure.Remove(structure);
    }

    private void Enable()
    {
        foreach (var (structure, foundation) in _foundationsByStructure)
        {
            if (structure.IsRevealed())
                foundation.Visible = true;
        }
    }

    private void Disable()
    {
        foreach (var (_, foundation) in _foundationsByStructure)
        {
            foundation.Visible = false;
        }
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
