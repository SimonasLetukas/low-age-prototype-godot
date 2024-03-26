using System;
using Godot;
using low_age_data.Domain.Factions;

public class Settings : Control
{
    public const string ScenePath = @"res://app/client/settings/Settings.tscn";
    public static Settings Instance() => (Settings) GD.Load<PackedScene>(ScenePath).Instance();
    
    public event Action<FactionId> FactionSelected = delegate { };
    
    private FactionSelection _factionSelection;
    private OptionButton _animationSelection;
    private CheckBox _researchToggle;
    private CheckBox _bigCursorToggle;
    private Button _backButton;
    
    public override void _Ready()
    {
        _factionSelection = GetNode<FactionSelection>("Items/StartingFaction/Faction");
        _animationSelection = FindNode("Animation") as OptionButton;
        _researchToggle = FindNode("Research") as CheckBox;
        _bigCursorToggle = FindNode("BigCursor") as CheckBox;
        _backButton = FindNode("Back") as Button;

        _factionSelection.FactionSelected += OnFactionSelected;
        _animationSelection?.Connect("item_selected", this, nameof(OnAnimationSpeedSelected));
        _researchToggle?.Connect(nameof(_researchToggle.Pressed).ToLower(), this, nameof(OnResearchToggled));
        _bigCursorToggle?.Connect(nameof(_bigCursorToggle.Pressed).ToLower(), this, nameof(OnCursorSizeToggled));
        _backButton?.Connect(nameof(_backButton.Pressed).ToLower(), this, nameof(OnBackPressed));
        
        _factionSelection.SetSelectedFaction(Config.Instance.StartingFaction);
        if (_animationSelection != null) _animationSelection.Selected = (int)Config.Instance.AnimationSpeed;
        if (_researchToggle != null) _researchToggle.Pressed = Config.Instance.ResearchEnabled;
        if (_bigCursorToggle != null) _bigCursorToggle.Pressed = Config.Instance.LargeCursor;
    }

    public override void _ExitTree()
    {
        _factionSelection.FactionSelected -= OnFactionSelected;
    }

    private void OnAnimationSpeedSelected(int index) => Config.Instance.AnimationSpeed = (Config.AnimationSpeeds)index;
    
    private void OnResearchToggled() => Config.Instance.ResearchEnabled = _researchToggle.Pressed;

    private void OnCursorSizeToggled() => Config.Instance.LargeCursor = _bigCursorToggle.Pressed;

    private void OnBackPressed()
    {
        Config.Instance.Save();
        Visible = false;
    }

    private void OnFactionSelected(FactionId faction) => FactionSelected(faction);
}
