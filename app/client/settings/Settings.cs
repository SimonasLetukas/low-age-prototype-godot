using System;
using Godot;
using LowAgeData.Domain.Factions;

public partial class Settings : Control
{
    public const string ScenePath = @"res://app/client/settings/Settings.tscn";
    public static Settings Instance() => (Settings) GD.Load<PackedScene>(ScenePath).Instantiate();
    
    public event Action<FactionId> FactionSelected = delegate { };
    
    private FactionSelection _factionSelection = null!;
    private OptionButton _animationSelection = null!;
    private CheckBox _bigCursorToggle = null!;
    private CheckBox _showHintsToggle = null!;
    private CheckBox _connectTerrain = null!;
    
    private CheckBox _researchToggle = null!;
    private CheckBox _sameTeamCombat = null!;
    
    private Button _backButton = null!;
    
    public override void _Ready()
    {
        _factionSelection = (FactionSelection)FindChild("Faction")!;
        _animationSelection = (OptionButton)FindChild("Animation")!; 
        _bigCursorToggle = (CheckBox)FindChild("BigCursor")!;
        _showHintsToggle = (CheckBox)FindChild("ShowHints")!;
        _connectTerrain = (CheckBox)FindChild("ConnectTerrain")!;
        
        _researchToggle = (CheckBox)FindChild("Research")!;
        _sameTeamCombat = (CheckBox)FindChild("AllowSameTeamCombat")!;
        
        _backButton = (Button)FindChild("Back")!;

        _factionSelection.FactionSelected += OnFactionSelected;
        _animationSelection.Connect("item_selected", new Callable(this, nameof(OnAnimationSpeedSelected)));
        _bigCursorToggle.Connect(nameof(_bigCursorToggle.Pressed).ToLower(), new Callable(this, nameof(OnCursorSizeToggled)));
        _showHintsToggle.Connect(nameof(_showHintsToggle.Pressed).ToLower(), new Callable(this, nameof(OnShowHintsToggled)));
        _connectTerrain.Connect(nameof(_connectTerrain.Pressed).ToLower(), new Callable(this, nameof(OnConnectTerrainToggled)));
        
        _researchToggle.Connect(nameof(_researchToggle.Pressed).ToLower(), new Callable(this, nameof(OnResearchToggled)));
        _sameTeamCombat.Connect(nameof(_sameTeamCombat.Pressed).ToLower(), new Callable(this, nameof(OnSameTeamCombatToggled)));

        _backButton.Connect(nameof(_backButton.Pressed).ToLower(), new Callable(this, nameof(OnBackPressed)));
        
        _factionSelection.SetSelectedFaction(Config.Instance.StartingFaction);
        _animationSelection.Selected = (int)Config.Instance.AnimationSpeed;
        _bigCursorToggle.ButtonPressed = Config.Instance.LargeCursor;
        _showHintsToggle.ButtonPressed = Config.Instance.ShowHints;
        _connectTerrain.ButtonPressed = Config.Instance.ConnectTerrain;
        
        _researchToggle.ButtonPressed = Config.Instance.ResearchEnabled;
        _sameTeamCombat.ButtonPressed = Config.Instance.AllowSameTeamCombat;
    }

    public override void _ExitTree()
    {
        _factionSelection.FactionSelected -= OnFactionSelected;
    }

    private void OnAnimationSpeedSelected(int index) => Config.Instance.AnimationSpeed = (Config.AnimationSpeeds)index;

    private void OnCursorSizeToggled() => Config.Instance.LargeCursor = _bigCursorToggle.IsPressed();
    
    private void OnShowHintsToggled() => Config.Instance.ShowHints = _showHintsToggle.IsPressed();
    
    private void OnConnectTerrainToggled() => Config.Instance.ConnectTerrain = _connectTerrain.IsPressed();
    
    private void OnResearchToggled() => Config.Instance.ResearchEnabled = _researchToggle.IsPressed();
    
    private void OnSameTeamCombatToggled() => Config.Instance.AllowSameTeamCombat = _sameTeamCombat.IsPressed();

    private void OnBackPressed()
    {
        Config.Instance.Save();
        Visible = false;
    }

    private void OnFactionSelected(FactionId faction) => FactionSelected(faction);
}
