using System;
using Godot;
using low_age_data.Domain.Factions;
using Newtonsoft.Json;

public partial class Config : Node
{
    public const string SavePath = @"res://data/config.json";
    public static Config Instance = null;

    public void Save()
    {
        var file = new File();
        file.Open(SavePath, File.ModeFlags.Write);
        file.StoreString(ToString());
        file.Close();
    }

    public void Load()
    {
        var file = new File();
        var result = file.FileExists(SavePath) ? file.Open(SavePath, File.ModeFlags.Read) : Error.DoesNotExist;
        if (result is Error.Ok) 
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
            return false;
        }
    }

    public AnimationSpeeds AnimationSpeed
    {
        get => _data.AnimationSpeed;
        set => _data.AnimationSpeed = value;
    }

    public bool ResearchEnabled
    {
        get => _data.ResearchEnabled;
        set => _data.ResearchEnabled = value;
    }

    public FactionId StartingFaction
    {
        get => _data.StartingFaction;
        set => _data.StartingFaction = value;
    }

    public bool LargeCursor
    {
        get => _data.LargeCursor;
        set => _data.LargeCursor = value;
    }

    private ConfigData _data = new ConfigData();
    
    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
        
        Load();
    }

    private class ConfigData
    {
        public AnimationSpeeds AnimationSpeed { get; set; } = AnimationSpeeds.Fast;
        public bool ResearchEnabled { get; set; } = false;
        public FactionId StartingFaction { get; set; } = FactionId.Uee;
        public bool LargeCursor { get; set; } = false;
    }

    public enum AnimationSpeeds
    {
        Slow,
        Medium,
        Fast
    }
}