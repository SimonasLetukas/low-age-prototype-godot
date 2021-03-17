using System;
using System.Collections.Generic;
using System.IO;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Tiles;
using low_age_data.Domain.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace low_age_data
{
    class Program
    {
        private const string FileName = "data.json";

        static void Main(string[] args)
        {
            var tiles = new List<Tile>
            {
                new Tile(
                    TileName.Grass, 
                    nameof(TileName.Grass),
                    "",
                    Terrains.Grass,
                    1.0f,
                    true),
                new Tile(
                    TileName.Mountains,
                    nameof(TileName.Mountains),
                    "",
                    Terrains.Mountains,
                    0.0f,
                    false),
                new Tile(
                    TileName.Marsh,
                    nameof(TileName.Marsh),
                    "",
                    Terrains.Marsh,
                    2.0f,
                    false),
                new Tile(
                    TileName.Scraps,
                    nameof(TileName.Scraps),
                    "",
                    Terrains.Scraps,
                    1.0f,
                    true),
                new Tile(
                    TileName.Celestium,
                    nameof(TileName.Celestium),
                    "",
                    Terrains.Celestium,
                    1.0f,
                    true)
            };

            var units = new List<Unit>
            {
                new Unit(
                    UnitName.Slave,
                    nameof(UnitName.Slave),
                    "",
                    new List<Stat>
                    {
                        new CombatStat(6, true, Stats.Health),
                        new CombatStat(0, false, Stats.MeleeArmour),
                        new CombatStat(0, false, Stats.RangedArmour),
                        new CombatStat(5, true, Stats.Movement),
                        new CombatStat(20, false, Stats.Initiative),
                        new AttackStat(1, false, Attacks.Melee, 1, 1)
                    },
                    Factions.Revelators,
                    new List<CombatAttributes>
                    {
                        CombatAttributes.Light,
                        CombatAttributes.Biological
                    },
                    new List<AbilityName>
                    {
                        AbilityName.Slave.Build,
                        AbilityName.Slave.Repair,
                        AbilityName.Slave.ManualLabour
                    }),
            };

            var structures = new List<Structure>
            {

            };

            var abilities = new List<Ability>
            {

            };

            var effects = new List<Effect>
            {

            };

            var behaviours = new List<Behaviour>
            {

            };

            var outputObject = new OutputObject
            {
                Entities = new OutputEntities
                {
                    Tiles = tiles,
                    Units = units,
                    Structures = structures
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
    }
}
