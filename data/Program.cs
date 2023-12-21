using low_age_data.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using low_age_data.Shared;

namespace low_age_data
{
    class Program
    {
        private const string FileName = "data.json";

        static void Main(string[] args)
        {
            var resources = Resources.Get();
            var factions = Factions.Get();
            var tiles = Tiles.Get();
            var units = Units.Get();
            var structures = Structures.Get();
            var doodads = Doodads.Get();
            var abilities = Abilities.Get();
            var effects = Effects.Get();
            var behaviours = Behaviours.Get();
            var masks = Masks.Get();

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
                FileName);

            File.WriteAllText(path, outputJson);
        }
    }
}