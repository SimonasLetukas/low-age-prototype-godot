using LowAgeCommon;

/// <summary>
/// Identifies abilities that should open a selection menu when pressed.
/// </summary>
public interface IAbilityHasSelection
{
    int GetSelectableItemCost(Id selectableItemId);
    string GetSelectableItemText(Id selectableItemId);
    bool IsSelectableItemDisabled(Id selectableItemId);
}