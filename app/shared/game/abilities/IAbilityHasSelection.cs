using System.Collections.Generic;
using LowAgeCommon;
using LowAgeData.Domain.Common;

/// <summary>
/// Identifies abilities that should open a selection menu when pressed.
/// </summary>
public interface IAbilityHasSelection
{
    IList<Payment> GetSelectableItemNonConsumableCost(Id selectableItemId);
    string GetSelectableItemText(Id selectableItemId);
    bool IsSelectableItemDisabled(Id selectableItemId);
}