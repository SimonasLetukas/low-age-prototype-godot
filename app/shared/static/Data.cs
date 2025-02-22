using System.Linq;
using Godot;
using LowAgeData;
using LowAgeData.Domain.Entities;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

/// <summary>
/// Handles game data blueprint.
/// </summary>
public partial class Data : Node
{
    public static Data Instance = null!;

    public Blueprint Blueprint { get; private set; } = null!;
    
    private const string DataBlueprintLocation = "res://data/data.json";

    public override void _Ready()
    {
        base._Ready();
        
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }
    
    public void Reset()
    {
        Blueprint = null!;
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
        })!;
    }

    public Entity GetEntityBlueprintById(EntityId id)
    {
        return (Blueprint.Entities.Units.FirstOrDefault(x => x.Id.Equals(id)) 
                ?? Blueprint.Entities.Structures.FirstOrDefault(x => x.Id.Equals(id)) as Entity) 
               ?? Blueprint.Entities.Doodads.First(x => x.Id.Equals(id));
    }
}