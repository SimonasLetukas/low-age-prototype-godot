using System;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Abilities
{
    [JsonConverter(typeof(AbilityIdJsonConverter))]
    public class AbilityId : Id
    {
        [JsonConstructor]
        public AbilityId(string value, bool addPrefix = false) : base(addPrefix ? $"ability-{value}" : value)
        {
        }

        public static class Shared
        {
            public static AbilityId PassiveIncome => new AbilityId($"{nameof(Shared)}{nameof(PassiveIncome)}".ToKebabCase(), true);
            public static AbilityId ScrapsIncome => new AbilityId($"{nameof(Shared)}{nameof(ScrapsIncome)}".ToKebabCase(), true);
            public static AbilityId CelestiumIncome => new AbilityId($"{nameof(Shared)}{nameof(CelestiumIncome)}".ToKebabCase(), true);
            public static AbilityId UnitInProduction => new AbilityId($"{nameof(Shared)}{nameof(UnitInProduction)}".ToKebabCase(), true);

            public static class Revelators
            {
                public static AbilityId BuildingStructure => new AbilityId($"{nameof(Shared)}{nameof(Revelators)}{nameof(BuildingStructure)}".ToKebabCase(), true);
            }

            public static class Uee
            {
                public static AbilityId BuildingStructure => new AbilityId($"{nameof(Shared)}{nameof(Uee)}{nameof(BuildingStructure)}".ToKebabCase(), true);
                public static AbilityId PowerGenerator => new AbilityId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGenerator)}".ToKebabCase(), true);
                public static AbilityId Build => new AbilityId($"{nameof(Shared)}{nameof(Uee)}{nameof(Build)}".ToKebabCase(), true);
                public static AbilityId PowerDependency => new AbilityId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependency)}".ToKebabCase(), true);
            }
        }

        public static class Citadel
        {
            public static AbilityId ExecutiveStash => new AbilityId($"{nameof(Citadel)}{nameof(ExecutiveStash)}".ToKebabCase(), true);
            public static AbilityId Ascendable => new AbilityId($"{nameof(Citadel)}{nameof(Ascendable)}".ToKebabCase(), true);
            public static AbilityId HighGround => new AbilityId($"{nameof(Citadel)}{nameof(HighGround)}".ToKebabCase(), true);
            public static AbilityId PromoteGoons => new AbilityId($"{nameof(Citadel)}{nameof(PromoteGoons)}".ToKebabCase(), true);
        }

        public static class Hut
        {
            public static AbilityId Building => new AbilityId($"{nameof(Hut)}{nameof(Building)}".ToKebabCase(), true);
        }

        public static class Obelisk
        {
            public static AbilityId Building => new AbilityId($"{nameof(Obelisk)}{nameof(Building)}".ToKebabCase(), true);
            public static AbilityId CelestiumDischarge => new AbilityId($"{nameof(Obelisk)}{nameof(CelestiumDischarge)}".ToKebabCase(), true);
        }

        public static class Shack
        {
            public static AbilityId Accommodation => new AbilityId($"{nameof(Shack)}{nameof(Accommodation)}".ToKebabCase(), true);
        }

        public static class Smith
        {
            public static AbilityId MeleeWeaponProduction => new AbilityId($"{nameof(Smith)}{nameof(MeleeWeaponProduction)}".ToKebabCase(), true);
        }

        public static class Fletcher
        {
            public static AbilityId RangedWeaponProduction => new AbilityId($"{nameof(Fletcher)}{nameof(RangedWeaponProduction)}".ToKebabCase(), true);
        }

        public static class Alchemy
        {
            public static AbilityId SpecialWeaponProduction => new AbilityId($"{nameof(Alchemy)}{nameof(SpecialWeaponProduction)}".ToKebabCase(), true);
        }

        public static class Depot
        {
            public static AbilityId WeaponStorage => new AbilityId($"{nameof(Depot)}{nameof(WeaponStorage)}".ToKebabCase(), true);
        }

        public static class Workshop
        {
            public static AbilityId Research => new AbilityId($"{nameof(Workshop)}{nameof(Research)}".ToKebabCase(), true);
        }

        public static class Outpost
        {
            public static AbilityId Ascendable => new AbilityId($"{nameof(Outpost)}{nameof(Ascendable)}".ToKebabCase(), true);
            public static AbilityId HighGround => new AbilityId($"{nameof(Outpost)}{nameof(HighGround)}".ToKebabCase(), true);
        }

        public static class Barricade
        {
            public static AbilityId ProtectiveShield => new AbilityId($"{nameof(Barricade)}{nameof(ProtectiveShield)}".ToKebabCase(), true);
            public static AbilityId Caltrops => new AbilityId($"{nameof(Barricade)}{nameof(Caltrops)}".ToKebabCase(), true);
            public static AbilityId Decompose => new AbilityId($"{nameof(Barricade)}{nameof(Decompose)}".ToKebabCase(), true);
        }

        public static class BatteryCore
        {
            public static AbilityId PowerGrid => new AbilityId($"{nameof(BatteryCore)}{nameof(PowerGrid)}".ToKebabCase(), true);
            public static AbilityId FusionCoreUpgrade => new AbilityId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgrade)}".ToKebabCase(), true);
        }

        public static class FusionCore
        {
            public static AbilityId PowerGrid => new AbilityId($"{nameof(FusionCore)}{nameof(PowerGrid)}".ToKebabCase(), true);
            public static AbilityId DefenceProtocol => new AbilityId($"{nameof(FusionCore)}{nameof(DefenceProtocol)}".ToKebabCase(), true);
            public static AbilityId CelestiumCoreUpgrade => new AbilityId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase(), true);
        }
        
        public static class CelestiumCore
        {
            public static AbilityId PowerGrid => new AbilityId($"{nameof(CelestiumCore)}{nameof(PowerGrid)}".ToKebabCase(), true);
            public static AbilityId DefenceProtocol => new AbilityId($"{nameof(CelestiumCore)}{nameof(DefenceProtocol)}".ToKebabCase(), true);
            public static AbilityId HeightenedConductivity => new AbilityId($"{nameof(CelestiumCore)}{nameof(HeightenedConductivity)}".ToKebabCase(), true);
        }
        
        public static class Collector
        {
            public static AbilityId Building => new AbilityId($"{nameof(Collector)}{nameof(Building)}".ToKebabCase(), true);
            public static AbilityId DirectTransitSystem => new AbilityId($"{nameof(Collector)}{nameof(DirectTransitSystem)}".ToKebabCase(), true);
        }
        
        public static class Extractor
        {
            public static AbilityId Building => new AbilityId($"{nameof(Extractor)}{nameof(Building)}".ToKebabCase(), true);
            public static AbilityId ReinforcedInfrastructure => new AbilityId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructure)}".ToKebabCase(), true);
        }
        
        public static class PowerPole
        {
            public static AbilityId PowerGrid => new AbilityId($"{nameof(PowerPole)}{nameof(PowerGrid)}".ToKebabCase(), true);
            public static AbilityId ExcessDistribution => new AbilityId($"{nameof(PowerPole)}{nameof(ExcessDistribution)}".ToKebabCase(), true);
            public static AbilityId ImprovedPowerGrid => new AbilityId($"{nameof(PowerPole)}{nameof(ImprovedPowerGrid)}".ToKebabCase(), true);
            public static AbilityId PowerGridImproved => new AbilityId($"{nameof(PowerPole)}{nameof(PowerGridImproved)}".ToKebabCase(), true);
            public static AbilityId ExcessDistributionImproved => new AbilityId($"{nameof(PowerPole)}{nameof(ExcessDistributionImproved)}".ToKebabCase(), true);
        }
        
        public static class Temple
        {
            public static AbilityId KeepingTheFaith => new AbilityId($"{nameof(Temple)}{nameof(KeepingTheFaith)}".ToKebabCase(), true);
        }
        
        public static class MilitaryBase
        {
            public static AbilityId Train => new AbilityId($"{nameof(MilitaryBase)}{nameof(Train)}".ToKebabCase(), true);
        }
        
        public static class Factory
        {
            public static AbilityId Train => new AbilityId($"{nameof(Factory)}{nameof(Train)}".ToKebabCase(), true);
        }
        
        public static class Laboratory
        {
            public static AbilityId Train => new AbilityId($"{nameof(Laboratory)}{nameof(Train)}".ToKebabCase(), true);
        }
        
        public static class Armoury
        {
            public static AbilityId Research => new AbilityId($"{nameof(Armoury)}{nameof(Research)}".ToKebabCase(), true);
        }
        
        public static class Wall
        {
            public static AbilityId Building => new AbilityId($"{nameof(Wall)}{nameof(Building)}".ToKebabCase(), true);
            public static AbilityId HighGround => new AbilityId($"{nameof(Wall)}{nameof(HighGround)}".ToKebabCase(), true);
        }
        
        public static class Stairs
        {
            public static AbilityId Ascendable => new AbilityId($"{nameof(Stairs)}{nameof(Ascendable)}".ToKebabCase(), true);
        }
        
        public static class Gate
        {
            public static AbilityId HighGround => new AbilityId($"{nameof(Gate)}{nameof(HighGround)}".ToKebabCase(), true);
            public static AbilityId Ascendable => new AbilityId($"{nameof(Gate)}{nameof(Ascendable)}".ToKebabCase(), true);
            public static AbilityId Entrance => new AbilityId($"{nameof(Gate)}{nameof(Entrance)}".ToKebabCase(), true);
        }
        
        public static class Watchtower
        {
            public static AbilityId VantagePoint => new AbilityId($"{nameof(Watchtower)}{nameof(VantagePoint)}".ToKebabCase(), true);
        }
        
        public static class Bastion
        {
            public static AbilityId Battlement => new AbilityId($"{nameof(Bastion)}{nameof(Battlement)}".ToKebabCase(), true);
        }

        public static class Leader
        {
            public static AbilityId AllForOne => new AbilityId($"{nameof(Leader)}{nameof(AllForOne)}".ToKebabCase(), true);
            public static AbilityId MenacingPresence => new AbilityId($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase(), true);
            public static AbilityId OneForAll => new AbilityId($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase(), true);
        }

        public static class Slave
        {
            public static AbilityId Build => new AbilityId($"{nameof(Slave)}{nameof(Build)}".ToKebabCase(), true);
            public static AbilityId Repair => new AbilityId($"{nameof(Slave)}{nameof(Repair)}".ToKebabCase(), true);
            public static AbilityId ManualLabour => new AbilityId($"{nameof(Slave)}{nameof(ManualLabour)}".ToKebabCase(), true);
        }

        public static class Quickdraw
        {
            public static AbilityId Doubleshot => new AbilityId($"{nameof(Quickdraw)}{nameof(Doubleshot)}".ToKebabCase(), true);
            public static AbilityId Cripple => new AbilityId($"{nameof(Quickdraw)}{nameof(Cripple)}".ToKebabCase(), true);
        }

        public static class Gorger
        {
            public static AbilityId FanaticSuicide => new AbilityId($"{nameof(Gorger)}{nameof(FanaticSuicide)}".ToKebabCase(), true);
            public static AbilityId FanaticSuicidePassive => new AbilityId($"{nameof(Gorger)}{nameof(FanaticSuicidePassive)}".ToKebabCase(), true);
        }

        public static class Camou
        {
            public static AbilityId SilentAssassin => new AbilityId($"{nameof(Camou)}{nameof(SilentAssassin)}".ToKebabCase(), true);
            public static AbilityId Climb => new AbilityId($"{nameof(Camou)}{nameof(Climb)}".ToKebabCase(), true);
            public static AbilityId ClimbPassive => new AbilityId($"{nameof(Camou)}{nameof(ClimbPassive)}".ToKebabCase(), true);
        }

        public static class Shaman
        {
            public static AbilityId WondrousGoo => new AbilityId($"{nameof(Shaman)}{nameof(WondrousGoo)}".ToKebabCase(), true);
        }

        public static class Pyre
        {
            public static AbilityId WallOfFlames => new AbilityId($"{nameof(Pyre)}{nameof(WallOfFlames)}".ToKebabCase(), true);
            public static AbilityId PhantomMenace => new AbilityId($"{nameof(Pyre)}{nameof(PhantomMenace)}".ToKebabCase(), true);
        }

        public static class BigBadBull
        {
            public static AbilityId UnleashTheRage => new AbilityId($"{nameof(BigBadBull)}{nameof(UnleashTheRage)}".ToKebabCase(), true);
        }

        public static class Mummy
        {
            public static AbilityId SpawnRoach => new AbilityId($"{nameof(Mummy)}{nameof(SpawnRoach)}".ToKebabCase(), true);
            public static AbilityId SpawnRoachModified => new AbilityId($"{nameof(Mummy)}{nameof(SpawnRoachModified)}".ToKebabCase(), true);
            public static AbilityId LeapOfHunger => new AbilityId($"{nameof(Mummy)}{nameof(LeapOfHunger)}".ToKebabCase(), true);
        }

        public static class Roach
        {
            public static AbilityId DegradingCarapace => new AbilityId($"{nameof(Roach)}{nameof(DegradingCarapace)}".ToKebabCase(), true);
            public static AbilityId CorrosiveSpit => new AbilityId($"{nameof(Roach)}{nameof(CorrosiveSpit)}".ToKebabCase(), true);
        }

        public static class Parasite
        {
            public static AbilityId ParalysingGrasp => new AbilityId($"{nameof(Parasite)}{nameof(ParalysingGrasp)}".ToKebabCase(), true);
        }

        public static class Horrior
        {
            public static AbilityId ExpertFormation => new AbilityId($"{nameof(Horrior)}{nameof(ExpertFormation)}".ToKebabCase(), true);
            public static AbilityId Mount => new AbilityId($"{nameof(Horrior)}{nameof(Mount)}".ToKebabCase(), true);
        }

        public static class Marksman
        {
            public static AbilityId CriticalMark => new AbilityId($"{nameof(Marksman)}{nameof(CriticalMark)}".ToKebabCase(), true);
        }

        public static class Surfer
        {
            public static AbilityId Dismount => new AbilityId($"{nameof(Surfer)}{nameof(Dismount)}".ToKebabCase(), true);
        }

        public static class Mortar
        {
            public static AbilityId DeadlyAmmunition => new AbilityId($"{nameof(Mortar)}{nameof(DeadlyAmmunition)}".ToKebabCase(), true);
            public static AbilityId Reload => new AbilityId($"{nameof(Mortar)}{nameof(Reload)}".ToKebabCase(), true);
            public static AbilityId PiercingBlast => new AbilityId($"{nameof(Mortar)}{nameof(PiercingBlast)}".ToKebabCase(), true);
        }

        public static class Hawk
        {
            public static AbilityId TacticalGoggles => new AbilityId($"{nameof(Hawk)}{nameof(TacticalGoggles)}".ToKebabCase(), true);
            public static AbilityId Leadership => new AbilityId($"{nameof(Hawk)}{nameof(Leadership)}".ToKebabCase(), true);
            public static AbilityId HealthKit => new AbilityId($"{nameof(Hawk)}{nameof(HealthKit)}".ToKebabCase(), true);
        }

        public static class Engineer
        {
            public static AbilityId AssembleMachine => new AbilityId($"{nameof(Engineer)}{nameof(AssembleMachine)}".ToKebabCase(), true);
            public static AbilityId Operate => new AbilityId($"{nameof(Engineer)}{nameof(Operate)}".ToKebabCase(), true);
            public static AbilityId Repair => new AbilityId($"{nameof(Engineer)}{nameof(Repair)}".ToKebabCase(), true);
        }

        public static class Cannon
        {
            public static AbilityId Assembling => new AbilityId($"{nameof(Cannon)}{nameof(Assembling)}".ToKebabCase(), true);
            public static AbilityId Machine => new AbilityId($"{nameof(Cannon)}{nameof(Machine)}".ToKebabCase(), true);
            public static AbilityId HeatUp => new AbilityId($"{nameof(Cannon)}{nameof(HeatUp)}".ToKebabCase(), true);
        }

        public static class Ballista
        {
            public static AbilityId Assembling => new AbilityId($"{nameof(Ballista)}{nameof(Assembling)}".ToKebabCase(), true);
            public static AbilityId Machine => new AbilityId($"{nameof(Ballista)}{nameof(Machine)}".ToKebabCase(), true);
            public static AbilityId AddOn => new AbilityId($"{nameof(Ballista)}{nameof(AddOn)}".ToKebabCase(), true);
            public static AbilityId Aim => new AbilityId($"{nameof(Ballista)}{nameof(Aim)}".ToKebabCase(), true);
        }

        public static class Radar
        {
            public static AbilityId Assembling => new AbilityId($"{nameof(Radar)}{nameof(Assembling)}".ToKebabCase(), true);
            public static AbilityId Machine => new AbilityId($"{nameof(Radar)}{nameof(Machine)}".ToKebabCase(), true);
            public static AbilityId ResonatingSweep => new AbilityId($"{nameof(Radar)}{nameof(ResonatingSweep)}".ToKebabCase(), true);
            public static AbilityId RadioLocation => new AbilityId($"{nameof(Radar)}{nameof(RadioLocation)}".ToKebabCase(), true);
        }

        public static class Vessel
        {
            public static AbilityId Machine => new AbilityId($"{nameof(Vessel)}{nameof(Machine)}".ToKebabCase(), true);
            public static AbilityId AbsorbentField => new AbilityId($"{nameof(Vessel)}{nameof(AbsorbentField)}".ToKebabCase(), true);
            public static AbilityId Fortify => new AbilityId($"{nameof(Vessel)}{nameof(Fortify)}".ToKebabCase(), true);
        }

        public static class Omen
        {
            public static AbilityId Rendition => new AbilityId($"{nameof(Omen)}{nameof(Rendition)}".ToKebabCase(), true);
            public static AbilityId RenditionPlacement => new AbilityId($"{nameof(Omen)}{nameof(RenditionPlacement)}".ToKebabCase(), true);
        }
        
        private class AbilityIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(AbilityId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (AbilityId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new AbilityId(value ?? throw new InvalidOperationException());
            }
        }
    }
}