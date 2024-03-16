using System;
using System.Collections.Generic;
using Godot;
using low_age_data.Domain.Factions;

public class FactionSelection : OptionButton
{
    public event Action<FactionId> FactionSelected = delegate { };
    
    private readonly Dictionary<int, FactionId> _factionIdsByOptionButtonIndex = new Dictionary<int, FactionId>();
    private readonly Dictionary<FactionId, int> _optionButtonIndexesByFactionId = new Dictionary<FactionId, int>();

    public override void _Ready()
    {
        PopulateFactions();

        Connect("item_selected", this, nameof(OnItemSelected));
    }

    public FactionId GetSelectedFaction() => _factionIdsByOptionButtonIndex[Selected];

    public void SetSelectedFaction(FactionId factionId)
    {
        var index = _optionButtonIndexesByFactionId[factionId];
        Selected = index;
    }
    
    private void PopulateFactions()
    {
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

        Selected = _optionButtonIndexesByFactionId.TryGetValue(Config.Instance.StartingFaction, out var preferredFaction) 
            ? preferredFaction
            : optionButtonIndex;
    }

    private void OnItemSelected(int index)
    {
        var factionId = _factionIdsByOptionButtonIndex[index];
        FactionSelected(factionId);
    }
}
