using LowAgeCommon;

/// <summary>
/// Identifies abilities that should open a selection menu when pressed.
/// </summary>
public interface ISelectable
{
    string GetSelectableItemText(Id selectableItemId);
    bool IsSelectableItemDisabled(Id selectableItemId);
}