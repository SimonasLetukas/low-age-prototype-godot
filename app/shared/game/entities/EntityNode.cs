using System;
using Godot;

/// <summary>
/// Selectable object that has a presence and is interactable on the map: actor (structure or unit) with abilities
/// and stats, or a simple doodad.
/// </summary>
public class EntityNode : Node2D, INodeFromBlueprint<low_age_data.Domain.Entities.Entity>
{
    public Guid Id { get; }
    
    private low_age_data.Domain.Entities.Entity Blueprint { get; set; }
    
    public void SetBlueprint(low_age_data.Domain.Entities.Entity blueprint)
    {
        throw new NotImplementedException();
    }
}
