using System;
using Godot;
using LowAgeData.Domain.Factions;
using MultipurposePathfinding;

public partial class PlayerInLobby : HBoxContainer
{
    public const string ScenePath = @"res://app/shared/lobby/players/PlayerInLobby.tscn";

    public event Action<PlayerInLobby, int> PlayerSelectedOriginalPlayer = delegate { };
    public event Action<PlayerInLobby, FactionId> PlayerSelectedFaction = delegate { };
    public event Action<PlayerInLobby, Team> PlayerSelectedTeam = delegate { };
    public event Action<PlayerInLobby, bool> PlayerChangedReadyStatus = delegate { };

    public static PlayerInLobby Instance() => (PlayerInLobby) GD.Load<PackedScene>(ScenePath).Instantiate();
    
    public Player Player { get; private set; } = null!;

    private OriginalPlayer _originalPlayer = null!;
    private FactionSelection _factionSelection = null!;
    private TeamSelection _teamSelection = null!;
    private CheckBox _readyStatus = null!;
    private Label _nameLabel = null!;

    public override void _Ready()
    {
        _originalPlayer = GetNode<OriginalPlayer>("OriginalPlayer");
        _originalPlayer.OriginalPlayerSelected += OnOriginalPlayerSelected;
        _originalPlayer.Visible = false;
        
        _factionSelection = GetNode<FactionSelection>("Faction");
        _factionSelection.FactionSelected += OnFactionSelectionSelected;
        
        _teamSelection = GetNode<TeamSelection>("Team");
        _teamSelection.TeamSelected += OnTeamSelectionSelected;

        _nameLabel = GetNode<Label>("Name");

        _readyStatus = GetNode<CheckBox>("Ready");
        _readyStatus.Connect("toggled", new Callable(this, nameof(OnReadyStatusToggled)));

        Data.Instance.SaveUpdated += OnSaveUpdated;
    }

    public override void _ExitTree()
    {
        _originalPlayer.OriginalPlayerSelected -= OnOriginalPlayerSelected;
        _factionSelection.FactionSelected -= OnFactionSelectionSelected;
        _teamSelection.TeamSelected -= OnTeamSelectionSelected;
        Data.Instance.SaveUpdated -= OnSaveUpdated;

        base._ExitTree();
    }

    public void SetupPlayer(int playerId)
    {
        SetMultiplayerAuthority(playerId);
        if (Multiplayer.GetUniqueId() != playerId)
        {
            _originalPlayer.Disabled = true;
            _factionSelection.Disabled = true;
            _teamSelection.Disabled = true;
            _readyStatus.Disabled = true;
        }
        Name = $"{playerId}";

        if (Data.Instance.Save is not null)
            PrepareInputsForSavedGame();

        Player = Players.Instance.GetById(playerId);
        _nameLabel.Text = Player.Name;
        
        var originalPlayer = _originalPlayer.SetSelectedOriginalPlayer(Player.StableId);
        if (originalPlayer is not null)
        {
            Player.Faction = originalPlayer.FactionId;
            Player.Team = originalPlayer.Team;
        }
        
        _factionSelection.SetSelectedFaction(Player.Faction);
        _teamSelection.SetSelectedTeam(Player.Team);
        SetReadyStatus(Player.Ready); 
    }

    public void SetOriginalPlayer(int originalPlayerStableId)
    {
        var originalPlayer = _originalPlayer.SetSelectedOriginalPlayer(originalPlayerStableId);
        if (originalPlayer is null)
            return;
        
        Player.StableId = originalPlayerStableId;
        PlayerSelectedFaction(this, originalPlayer.FactionId);
        PlayerSelectedTeam(this, originalPlayer.Team);
    }

    public void SetSelectedFaction(FactionId to)
    {
        _factionSelection.SetSelectedFaction(to);
        Player.Faction = to;
    }

    public void SetSelectedTeam(Team to)
    {
        _teamSelection.SetSelectedTeam(to);
        Player.Team = to;
    }

    public void AdjustMaxSelectedTeam()
    {
        var maxAvailableTeam = Players.Instance.GetMaxAvailableTeam();
        if (Player.Team > maxAvailableTeam)
            SetSelectedTeam(maxAvailableTeam);
    }

    public void SetReadyStatus(bool to)
    {
        _readyStatus.ButtonPressed = to;
        Player.Ready = to;
    }

    private void PrepareInputsForSavedGame()
    {
        _originalPlayer.Visible = true;
        
        _factionSelection.Disabled = true;
        _teamSelection.Disabled = true;
    }

    private void OnOriginalPlayerSelected(int originalPlayerStableId)
        => PlayerSelectedOriginalPlayer(this, originalPlayerStableId);

    private void OnFactionSelectionSelected(FactionId factionId) => PlayerSelectedFaction(this, factionId);

    private void OnTeamSelectionSelected(Team team) => PlayerSelectedTeam(this, team);

    private void OnReadyStatusToggled(bool to) => PlayerChangedReadyStatus(this, to);

    private void OnSaveUpdated(Save save)
    {
        PrepareInputsForSavedGame();
        SetOriginalPlayer(Player.StableId);
    }
}
