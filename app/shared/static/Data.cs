using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data;
using low_age_data.Domain.Factions;
using low_age_data.Shared;
using Newtonsoft.Json;

/// <summary>
/// Handles game data for players, tiles, units & structures.
/// </summary>
public class Data : Node
{
    public static Data Instance = null;
    
    public Blueprint Blueprint { get; private set; }
    public IList<Player> Players { get; private set; } = new List<Player>();
    
    private const string DataBlueprintLocation = "res://data/data.json";

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }
    
    public void ReadBlueprint() 
        // TODO to save resources currently both server and client read from their local files,
        // it should be transferred by server instead to have one source of truth.
    {
        var dataFile = new File();
        dataFile.Open(DataBlueprintLocation, File.ModeFlags.Read);
        var dataJson = dataFile.GetAsText(); 
        dataFile.Close();

        Blueprint = JsonConvert.DeserializeObject<Blueprint>(dataJson, new JsonSerializerSettings
        {
            SerializationBinder = new KnownTypesBinder(),
            TypeNameHandling = TypeNameHandling.Auto
        });
    }

    public void AddPlayer(int playerId, string playerName, FactionId faction)
    {
        Players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            Faction = faction
        });
    }
    
    public void RemovePlayer(int id)
    {
        var playerToRemove = Players.SingleOrDefault(x => x.Id.Equals(id));
        if (playerToRemove is null)
        {
            return;
        }
        Players.Remove(playerToRemove);
    }

    public string GetPlayerName(int id) => Players.Single(x => x.Id.Equals(id)).Name;
}

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FactionId Faction { get; set; }
}