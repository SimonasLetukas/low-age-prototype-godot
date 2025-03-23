using System;
using Godot;
using LowAgeData.Domain.Factions;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

public partial class Config : Node
{
    public const string SavePath = @"res://data/config.json";
    public static Config Instance = null!;
    
    public void Save()
    {
        var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(ToString());
        file.Close();
    }

    public void Load()
    {
        if (FileAccess.FileExists(SavePath) is false)
            return;

        var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        FromString(file.GetAsText());
        file.Close();
    }

    public override string ToString() => JsonConvert.SerializeObject(_data);

    public bool FromString(string value)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject<ConfigData>(value);
            if (obj is null)
                return false;
            
            _data = obj;
            return true;
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
            return false;
        }
    }

    public AnimationSpeeds AnimationSpeed
    {
        get => GetData().AnimationSpeed;
        set => GetData().AnimationSpeed = value;
    }

    public bool ConnectTerrain
    {
        get => GetData().ConnectTerrain;
        set => GetData().ConnectTerrain = value;
    }

    public bool ResearchEnabled
    {
        get => GetData().ResearchEnabled;
        set => GetData().ResearchEnabled = value;
    }

    public FactionId StartingFaction
    {
        get => GetData().StartingFaction;
        set => GetData().StartingFaction = value;
    }

    public bool LargeCursor
    {
        get => GetData().LargeCursor;
        set => GetData().LargeCursor = value;
    }

    public bool ShowHints
    {
        get => GetData().ShowHints;
        set => GetData().ShowHints = value;
    }

    public bool AllowSameTeamCombat
    {
        get => GetData().AllowSameTeamCombat;
        set => GetData().AllowSameTeamCombat = value;
    }

    private ConfigData? _data;
    private ConfigData GetData()
    {
        if (_data is null)
            Load();
        
        return _data!;
    }
    
    public override void _Ready()
    {
        base._Ready();
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;

        if (GetMultiplayer().IsServer())
            return;
        
        Load();
        Save();
    }

    private class ConfigData
    {
        public AnimationSpeeds AnimationSpeed { get; set; } = AnimationSpeeds.Fast;
        public bool ConnectTerrain { get; set; } = false;
        public bool ResearchEnabled { get; set; } = false;
        public FactionId StartingFaction { get; set; } = FactionId.Uee;
        public bool LargeCursor { get; set; } = false;
        public bool ShowHints { get; set; } = true;
        public bool AllowSameTeamCombat { get; set; } = false;
    }

    public enum AnimationSpeeds
    {
        Slow,
        Medium,
        Fast
    }
}