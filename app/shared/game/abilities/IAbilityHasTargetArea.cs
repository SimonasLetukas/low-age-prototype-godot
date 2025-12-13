using System.Collections.Generic;
using LowAgeCommon;

/// <summary>
/// Identifies abilities that show a target area when selected
/// </summary>
public interface IAbilityHasTargetArea
{
    public bool WholeMapIsTargeted();
    IEnumerable<Vector2Int> GetTargetPositions(EntityNode caster, Vector2Int mapSize);
}