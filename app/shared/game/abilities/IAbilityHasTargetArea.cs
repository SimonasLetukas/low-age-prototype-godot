using System.Collections.Generic;
using Godot;
using LowAgeCommon;

/// <summary>
/// Identifies abilities that show a target area when selected
/// </summary>
public interface IAbilityHasTargetArea : IAbilityNode
{
    public bool WholeMapIsTargeted();
    IEnumerable<Vector2Int> GetTargetPositions(EntityNode caster);
    IEnumerable<Vector2> GetGlobalPositionsOfFocusedTargets();
}