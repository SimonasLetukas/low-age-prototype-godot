using System;
using System.Linq;
using Godot;
using low_age_data.Domain.Factions;

public class PlayerInLobby : HBoxContainer
{
    public const string ScenePath = @"res://app/shared/lobby/players/PlayerInLobby.tscn";

    public event Action<PlayerInLobby, FactionId> PlayerSelectedFaction = delegate { };

    public FactionSelection FactionSelection { get; private set; }
    public Player Player { get; private set; }

    private Label _nameLabel;
    
    public override void _Ready()
    {
        FactionSelection = GetNode<FactionSelection>("Faction");
        FactionSelection.FactionSelected += OnFactionSelectionSelected;

        _nameLabel = GetNode<Label>("Name");
    }

    public override void _ExitTree()
    {
        FactionSelection.FactionSelected -= OnFactionSelectionSelected;
        base._ExitTree();
    }

    public Player SetupPlayer(int playerId)
    {
        SetNetworkMaster(playerId);
        Name = $"{playerId}";

        Player = Data.Instance.Players.Single(x => x.Id.Equals(playerId));
        _nameLabel.Text = Player.Name;
        
        FactionSelection.SetSelectedFaction(Player.Faction);
        
        return Player;
    }

    private void OnFactionSelectionSelected(FactionId factionId)
    {
        PlayerSelectedFaction(this, factionId);
    }
}
