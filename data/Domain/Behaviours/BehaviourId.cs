using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Behaviours
{
    [JsonConverter(typeof(BehaviourIdJsonConverter))]
    public class BehaviourId : Id
    {
        [JsonConstructor]
        public BehaviourId(string value, bool addPrefix = false) : base(addPrefix ? $"behaviour-{value}" : value)
        {
        }

        public static class Shared
        {
            public static BehaviourId HighGroundBuff => new BehaviourId($"{nameof(Shared)}{nameof(HighGroundBuff)}".ToKebabCase(), true);
            public static BehaviourId PassiveIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(PassiveIncomeIncome)}".ToKebabCase(), true);
            public static BehaviourId ScrapsIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(ScrapsIncomeIncome)}".ToKebabCase(), true);
            public static BehaviourId CelestiumIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(CelestiumIncomeIncome)}".ToKebabCase(), true);
            public static BehaviourId UnitInProductionBuildable => new BehaviourId($"{nameof(Shared)}{nameof(UnitInProductionBuildable)}".ToKebabCase(), true);

            public static class Revelators
            {
                public static BehaviourId BuildingStructureBuildable => new BehaviourId($"{nameof(Shared)}{nameof(Revelators)}{nameof(BuildingStructureBuildable)}".ToKebabCase(), true);
                public static BehaviourId NoPopulationSpaceInterceptDamage => new BehaviourId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceInterceptDamage)}".ToKebabCase(), true);
            }
            
            public static class Uee
            {
                public static BehaviourId BuildingStructureBuildable => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(BuildingStructureBuildable)}".ToKebabCase(), true);
                public static BehaviourId PowerGeneratorBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorBuff)}".ToKebabCase(), true);
                public static BehaviourId PowerDependencyBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuff)}".ToKebabCase(), true);
                public static BehaviourId PowerDependencyBuffDisable => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffDisable)}".ToKebabCase(), true);
                public static BehaviourId PowerDependencyBuffInactive => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffInactive)}".ToKebabCase(), true);
                public static BehaviourId PositiveFaithBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithBuff)}".ToKebabCase(), true);
            }
        }
        
        public static class Citadel
        {
            public static BehaviourId ExecutiveStashIncome => new BehaviourId($"{nameof(Citadel)}{nameof(ExecutiveStashIncome)}".ToKebabCase(), true);
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Citadel)}{nameof(AscendableAscendable)}".ToKebabCase(), true);
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Citadel)}{nameof(HighGroundHighGround)}".ToKebabCase(), true);
        }

        public static class Hut
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Hut)}{nameof(BuildingBuildable)}".ToKebabCase(), true);
        }
        
        public static class Obelisk
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Obelisk)}{nameof(BuildingBuildable)}".ToKebabCase(), true);
            public static BehaviourId CelestiumDischargeBuffLong => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffLong)}".ToKebabCase(), true);
            public static BehaviourId CelestiumDischargeBuffShort => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffShort)}".ToKebabCase(), true);
            public static BehaviourId CelestiumDischargeBuffNegative => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffNegative)}".ToKebabCase(), true);
        }
        
        public static class Shack
        {
            public static BehaviourId AccommodationIncome => new BehaviourId($"{nameof(Shack)}{nameof(AccommodationIncome)}".ToKebabCase(), true);
        }
        
        public static class Smith
        {
            public static BehaviourId MeleeWeaponProductionIncome => new BehaviourId($"{nameof(Smith)}{nameof(MeleeWeaponProductionIncome)}".ToKebabCase(), true);
        }
        
        public static class Fletcher
        {
            public static BehaviourId RangedWeaponProductionIncome => new BehaviourId($"{nameof(Fletcher)}{nameof(RangedWeaponProductionIncome)}".ToKebabCase(), true);
        }
        
        public static class Alchemy
        {
            public static BehaviourId SpecialWeaponProductionIncome => new BehaviourId($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionIncome)}".ToKebabCase(), true);
        }
        
        public static class Depot
        {
            public static BehaviourId WeaponStorageIncome => new BehaviourId($"{nameof(Depot)}{nameof(WeaponStorageIncome)}".ToKebabCase(), true);
        }
        
        public static class Workshop
        {
        }
        
        public static class Outpost
        {
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Outpost)}{nameof(AscendableAscendable)}".ToKebabCase(), true);
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Outpost)}{nameof(HighGroundHighGround)}".ToKebabCase(), true);
        }
        
        public static class Barricade
        {
            public static BehaviourId ProtectiveShieldBuff => new BehaviourId($"{nameof(Barricade)}{nameof(ProtectiveShieldBuff)}".ToKebabCase(), true);
            public static BehaviourId DecomposeBuff => new BehaviourId($"{nameof(Barricade)}{nameof(DecomposeBuff)}".ToKebabCase(), true);
        }
        
        public static class BatteryCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(BatteryCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase(), true);
            public static BehaviourId FusionCoreUpgradeBuff => new BehaviourId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeBuff)}".ToKebabCase(), true);
        }
        
        public static class FusionCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(FusionCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase(), true);
            public static BehaviourId CelestiumCoreUpgradeBuff => new BehaviourId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeBuff)}".ToKebabCase(), true);
        }
        
        public static class CelestiumCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(CelestiumCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase(), true);
        }
        
        public static class Collector
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Collector)}{nameof(BuildingBuildable)}".ToKebabCase(), true);
            public static BehaviourId DirectTransitSystemInactiveBuff => new BehaviourId($"{nameof(Collector)}{nameof(DirectTransitSystemInactiveBuff)}".ToKebabCase(), true);
            public static BehaviourId DirectTransitSystemActiveIncome => new BehaviourId($"{nameof(Collector)}{nameof(DirectTransitSystemActiveIncome)}".ToKebabCase(), true);
        }
        
        public static class Extractor
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Extractor)}{nameof(BuildingBuildable)}".ToKebabCase(), true);
            public static BehaviourId ReinforcedInfrastructureInactiveBuff => new BehaviourId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureInactiveBuff)}".ToKebabCase(), true);
            public static BehaviourId ReinforcedInfrastructureActiveBuff => new BehaviourId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureActiveBuff)}".ToKebabCase(), true);
        }

        public static class PowerPole
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(PowerPole)}{nameof(PowerGridMaskProvider)}".ToKebabCase(), true);
            public static BehaviourId ExcessDistributionBuff => new BehaviourId($"{nameof(PowerPole)}{nameof(ExcessDistributionBuff)}".ToKebabCase(), true);
            public static BehaviourId PowerGridImprovedMaskProvider => new BehaviourId($"{nameof(PowerPole)}{nameof(PowerGridImprovedMaskProvider)}".ToKebabCase(), true);
        }
        
        public static class Temple
        {
            public static BehaviourId KeepingTheFaithBuff => new BehaviourId($"{nameof(Temple)}{nameof(KeepingTheFaithBuff)}".ToKebabCase(), true);
            public static BehaviourId KeepingTheFaithIncome => new BehaviourId($"{nameof(Temple)}{nameof(KeepingTheFaithIncome)}".ToKebabCase(), true);
        }
        
        public static class Wall
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Wall)}{nameof(BuildingBuildable)}".ToKebabCase(), true);
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Wall)}{nameof(HighGroundHighGround)}".ToKebabCase(), true);
        }

        public static class Stairs
        {
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Stairs)}{nameof(AscendableAscendable)}".ToKebabCase(), true);
        }

        public static class Gate
        {
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Gate)}{nameof(HighGroundHighGround)}".ToKebabCase(), true);
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Gate)}{nameof(AscendableAscendable)}".ToKebabCase(), true);
            public static BehaviourId EntranceMovementBlock => new BehaviourId($"{nameof(Gate)}{nameof(EntranceMovementBlock)}".ToKebabCase(), true);
        }

        public static class Watchtower
        {
            public static BehaviourId VantagePointHighGround => new BehaviourId($"{nameof(Watchtower)}{nameof(VantagePointHighGround)}".ToKebabCase(), true);
            public static BehaviourId VantagePointBuff => new BehaviourId($"{nameof(Watchtower)}{nameof(VantagePointBuff)}".ToKebabCase(), true);
        }

        public static class Bastion
        {
            public static BehaviourId BattlementHighGround => new BehaviourId($"{nameof(Bastion)}{nameof(BattlementHighGround)}".ToKebabCase(), true);
            public static BehaviourId BattlementBuff => new BehaviourId($"{nameof(Bastion)}{nameof(BattlementBuff)}".ToKebabCase(), true);
        }

        public static class Leader
        {
            public static BehaviourId AllForOneBuff => new BehaviourId($"{nameof(Leader)}{nameof(AllForOneBuff)}".ToKebabCase(), true);
            public static BehaviourId MenacingPresenceBuff => new BehaviourId($"{nameof(Leader)}{nameof(MenacingPresenceBuff)}".ToKebabCase(), true);
            public static BehaviourId OneForAllObeliskBuff => new BehaviourId($"{nameof(Leader)}{nameof(OneForAllObeliskBuff)}".ToKebabCase(), true);
            public static BehaviourId OneForAllHealBuff => new BehaviourId($"{nameof(Leader)}{nameof(OneForAllHealBuff)}".ToKebabCase(), true);
        }

        public static class Slave
        {
            public static BehaviourId RepairStructureBuff => new BehaviourId($"{nameof(Slave)}{nameof(RepairStructureBuff)}".ToKebabCase(), true);
            public static BehaviourId RepairWait => new BehaviourId($"{nameof(Slave)}{nameof(RepairWait)}".ToKebabCase(), true);
            public static BehaviourId ManualLabourBuff => new BehaviourId($"{nameof(Slave)}{nameof(ManualLabourBuff)}".ToKebabCase(), true);
            public static BehaviourId ManualLabourWait => new BehaviourId($"{nameof(Slave)}{nameof(ManualLabourWait)}".ToKebabCase(), true);
        }

        public static class Quickdraw
        {
            public static BehaviourId DoubleshotExtraAttack => new BehaviourId($"{nameof(Quickdraw)}{nameof(DoubleshotExtraAttack)}".ToKebabCase(), true);
            public static BehaviourId CrippleBuff => new BehaviourId($"{nameof(Quickdraw)}{nameof(CrippleBuff)}".ToKebabCase(), true);
        }

        public static class Gorger
        {
            public static BehaviourId FanaticSuicideBuff => new BehaviourId($"{nameof(Gorger)}{nameof(FanaticSuicideBuff)}".ToKebabCase(), true);
        }

        public static class Camou
        {
            public static BehaviourId SilentAssassinBuff => new BehaviourId($"{nameof(Camou)}{nameof(SilentAssassinBuff)}".ToKebabCase(), true);
            public static BehaviourId ClimbWait => new BehaviourId($"{nameof(Camou)}{nameof(ClimbWait)}".ToKebabCase(), true);
            public static BehaviourId ClimbBuff => new BehaviourId($"{nameof(Camou)}{nameof(ClimbBuff)}".ToKebabCase(), true);
        }

        public static class Shaman
        {
            public static BehaviourId WondrousGooFeatureWait => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooFeatureWait)}".ToKebabCase(), true);
            public static BehaviourId WondrousGooFeatureBuff => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooFeatureBuff)}".ToKebabCase(), true);
            public static BehaviourId WondrousGooBuff => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooBuff)}".ToKebabCase(), true);
        }

        public static class Pyre
        {
            public static BehaviourId CargoTether => new BehaviourId($"{nameof(Pyre)}{nameof(CargoTether)}".ToKebabCase(), true);
            public static BehaviourId CargoWallOfFlamesBuff => new BehaviourId($"{nameof(Pyre)}{nameof(CargoWallOfFlamesBuff)}".ToKebabCase(), true);
            public static BehaviourId WallOfFlamesBuff => new BehaviourId($"{nameof(Pyre)}{nameof(WallOfFlamesBuff)}".ToKebabCase(), true);
            public static BehaviourId PhantomMenaceBuff => new BehaviourId($"{nameof(Pyre)}{nameof(PhantomMenaceBuff)}".ToKebabCase(), true);
        }

        public static class Roach
        {
            public static BehaviourId DegradingCarapaceBuff => new BehaviourId($"{nameof(Roach)}{nameof(DegradingCarapaceBuff)}".ToKebabCase(), true);
            public static BehaviourId DegradingCarapacePeriodicDamageBuff => new BehaviourId($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicDamageBuff)}".ToKebabCase(), true);
        }

        public static class Parasite
        {
            public static BehaviourId ParalysingGraspTether => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspTether)}".ToKebabCase(), true);
            public static BehaviourId ParalysingGraspBuff => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspBuff)}".ToKebabCase(), true);
            public static BehaviourId ParalysingGraspSelfBuff => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspSelfBuff)}".ToKebabCase(), true);
        }

        public static class Horrior
        {
            public static BehaviourId ExpertFormationBuff => new BehaviourId($"{nameof(Horrior)}{nameof(ExpertFormationBuff)}".ToKebabCase(), true);
            public static BehaviourId MountWait => new BehaviourId($"{nameof(Horrior)}{nameof(MountWait)}".ToKebabCase(), true);
            public static BehaviourId MountBuff => new BehaviourId($"{nameof(Horrior)}{nameof(MountBuff)}".ToKebabCase(), true);
        }

        public static class Marksman
        {
            public static BehaviourId CriticalMarkBuff => new BehaviourId($"{nameof(Marksman)}{nameof(CriticalMarkBuff)}".ToKebabCase(), true);
        }
        
        public static class Surfer
        {
            public static BehaviourId DismountBuff => new BehaviourId($"{nameof(Surfer)}{nameof(DismountBuff)}".ToKebabCase(), true);
        }

        public static class Mortar
        {
            public static BehaviourId DeadlyAmmunitionAmmunition => new BehaviourId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionAmmunition)}".ToKebabCase(), true);
            public static BehaviourId ReloadWait => new BehaviourId($"{nameof(Mortar)}{nameof(ReloadWait)}".ToKebabCase(), true);
            public static BehaviourId ReloadBuff => new BehaviourId($"{nameof(Mortar)}{nameof(ReloadBuff)}".ToKebabCase(), true);
            public static BehaviourId PiercingBlastBuff => new BehaviourId($"{nameof(Mortar)}{nameof(PiercingBlastBuff)}".ToKebabCase(), true);
        }

        public static class Hawk
        {
            public static BehaviourId TacticalGogglesBuff => new BehaviourId($"{nameof(Hawk)}{nameof(TacticalGogglesBuff)}".ToKebabCase(), true);
            public static BehaviourId LeadershipBuff => new BehaviourId($"{nameof(Hawk)}{nameof(LeadershipBuff)}".ToKebabCase(), true);
            public static BehaviourId HealthKitBuff => new BehaviourId($"{nameof(Hawk)}{nameof(HealthKitBuff)}".ToKebabCase(), true);
            public static BehaviourId HealthKitHealBuff => new BehaviourId($"{nameof(Hawk)}{nameof(HealthKitHealBuff)}".ToKebabCase(), true);
        }

        public static class Engineer
        {
            public static BehaviourId OperateBuff => new BehaviourId($"{nameof(Engineer)}{nameof(OperateBuff)}".ToKebabCase(), true);
            public static BehaviourId RepairStructureOrMachineBuff => new BehaviourId($"{nameof(Engineer)}{nameof(RepairStructureOrMachineBuff)}".ToKebabCase(), true);
            public static BehaviourId RepairHorriorBuff => new BehaviourId($"{nameof(Engineer)}{nameof(RepairHorriorBuff)}".ToKebabCase(), true);
            public static BehaviourId RepairWait => new BehaviourId($"{nameof(Engineer)}{nameof(RepairWait)}".ToKebabCase(), true);
        }

        public static class Cannon
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Cannon)}{nameof(AssemblingBuildable)}".ToKebabCase(), true);
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Cannon)}{nameof(MachineCounter)}".ToKebabCase(), true);
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Cannon)}{nameof(MachineBuff)}".ToKebabCase(), true);
            public static BehaviourId HeatUpDangerZoneBuff => new BehaviourId($"{nameof(Cannon)}{nameof(HeatUpDangerZoneBuff)}".ToKebabCase(), true);
            public static BehaviourId HeatUpWait => new BehaviourId($"{nameof(Cannon)}{nameof(HeatUpWait)}".ToKebabCase(), true);
        }

        public static class Ballista
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Ballista)}{nameof(AssemblingBuildable)}".ToKebabCase(), true);
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Ballista)}{nameof(MachineCounter)}".ToKebabCase(), true);
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Ballista)}{nameof(MachineBuff)}".ToKebabCase(), true);
            public static BehaviourId AimBuff => new BehaviourId($"{nameof(Ballista)}{nameof(AimBuff)}".ToKebabCase(), true);
        }

        public static class Radar
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Radar)}{nameof(AssemblingBuildable)}".ToKebabCase(), true);
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Radar)}{nameof(MachineCounter)}".ToKebabCase(), true);
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Radar)}{nameof(MachineBuff)}".ToKebabCase(), true);
            public static BehaviourId ResonatingSweepBuff => new BehaviourId($"{nameof(Radar)}{nameof(ResonatingSweepBuff)}".ToKebabCase(), true);
            public static BehaviourId RadioLocationBuff => new BehaviourId($"{nameof(Radar)}{nameof(RadioLocationBuff)}".ToKebabCase(), true);
            public static BehaviourId RadioLocationFeatureBuff => new BehaviourId($"{nameof(Radar)}{nameof(RadioLocationFeatureBuff)}".ToKebabCase(), true);
        }

        public static class Vessel
        {
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Vessel)}{nameof(MachineCounter)}".ToKebabCase(), true);
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Vessel)}{nameof(MachineBuff)}".ToKebabCase(), true);
            public static BehaviourId AbsorbentFieldInterceptDamage => new BehaviourId($"{nameof(Vessel)}{nameof(AbsorbentFieldInterceptDamage)}".ToKebabCase(), true);
            public static BehaviourId FortifyDestroyBuff => new BehaviourId($"{nameof(Vessel)}{nameof(FortifyDestroyBuff)}".ToKebabCase(), true);
            public static BehaviourId FortifyBuff => new BehaviourId($"{nameof(Vessel)}{nameof(FortifyBuff)}".ToKebabCase(), true);
        }

        public static class Omen
        {
            public static BehaviourId RenditionPlacementBuff => new BehaviourId($"{nameof(Omen)}{nameof(RenditionPlacementBuff)}".ToKebabCase(), true);
            public static BehaviourId RenditionInterceptDamage => new BehaviourId($"{nameof(Omen)}{nameof(RenditionInterceptDamage)}".ToKebabCase(), true);
            public static BehaviourId RenditionBuffTimer => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffTimer)}".ToKebabCase(), true);
            public static BehaviourId RenditionBuffDeath => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffDeath)}".ToKebabCase(), true);
            public static BehaviourId RenditionBuffSlow => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffSlow)}".ToKebabCase(), true);
        }
        
        private class BehaviourIdJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(BehaviourId);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (BehaviourId)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new BehaviourId(value ?? throw new InvalidOperationException());
            }
        }
    }
}
