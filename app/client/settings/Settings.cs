using System;
using Godot;
using LowAgeData.Domain.Factions;

public partial class Settings : Control
{
    public const string ScenePath = @"res://app/client/settings/Settings.tscn";
    public static Settings Instance() => (Settings) GD.Load<PackedScene>(ScenePath).Instantiate();
    
    public event Action<FactionId> FactionSelected = delegate { };
    
    private FactionSelection _factionSelection;
    private OptionButton _animationSelection;
    private CheckBox _researchToggle;
    private CheckBox _bigCursorToggle;
    private CheckBox _showHintsToggle;
    private Button _backButton;
    
    public override void _Ready()
    {
        _factionSelection = GetNode<FactionSelection>("Items/StartingFaction/Faction");
        _animationSelection = FindChild("Animation") as OptionButton;
        _researchToggle = FindChild("Research") as CheckBox;
        _bigCursorToggle = FindChild("BigCursor") as CheckBox;
        _showHintsToggle = FindChild("ShowHints") as CheckBox;
        _backButton = FindChild("Back") as Button;

        _factionSelection.FactionSelected += OnFactionSelected;
        _animationSelection?.Connect("item_selected", new Callable(this, nameof(OnAnimationSpeedSelected)));
        _researchToggle?.Connect(nameof(_researchToggle.Pressed).ToLower(), new Callable(this, nameof(OnResearchToggled)));
        _bigCursorToggle?.Connect(nameof(_bigCursorToggle.Pressed).ToLower(), new Callable(this, nameof(OnCursorSizeToggled)));
        _backButton?.Connect(nameof(_backButton.Pressed).ToLower(), new Callable(this, nameof(OnBackPressed)));
        _showHintsToggle?.Connect(nameof(_showHintsToggle.Pressed).ToLower(), new Callable(this, nameof(OnShowHintsToggled)));
        
        _factionSelection.SetSelectedFaction(Config.Instance.StartingFaction);
        if (_animationSelection != null) _animationSelection.Selected = (int)Config.Instance.AnimationSpeed;
        if (_researchToggle != null) _researchToggle.ButtonPressed = Config.Instance.ResearchEnabled;
        if (_bigCursorToggle != null) _bigCursorToggle.ButtonPressed = Config.Instance.LargeCursor;
        if (_showHintsToggle != null) _showHintsToggle.ButtonPressed = Config.Instance.ShowHints;
    }

    public override void _ExitTree()
    {
        _factionSelection.FactionSelected -= OnFactionSelected;
    }

    private void OnAnimationSpeedSelected(int index) => Config.Instance.AnimationSpeed = (Config.AnimationSpeeds)index;
    
    private void OnResearchToggled() => Config.Instance.ResearchEnabled = _researchToggle.IsPressed();

    private void OnCursorSizeToggled() => Config.Instance.LargeCursor = _bigCursorToggle.IsPressed();
    
    private void OnShowHintsToggled() => Config.Instance.ShowHints = _showHintsToggle.IsPressed();

    private void OnBackPressed()
    {
        Config.Instance.Save();
        Visible = false;
    }

    private void OnFactionSelected(FactionId faction) => FactionSelected(faction);
}
