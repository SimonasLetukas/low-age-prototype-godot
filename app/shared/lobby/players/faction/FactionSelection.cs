using System;
using System.Collections.Generic;
using Godot;
using LowAgeData.Domain.Factions;

public partial class FactionSelection : OptionButton
{
    public event Action<FactionId> FactionSelected = delegate { };
    
    private readonly Dictionary<int, FactionId> _factionIdsByOptionButtonIndex = new();
    private readonly Dictionary<FactionId, int> _optionButtonIndexesByFactionId = new();

    public override void _Ready()
    {
        PopulateFactions();

        Connect("item_selected", new Callable(this, nameof(OnItemSelected)));
    }

    public FactionId GetSelectedFaction() => _factionIdsByOptionButtonIndex[Selected];

    public void SetSelectedFaction(FactionId factionId)
    {
        var index = _optionButtonIndexesByFactionId[factionId];
        Selected = index;
    }
    
    private void PopulateFactions()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Data.Instance.Blueprint is null) 
            Data.Instance.ReadBlueprint();
        
        Clear();
        
        var optionButtonIndex = 0;
        foreach (var faction in Data.Instance.Blueprint?.Factions ?? new List<Faction>())
        {
            AddItem(faction.DisplayName, optionButtonIndex);
            _factionIdsByOptionButtonIndex[optionButtonIndex] = faction.Id;
            _optionButtonIndexesByFactionId[faction.Id] = optionButtonIndex;
            optionButtonIndex++;
        }

        if (OS.HasFeature(nameof(Server).ToLower()))
            Selected = optionButtonIndex;
        else
            Selected = _optionButtonIndexesByFactionId.GetValueOrDefault(
                Config.Instance.StartingFaction, optionButtonIndex);
    }

    private void OnItemSelected(int index)
    {
        var factionId = _factionIdsByOptionButtonIndex[index];
        Config.Instance.StartingFaction = factionId;
        FactionSelected(factionId);
    }
}
