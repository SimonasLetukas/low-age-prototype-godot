using System;
using System.Linq;
using Godot;
using LowAgeData.Domain.Factions;

public partial class PlayerInLobby : HBoxContainer
{
    public const string ScenePath = @"res://app/shared/lobby/players/PlayerInLobby.tscn";

    public event Action<PlayerInLobby, FactionId> PlayerSelectedFaction = delegate { };
    public event Action<PlayerInLobby, bool> PlayerChangedReadyStatus = delegate { };

    public static PlayerInLobby Instance() => (PlayerInLobby) GD.Load<PackedScene>(ScenePath).Instantiate();
    
    public Player Player { get; private set; }
    
    private FactionSelection _factionSelection;
    private CheckBox _readyStatus;
    private Label _nameLabel;
    
    public override void _Ready()
    {
        _factionSelection = GetNode<FactionSelection>("Faction");
        _factionSelection.FactionSelected += OnFactionSelectionSelected;

        _nameLabel = GetNode<Label>("Name");

        _readyStatus = GetNode<CheckBox>("Ready");
        _readyStatus.Connect("toggled", new Callable(this, nameof(OnReadyStatusToggled)));
    }

    public override void _ExitTree()
    {
        _factionSelection.FactionSelected -= OnFactionSelectionSelected;
        base._ExitTree();
    }

    public Player SetupPlayer(int playerId)
    {
        SetMultiplayerAuthority(playerId);
        if (Multiplayer.GetUniqueId() != playerId)
        {
            _factionSelection.Disabled = true;
            _readyStatus.Disabled = true;
        }
        Name = $"{playerId}";

        Player = Data.Instance.Players.Single(x => x.Id.Equals(playerId));
        _nameLabel.Text = Player.Name;
        
        _factionSelection.SetSelectedFaction(Player.Faction);
        SetReadyStatus(Player.Ready);
        
        return Player;
    }

    public void SetSelectedFaction(FactionId to)
    {
        _factionSelection.SetSelectedFaction(to);
        Player.Faction = to;
    }

    public void SetReadyStatus(bool to)
    {
        _readyStatus.ButtonPressed = to;
        Player.Ready = to;
    }

    private void OnFactionSelectionSelected(FactionId factionId)
    {
        PlayerSelectedFaction(this, factionId);
    }

    private void OnReadyStatusToggled(bool to)
    {
        PlayerChangedReadyStatus(this, to);
    }
}
