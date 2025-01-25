// See https://aka.ms/new-console-template for more information

using LowAgeData;
using LowAgeData.Collections;
using LowAgeCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

const string fileName = "data.json";

var resources = ResourcesCollection.Get();
var factions = FactionsCollection.Get();
var tiles = TilesCollection.Get();
var units = UnitsCollection.Get();
var structures = StructuresCollection.Get();
var doodads = DoodadsCollection.Get();
var abilities = AbilitiesCollection.Get();
var effects = EffectsCollection.Get();
var behaviours = BehavioursCollection.Get();
var masks = MasksCollection.Get();

var outputObject = new Blueprint
{
    Resources = resources,
    Factions = factions,
    Entities = new EntityBlueprint
    {
        Units = units,
        Structures = structures,
        Doodads = doodads
    },
    Tiles = tiles,
    Abilities = abilities,
    Effects = effects,
    Behaviours = behaviours,
    Masks = masks
};

var jsonSettings = new JsonSerializerSettings
{
    ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
        //NamingStrategy = new SnakeCaseNamingStrategy()
    },
    Formatting = Formatting.Indented,
    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
    SerializationBinder = new KnownTypesBinder(),
    TypeNameHandling = TypeNameHandling.Auto
};
jsonSettings.Converters.Add(new StringEnumConverter());
var outputJson = JsonConvert.SerializeObject(outputObject, jsonSettings);

var path = Path.Combine(
    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.FullName,
    fileName);

File.WriteAllText(path, outputJson);