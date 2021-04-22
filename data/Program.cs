using low_age_data.Collections;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Features;
using low_age_data.Domain.Entities.Tiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace low_age_data
{
    class Program
    {
        private const string FileName = "data.json";

        static void Main(string[] args)
        {
            var tiles = Tiles.Get();
            var units = Units.Get();
            var structures = Structures.Get();
            var features = Features.Get();
            var abilities = Abilities.Get();
            var effects = Effects.Get();
            var behaviours = Behaviours.Get();

            var outputObject = new OutputObject
            {
                Entities = new OutputEntities
                {
                    Tiles = tiles,
                    Units = units,
                    Structures = structures,
                    Features = features
                },
                Abilities = abilities,
                Effects = effects,
                Behaviours = behaviours
            };

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    //NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            jsonSettings.Converters.Add(new StringEnumConverter());
            var outputJson = JsonConvert.SerializeObject(outputObject, jsonSettings);

            var path = Path.Combine(
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory 
                                    ?? Directory.GetCurrentDirectory())
                    .Parent.Parent.Parent.FullName, 
                FileName);

            File.WriteAllText(path, outputJson);
        }
    }

    class OutputObject
    {
        public OutputEntities Entities { get; set; } = new OutputEntities();
        public IList<Ability> Abilities { get; set; } = new List<Ability>();
        public IList<Effect> Effects { get; set; } = new List<Effect>();
        public IList<Behaviour> Behaviours { get; set; } = new List<Behaviour>();
    }

    class OutputEntities
    {
        public IList<Tile> Tiles { get; set; } = new List<Tile>();
        public IList<Unit> Units { get; set; } = new List<Unit>();
        public IList<Structure> Structures { get; set; } = new List<Structure>();
        public IList<Feature> Features { get; set; } = new List<Feature>();
    }
}
