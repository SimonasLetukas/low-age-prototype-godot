using System;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Behaviours
{
    [JsonConverter(typeof(BehaviourIdJsonConverter))]
    public class BehaviourId : Id
    {
        private BehaviourId(string value) : base($"behaviour-{value}")
        {
        }

        public static class Shared
        {
            public static BehaviourId HighGroundBuff => new BehaviourId($"{nameof(Shared)}{nameof(HighGroundBuff)}".ToKebabCase());
            public static BehaviourId PassiveIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(PassiveIncomeIncome)}".ToKebabCase());
            public static BehaviourId ScrapsIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(ScrapsIncomeIncome)}".ToKebabCase());
            public static BehaviourId CelestiumIncomeIncome => new BehaviourId($"{nameof(Shared)}{nameof(CelestiumIncomeIncome)}".ToKebabCase());

            public static class Revelators
            {
                public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Shared)}{nameof(Revelators)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourId NoPopulationSpaceInterceptDamage => new BehaviourId($"{nameof(Shared)}{nameof(Revelators)}{nameof(NoPopulationSpaceInterceptDamage)}".ToKebabCase());
            }
            
            public static class Uee
            {
                public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(BuildingBuildable)}".ToKebabCase());
                public static BehaviourId PowerGeneratorBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGeneratorBuff)}".ToKebabCase());
                public static BehaviourId PowerDependencyBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuff)}".ToKebabCase());
                public static BehaviourId PowerDependencyBuffDisable => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffDisable)}".ToKebabCase());
                public static BehaviourId PowerDependencyBuffInactive => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependencyBuffInactive)}".ToKebabCase());
                public static BehaviourId PositiveFaithBuff => new BehaviourId($"{nameof(Shared)}{nameof(Uee)}{nameof(PositiveFaithBuff)}".ToKebabCase());
            }
        }
        
        public static class Citadel
        {
            public static BehaviourId ExecutiveStashIncome => new BehaviourId($"{nameof(Citadel)}{nameof(ExecutiveStashIncome)}".ToKebabCase());
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Citadel)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Citadel)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Hut
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Hut)}{nameof(BuildingBuildable)}".ToKebabCase());
        }
        
        public static class Obelisk
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Obelisk)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourId CelestiumDischargeBuffLong => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffLong)}".ToKebabCase());
            public static BehaviourId CelestiumDischargeBuffShort => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffShort)}".ToKebabCase());
            public static BehaviourId CelestiumDischargeBuffNegative => new BehaviourId($"{nameof(Obelisk)}{nameof(CelestiumDischargeBuffNegative)}".ToKebabCase());
        }
        
        public static class Shack
        {
            public static BehaviourId AccommodationIncome => new BehaviourId($"{nameof(Shack)}{nameof(AccommodationIncome)}".ToKebabCase());
        }
        
        public static class Smith
        {
            public static BehaviourId MeleeWeaponProductionIncome => new BehaviourId($"{nameof(Smith)}{nameof(MeleeWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Fletcher
        {
            public static BehaviourId RangedWeaponProductionIncome => new BehaviourId($"{nameof(Fletcher)}{nameof(RangedWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Alchemy
        {
            public static BehaviourId SpecialWeaponProductionIncome => new BehaviourId($"{nameof(Alchemy)}{nameof(SpecialWeaponProductionIncome)}".ToKebabCase());
        }
        
        public static class Depot
        {
            public static BehaviourId WeaponStorageIncome => new BehaviourId($"{nameof(Depot)}{nameof(WeaponStorageIncome)}".ToKebabCase());
        }
        
        public static class Workshop
        {
        }
        
        public static class Outpost
        {
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Outpost)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Outpost)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }
        
        public static class Barricade
        {
            public static BehaviourId ProtectiveShieldBuff => new BehaviourId($"{nameof(Barricade)}{nameof(ProtectiveShieldBuff)}".ToKebabCase());
            public static BehaviourId DecomposeBuff => new BehaviourId($"{nameof(Barricade)}{nameof(DecomposeBuff)}".ToKebabCase());
        }
        
        public static class BatteryCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(BatteryCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourId FusionCoreUpgradeBuff => new BehaviourId($"{nameof(BatteryCore)}{nameof(FusionCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class FusionCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(FusionCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourId CelestiumCoreUpgradeBuff => new BehaviourId($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgradeBuff)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(CelestiumCore)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Collector)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourId DirectTransitSystemInactiveBuff => new BehaviourId($"{nameof(Collector)}{nameof(DirectTransitSystemInactiveBuff)}".ToKebabCase());
            public static BehaviourId DirectTransitSystemActiveIncome => new BehaviourId($"{nameof(Collector)}{nameof(DirectTransitSystemActiveIncome)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Extractor)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourId ReinforcedInfrastructureInactiveBuff => new BehaviourId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureInactiveBuff)}".ToKebabCase());
            public static BehaviourId ReinforcedInfrastructureActiveBuff => new BehaviourId($"{nameof(Extractor)}{nameof(ReinforcedInfrastructureActiveBuff)}".ToKebabCase());
        }

        public static class PowerPole
        {
            public static BehaviourId PowerGridMaskProvider => new BehaviourId($"{nameof(PowerPole)}{nameof(PowerGridMaskProvider)}".ToKebabCase());
            public static BehaviourId ExcessDistributionBuff => new BehaviourId($"{nameof(PowerPole)}{nameof(ExcessDistributionBuff)}".ToKebabCase());
            public static BehaviourId PowerGridImprovedMaskProvider => new BehaviourId($"{nameof(PowerPole)}{nameof(PowerGridImprovedMaskProvider)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static BehaviourId KeepingTheFaithBuff => new BehaviourId($"{nameof(Temple)}{nameof(KeepingTheFaithBuff)}".ToKebabCase());
            public static BehaviourId KeepingTheFaithIncome => new BehaviourId($"{nameof(Temple)}{nameof(KeepingTheFaithIncome)}".ToKebabCase());
        }
        
        public static class Wall
        {
            public static BehaviourId BuildingBuildable => new BehaviourId($"{nameof(Wall)}{nameof(BuildingBuildable)}".ToKebabCase());
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Wall)}{nameof(HighGroundHighGround)}".ToKebabCase());
        }

        public static class Stairs
        {
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Stairs)}{nameof(AscendableAscendable)}".ToKebabCase());
        }

        public static class Gate
        {
            public static BehaviourId HighGroundHighGround => new BehaviourId($"{nameof(Gate)}{nameof(HighGroundHighGround)}".ToKebabCase());
            public static BehaviourId AscendableAscendable => new BehaviourId($"{nameof(Gate)}{nameof(AscendableAscendable)}".ToKebabCase());
            public static BehaviourId EntranceMovementBlock => new BehaviourId($"{nameof(Gate)}{nameof(EntranceMovementBlock)}".ToKebabCase());
        }

        public static class Watchtower
        {
            public static BehaviourId VantagePointBuff => new BehaviourId($"{nameof(Watchtower)}{nameof(VantagePointBuff)}".ToKebabCase());
        }

        public static class Bastion
        {
            public static BehaviourId BattlementBuff => new BehaviourId($"{nameof(Bastion)}{nameof(BattlementBuff)}".ToKebabCase());
        }

        public static class Leader
        {
            public static BehaviourId AllForOneBuff => new BehaviourId($"{nameof(Leader)}{nameof(AllForOneBuff)}".ToKebabCase());
            public static BehaviourId MenacingPresenceBuff => new BehaviourId($"{nameof(Leader)}{nameof(MenacingPresenceBuff)}".ToKebabCase());
            public static BehaviourId OneForAllObeliskBuff => new BehaviourId($"{nameof(Leader)}{nameof(OneForAllObeliskBuff)}".ToKebabCase());
            public static BehaviourId OneForAllHealBuff => new BehaviourId($"{nameof(Leader)}{nameof(OneForAllHealBuff)}".ToKebabCase());
        }

        public static class Slave
        {
            public static BehaviourId RepairStructureBuff => new BehaviourId($"{nameof(Slave)}{nameof(RepairStructureBuff)}".ToKebabCase());
            public static BehaviourId RepairWait => new BehaviourId($"{nameof(Slave)}{nameof(RepairWait)}".ToKebabCase());
            public static BehaviourId ManualLabourBuff => new BehaviourId($"{nameof(Slave)}{nameof(ManualLabourBuff)}".ToKebabCase());
            public static BehaviourId ManualLabourWait => new BehaviourId($"{nameof(Slave)}{nameof(ManualLabourWait)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static BehaviourId DoubleshotExtraAttack => new BehaviourId($"{nameof(Quickdraw)}{nameof(DoubleshotExtraAttack)}".ToKebabCase());
            public static BehaviourId CrippleBuff => new BehaviourId($"{nameof(Quickdraw)}{nameof(CrippleBuff)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static BehaviourId FanaticSuicideBuff => new BehaviourId($"{nameof(Gorger)}{nameof(FanaticSuicideBuff)}".ToKebabCase());
        }

        public static class Camou
        {
            public static BehaviourId SilentAssassinBuff => new BehaviourId($"{nameof(Camou)}{nameof(SilentAssassinBuff)}".ToKebabCase());
            public static BehaviourId ClimbWait => new BehaviourId($"{nameof(Camou)}{nameof(ClimbWait)}".ToKebabCase());
            public static BehaviourId ClimbBuff => new BehaviourId($"{nameof(Camou)}{nameof(ClimbBuff)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static BehaviourId WondrousGooFeatureWait => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooFeatureWait)}".ToKebabCase());
            public static BehaviourId WondrousGooFeatureBuff => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooFeatureBuff)}".ToKebabCase());
            public static BehaviourId WondrousGooBuff => new BehaviourId($"{nameof(Shaman)}{nameof(WondrousGooBuff)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static BehaviourId CargoTether => new BehaviourId($"{nameof(Pyre)}{nameof(CargoTether)}".ToKebabCase());
            public static BehaviourId CargoWallOfFlamesBuff => new BehaviourId($"{nameof(Pyre)}{nameof(CargoWallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourId WallOfFlamesBuff => new BehaviourId($"{nameof(Pyre)}{nameof(WallOfFlamesBuff)}".ToKebabCase());
            public static BehaviourId PhantomMenaceBuff => new BehaviourId($"{nameof(Pyre)}{nameof(PhantomMenaceBuff)}".ToKebabCase());
        }

        public static class Roach
        {
            public static BehaviourId DegradingCarapaceBuff => new BehaviourId($"{nameof(Roach)}{nameof(DegradingCarapaceBuff)}".ToKebabCase());
            public static BehaviourId DegradingCarapacePeriodicDamageBuff => new BehaviourId($"{nameof(Roach)}{nameof(DegradingCarapacePeriodicDamageBuff)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static BehaviourId ParalysingGraspTether => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspTether)}".ToKebabCase());
            public static BehaviourId ParalysingGraspBuff => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspBuff)}".ToKebabCase());
            public static BehaviourId ParalysingGraspSelfBuff => new BehaviourId($"{nameof(Parasite)}{nameof(ParalysingGraspSelfBuff)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static BehaviourId ExpertFormationBuff => new BehaviourId($"{nameof(Horrior)}{nameof(ExpertFormationBuff)}".ToKebabCase());
            public static BehaviourId MountWait => new BehaviourId($"{nameof(Horrior)}{nameof(MountWait)}".ToKebabCase());
            public static BehaviourId MountBuff => new BehaviourId($"{nameof(Horrior)}{nameof(MountBuff)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static BehaviourId CriticalMarkBuff => new BehaviourId($"{nameof(Marksman)}{nameof(CriticalMarkBuff)}".ToKebabCase());
        }
        
        public static class Surfer
        {
            public static BehaviourId DismountBuff => new BehaviourId($"{nameof(Surfer)}{nameof(DismountBuff)}".ToKebabCase());
        }

        public static class Mortar
        {
            public static BehaviourId DeadlyAmmunitionAmmunition => new BehaviourId($"{nameof(Mortar)}{nameof(DeadlyAmmunitionAmmunition)}".ToKebabCase());
            public static BehaviourId ReloadWait => new BehaviourId($"{nameof(Mortar)}{nameof(ReloadWait)}".ToKebabCase());
            public static BehaviourId ReloadBuff => new BehaviourId($"{nameof(Mortar)}{nameof(ReloadBuff)}".ToKebabCase());
            public static BehaviourId PiercingBlastBuff => new BehaviourId($"{nameof(Mortar)}{nameof(PiercingBlastBuff)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static BehaviourId TacticalGogglesBuff => new BehaviourId($"{nameof(Hawk)}{nameof(TacticalGogglesBuff)}".ToKebabCase());
            public static BehaviourId LeadershipBuff => new BehaviourId($"{nameof(Hawk)}{nameof(LeadershipBuff)}".ToKebabCase());
            public static BehaviourId HealthKitBuff => new BehaviourId($"{nameof(Hawk)}{nameof(HealthKitBuff)}".ToKebabCase());
            public static BehaviourId HealthKitHealBuff => new BehaviourId($"{nameof(Hawk)}{nameof(HealthKitHealBuff)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static BehaviourId OperateBuff => new BehaviourId($"{nameof(Engineer)}{nameof(OperateBuff)}".ToKebabCase());
            public static BehaviourId RepairStructureOrMachineBuff => new BehaviourId($"{nameof(Engineer)}{nameof(RepairStructureOrMachineBuff)}".ToKebabCase());
            public static BehaviourId RepairHorriorBuff => new BehaviourId($"{nameof(Engineer)}{nameof(RepairHorriorBuff)}".ToKebabCase());
            public static BehaviourId RepairWait => new BehaviourId($"{nameof(Engineer)}{nameof(RepairWait)}".ToKebabCase());
        }

        public static class Cannon
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Cannon)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Cannon)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Cannon)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourId HeatUpDangerZoneBuff => new BehaviourId($"{nameof(Cannon)}{nameof(HeatUpDangerZoneBuff)}".ToKebabCase());
            public static BehaviourId HeatUpWait => new BehaviourId($"{nameof(Cannon)}{nameof(HeatUpWait)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Ballista)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Ballista)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Ballista)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourId AimBuff => new BehaviourId($"{nameof(Ballista)}{nameof(AimBuff)}".ToKebabCase());
        }

        public static class Radar
        {
            public static BehaviourId AssemblingBuildable => new BehaviourId($"{nameof(Radar)}{nameof(AssemblingBuildable)}".ToKebabCase());
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Radar)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Radar)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourId ResonatingSweepBuff => new BehaviourId($"{nameof(Radar)}{nameof(ResonatingSweepBuff)}".ToKebabCase());
            public static BehaviourId RadioLocationBuff => new BehaviourId($"{nameof(Radar)}{nameof(RadioLocationBuff)}".ToKebabCase());
            public static BehaviourId RadioLocationFeatureBuff => new BehaviourId($"{nameof(Radar)}{nameof(RadioLocationFeatureBuff)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static BehaviourId MachineCounter => new BehaviourId($"{nameof(Vessel)}{nameof(MachineCounter)}".ToKebabCase());
            public static BehaviourId MachineBuff => new BehaviourId($"{nameof(Vessel)}{nameof(MachineBuff)}".ToKebabCase());
            public static BehaviourId AbsorbentFieldInterceptDamage => new BehaviourId($"{nameof(Vessel)}{nameof(AbsorbentFieldInterceptDamage)}".ToKebabCase());
            public static BehaviourId FortifyDestroyBuff => new BehaviourId($"{nameof(Vessel)}{nameof(FortifyDestroyBuff)}".ToKebabCase());
            public static BehaviourId FortifyBuff => new BehaviourId($"{nameof(Vessel)}{nameof(FortifyBuff)}".ToKebabCase());
        }

        public static class Omen
        {
            public static BehaviourId RenditionPlacementBuff => new BehaviourId($"{nameof(Omen)}{nameof(RenditionPlacementBuff)}".ToKebabCase());
            public static BehaviourId RenditionInterceptDamage => new BehaviourId($"{nameof(Omen)}{nameof(RenditionInterceptDamage)}".ToKebabCase());
            public static BehaviourId RenditionBuffTimer => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffTimer)}".ToKebabCase());
            public static BehaviourId RenditionBuffDeath => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffDeath)}".ToKebabCase());
            public static BehaviourId RenditionBuffSlow => new BehaviourId($"{nameof(Omen)}{nameof(RenditionBuffSlow)}".ToKebabCase());
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
