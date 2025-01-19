using System.Collections.Generic;
using System.Linq;
using Godot;
using low_age_data;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Factions;
using low_age_data.Shared;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

/// <summary>
/// Handles game data for players, tiles, units & structures.
/// </summary>
public partial class Data : Node
{
    public static Data Instance = null;
    
    public Blueprint Blueprint { get; private set; }
    public IList<Player> Players { get; private set; } = new List<Player>();
    public bool AllPlayersReady => Players.All(x => x.Ready);
    
    private const string DataBlueprintLocation = "res://data/data.json";

    public override void _Ready()
    {
        base._Ready();
        
        if (Instance is null)
        {
            Instance = this;
        }
    }
    
    public void Reset()
    {
        Blueprint = null;
        Players = new List<Player>();
    }
    
    public void ReadBlueprint() 
        // TODO to save resources currently both server and client read from their local files,
        // it should be transferred by server instead to have one source of truth.
    {
        var dataFile = FileAccess.Open(DataBlueprintLocation, FileAccess.ModeFlags.Read);
        var dataJson = dataFile.GetAsText(); 
        dataFile.Close();

        Blueprint = JsonConvert.DeserializeObject<Blueprint>(dataJson, new JsonSerializerSettings
        {
            SerializationBinder = new KnownTypesBinder(),
            TypeNameHandling = TypeNameHandling.Auto
        });
    }

    public void AddPlayer(int playerId, string playerName, bool ready, FactionId faction)
    {
        Players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            Ready = ready,
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

    public Entity GetEntityBlueprintById(EntityId id)
    {
        return (Blueprint.Entities.Units.FirstOrDefault(x => x.Id.Equals(id)) 
                ?? (Entity)Blueprint.Entities.Structures.FirstOrDefault(x => x.Id.Equals(id))) 
               ?? Blueprint.Entities.Doodads.First(x => x.Id.Equals(id));
    }
}

public partial class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Ready { get; set; }
    public FactionId Faction { get; set; }
}