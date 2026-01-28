using System;
using Godot;
using LowAgeData.Domain.Factions;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

public partial class Config : Node
{
    public const string SavePath = @"user://config.json";
    public static Config Instance = null!;
    
    public void Save()
    {
        _data ??= new ConfigData();
        
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        
        if (file == null)
        {
            var err = FileAccess.GetOpenError();
            GD.PrintErr($"Failed to open config file. Error: {err}");
            return;
        }
        
        file.StoreString(ToString());
        file.Close();
    }

    public void Load()
    {
        if (FileAccess.FileExists(SavePath) is false)
        {
            _data = new ConfigData();
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        FromString(file.GetAsText());
        file.Close();
    }

    public override string ToString() => JsonConvert.SerializeObject(_data);

    public bool FromString(string value)
    {
        try
        {
            var obj = JsonConvert.DeserializeObject<ConfigData>(value);
            _data = obj 
                    ?? throw new Exception($"Config value '{value}' returned null after deserialization.");
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
    
    public bool ResearchEnabled
    {
        get => GetData().ResearchEnabled;
        set => GetData().ResearchEnabled = value;
    }

    public bool DeterministicInitiative
    {
        get => GetData().DeterministicInitiative;
        set => GetData().DeterministicInitiative = value;
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

        if (OS.HasFeature(nameof(Server).ToLower()))
            return;
        
        GD.Print("User data dir: ", OS.GetUserDataDir());
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
        
        Load();
        Save();
    }

    private class ConfigData
    {
        public AnimationSpeeds AnimationSpeed { get; set; } = AnimationSpeeds.Fast;
        public bool ConnectTerrain { get; set; } = false;
        public FactionId StartingFaction { get; set; } = FactionId.Uee;
        public bool LargeCursor { get; set; } = false;
        public bool ShowHints { get; set; } = true;
        public bool AllowSameTeamCombat { get; set; } = false;
        public bool ResearchEnabled { get; set; } = false;
        public bool DeterministicInitiative { get; set; } = false;
    }

    public enum AnimationSpeeds
    {
        Slow,
        Medium,
        Fast
    }
}