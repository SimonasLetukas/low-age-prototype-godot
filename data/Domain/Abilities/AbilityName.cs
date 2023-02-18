using low_age_data.Common;

namespace low_age_data.Domain.Abilities
{
    public class AbilityName : Name
    {
        private AbilityName(string value) : base($"ability-{value}")
        {
        }

        public static class Shared
        {
            public static AbilityName PassiveIncome => new AbilityName($"{nameof(Shared)}{nameof(PassiveIncome)}".ToKebabCase());
            public static AbilityName ScrapsIncome => new AbilityName($"{nameof(Shared)}{nameof(ScrapsIncome)}".ToKebabCase());
            public static AbilityName CelestiumIncome => new AbilityName($"{nameof(Shared)}{nameof(CelestiumIncome)}".ToKebabCase());

            public static class Revelators
            {
                public static AbilityName Building => new AbilityName($"{nameof(Shared)}{nameof(Revelators)}{nameof(Building)}".ToKebabCase());
            }

            public static class Uee
            {
                public static AbilityName Building => new AbilityName($"{nameof(Shared)}{nameof(Uee)}{nameof(Building)}".ToKebabCase());
                public static AbilityName PowerGenerator => new AbilityName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerGenerator)}".ToKebabCase());
                public static AbilityName Build => new AbilityName($"{nameof(Shared)}{nameof(Uee)}{nameof(Build)}".ToKebabCase());
                public static AbilityName PowerDependency => new AbilityName($"{nameof(Shared)}{nameof(Uee)}{nameof(PowerDependency)}".ToKebabCase());
            }
        }

        public static class Citadel
        {
            public static AbilityName ExecutiveStash => new AbilityName($"{nameof(Citadel)}{nameof(ExecutiveStash)}".ToKebabCase());
            public static AbilityName Ascendable => new AbilityName($"{nameof(Citadel)}{nameof(Ascendable)}".ToKebabCase());
            public static AbilityName HighGround => new AbilityName($"{nameof(Citadel)}{nameof(HighGround)}".ToKebabCase());
            public static AbilityName PromoteGoons => new AbilityName($"{nameof(Citadel)}{nameof(PromoteGoons)}".ToKebabCase());
        }

        public static class Hut
        {
            public static AbilityName Building => new AbilityName($"{nameof(Hut)}{nameof(Building)}".ToKebabCase());
        }

        public static class Obelisk
        {
            public static AbilityName Building => new AbilityName($"{nameof(Obelisk)}{nameof(Building)}".ToKebabCase());
            public static AbilityName CelestiumDischarge => new AbilityName($"{nameof(Obelisk)}{nameof(CelestiumDischarge)}".ToKebabCase());
        }

        public static class Shack
        {
            public static AbilityName Accommodation => new AbilityName($"{nameof(Shack)}{nameof(Accommodation)}".ToKebabCase());
        }

        public static class Smith
        {
            public static AbilityName MeleeWeaponProduction => new AbilityName($"{nameof(Smith)}{nameof(MeleeWeaponProduction)}".ToKebabCase());
        }

        public static class Fletcher
        {
            public static AbilityName RangedWeaponProduction => new AbilityName($"{nameof(Fletcher)}{nameof(RangedWeaponProduction)}".ToKebabCase());
        }

        public static class Alchemy
        {
            public static AbilityName SpecialWeaponProduction => new AbilityName($"{nameof(Alchemy)}{nameof(SpecialWeaponProduction)}".ToKebabCase());
        }

        public static class Depot
        {
            public static AbilityName WeaponStorage => new AbilityName($"{nameof(Depot)}{nameof(WeaponStorage)}".ToKebabCase());
        }

        public static class Workshop
        {
            public static AbilityName Research => new AbilityName($"{nameof(Workshop)}{nameof(Research)}".ToKebabCase());
        }

        public static class Outpost
        {
            public static AbilityName Ascendable => new AbilityName($"{nameof(Outpost)}{nameof(Ascendable)}".ToKebabCase());
            public static AbilityName HighGround => new AbilityName($"{nameof(Outpost)}{nameof(HighGround)}".ToKebabCase());
        }

        public static class Barricade
        {
            public static AbilityName ProtectiveShield => new AbilityName($"{nameof(Barricade)}{nameof(ProtectiveShield)}".ToKebabCase());
            public static AbilityName Caltrops => new AbilityName($"{nameof(Barricade)}{nameof(Caltrops)}".ToKebabCase());
            public static AbilityName Decompose => new AbilityName($"{nameof(Barricade)}{nameof(Decompose)}".ToKebabCase());
        }

        public static class BatteryCore
        {
            public static AbilityName PowerGrid => new AbilityName($"{nameof(BatteryCore)}{nameof(PowerGrid)}".ToKebabCase());
            public static AbilityName FusionCoreUpgrade => new AbilityName($"{nameof(BatteryCore)}{nameof(FusionCoreUpgrade)}".ToKebabCase());
        }

        public static class FusionCore
        {
            public static AbilityName PowerGrid => new AbilityName($"{nameof(FusionCore)}{nameof(PowerGrid)}".ToKebabCase());
            public static AbilityName DefenceProtocol => new AbilityName($"{nameof(FusionCore)}{nameof(DefenceProtocol)}".ToKebabCase());
            public static AbilityName CelestiumCoreUpgrade => new AbilityName($"{nameof(FusionCore)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase());
        }
        
        public static class CelestiumCore
        {
            public static AbilityName PowerGrid => new AbilityName($"{nameof(CelestiumCore)}{nameof(PowerGrid)}".ToKebabCase());
            public static AbilityName DefenceProtocol => new AbilityName($"{nameof(CelestiumCore)}{nameof(DefenceProtocol)}".ToKebabCase());
            public static AbilityName HeightenedConductivity => new AbilityName($"{nameof(CelestiumCore)}{nameof(HeightenedConductivity)}".ToKebabCase());
        }
        
        public static class Collector
        {
            public static AbilityName Building => new AbilityName($"{nameof(Collector)}{nameof(Building)}".ToKebabCase());
            public static AbilityName DirectTransitSystem => new AbilityName($"{nameof(Collector)}{nameof(DirectTransitSystem)}".ToKebabCase());
        }
        
        public static class Extractor
        {
            public static AbilityName Building => new AbilityName($"{nameof(Extractor)}{nameof(Building)}".ToKebabCase());
            public static AbilityName ReinforcedInfrastructure => new AbilityName($"{nameof(Extractor)}{nameof(ReinforcedInfrastructure)}".ToKebabCase());
        }
        
        public static class PowerPole
        {
            public static AbilityName PowerGrid => new AbilityName($"{nameof(PowerPole)}{nameof(PowerGrid)}".ToKebabCase());
            public static AbilityName ExcessDistribution => new AbilityName($"{nameof(PowerPole)}{nameof(ExcessDistribution)}".ToKebabCase());
            public static AbilityName ImprovedPowerGrid => new AbilityName($"{nameof(PowerPole)}{nameof(ImprovedPowerGrid)}".ToKebabCase());
            public static AbilityName PowerGridImproved => new AbilityName($"{nameof(PowerPole)}{nameof(PowerGridImproved)}".ToKebabCase());
            public static AbilityName ExcessDistributionImproved => new AbilityName($"{nameof(PowerPole)}{nameof(ExcessDistributionImproved)}".ToKebabCase());
        }
        
        public static class Temple
        {
            public static AbilityName KeepingTheFaith => new AbilityName($"{nameof(Temple)}{nameof(KeepingTheFaith)}".ToKebabCase());
        }
        
        public static class MilitaryBase
        {
            public static AbilityName Train => new AbilityName($"{nameof(MilitaryBase)}{nameof(Train)}".ToKebabCase());
        }
        
        public static class Factory
        {
            public static AbilityName Train => new AbilityName($"{nameof(Factory)}{nameof(Train)}".ToKebabCase());
        }
        
        public static class Laboratory
        {
            public static AbilityName Train => new AbilityName($"{nameof(Laboratory)}{nameof(Train)}".ToKebabCase());
        }
        
        public static class Armoury
        {
            public static AbilityName Research => new AbilityName($"{nameof(Armoury)}{nameof(Research)}".ToKebabCase());
        }
        
        public static class Wall
        {
            public static AbilityName Building => new AbilityName($"{nameof(Wall)}{nameof(Building)}".ToKebabCase());
            public static AbilityName HighGround => new AbilityName($"{nameof(Wall)}{nameof(HighGround)}".ToKebabCase());
        }
        
        public static class Stairs
        {
            public static AbilityName Ascendable => new AbilityName($"{nameof(Stairs)}{nameof(Ascendable)}".ToKebabCase());
        }
        
        public static class Gate
        {
            public static AbilityName HighGround => new AbilityName($"{nameof(Gate)}{nameof(HighGround)}".ToKebabCase());
            public static AbilityName Ascendable => new AbilityName($"{nameof(Gate)}{nameof(Ascendable)}".ToKebabCase());
            public static AbilityName Entrance => new AbilityName($"{nameof(Gate)}{nameof(Entrance)}".ToKebabCase());
        }
        
        public static class Watchtower
        {
            public static AbilityName VantagePoint => new AbilityName($"{nameof(Watchtower)}{nameof(VantagePoint)}".ToKebabCase());
        }
        
        public static class Bastion
        {
            public static AbilityName Battlement => new AbilityName($"{nameof(Bastion)}{nameof(Battlement)}".ToKebabCase());
        }

        public static class Leader
        {
            public static AbilityName AllForOne => new AbilityName($"{nameof(Leader)}{nameof(AllForOne)}".ToKebabCase());
            public static AbilityName MenacingPresence => new AbilityName($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase());
            public static AbilityName OneForAll => new AbilityName($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }

        public static class Slave
        {
            public static AbilityName Build => new AbilityName($"{nameof(Slave)}{nameof(Build)}".ToKebabCase());
            public static AbilityName Repair => new AbilityName($"{nameof(Slave)}{nameof(Repair)}".ToKebabCase());
            public static AbilityName ManualLabour => new AbilityName($"{nameof(Slave)}{nameof(ManualLabour)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static AbilityName Doubleshot => new AbilityName($"{nameof(Quickdraw)}{nameof(Doubleshot)}".ToKebabCase());
            public static AbilityName Cripple => new AbilityName($"{nameof(Quickdraw)}{nameof(Cripple)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static AbilityName FanaticSuicide => new AbilityName($"{nameof(Gorger)}{nameof(FanaticSuicide)}".ToKebabCase());
            public static AbilityName FanaticSuicidePassive => new AbilityName($"{nameof(Gorger)}{nameof(FanaticSuicidePassive)}".ToKebabCase());
        }

        public static class Camou
        {
            public static AbilityName SilentAssassin => new AbilityName($"{nameof(Camou)}{nameof(SilentAssassin)}".ToKebabCase());
            public static AbilityName Climb => new AbilityName($"{nameof(Camou)}{nameof(Climb)}".ToKebabCase());
            public static AbilityName ClimbPassive => new AbilityName($"{nameof(Camou)}{nameof(ClimbPassive)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static AbilityName WondrousGoo => new AbilityName($"{nameof(Shaman)}{nameof(WondrousGoo)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static AbilityName WallOfFlames => new AbilityName($"{nameof(Pyre)}{nameof(WallOfFlames)}".ToKebabCase());
            public static AbilityName PhantomMenace => new AbilityName($"{nameof(Pyre)}{nameof(PhantomMenace)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static AbilityName UnleashTheRage => new AbilityName($"{nameof(BigBadBull)}{nameof(UnleashTheRage)}".ToKebabCase());
        }

        public static class Mummy
        {
            public static AbilityName SpawnRoach => new AbilityName($"{nameof(Mummy)}{nameof(SpawnRoach)}".ToKebabCase());
            public static AbilityName SpawnRoachModified => new AbilityName($"{nameof(Mummy)}{nameof(SpawnRoachModified)}".ToKebabCase());
            public static AbilityName LeapOfHunger => new AbilityName($"{nameof(Mummy)}{nameof(LeapOfHunger)}".ToKebabCase());
        }

        public static class Roach
        {
            public static AbilityName DegradingCarapace => new AbilityName($"{nameof(Roach)}{nameof(DegradingCarapace)}".ToKebabCase());
            public static AbilityName CorrosiveSpit => new AbilityName($"{nameof(Roach)}{nameof(CorrosiveSpit)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static AbilityName ParalysingGrasp => new AbilityName($"{nameof(Parasite)}{nameof(ParalysingGrasp)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static AbilityName ExpertFormation => new AbilityName($"{nameof(Horrior)}{nameof(ExpertFormation)}".ToKebabCase());
            public static AbilityName Mount => new AbilityName($"{nameof(Horrior)}{nameof(Mount)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static AbilityName CriticalMark => new AbilityName($"{nameof(Marksman)}{nameof(CriticalMark)}".ToKebabCase());
        }

        public static class Surfer
        {
            public static AbilityName Dismount => new AbilityName($"{nameof(Surfer)}{nameof(Dismount)}".ToKebabCase());
        }

        public static class Mortar
        {
            public static AbilityName DeadlyAmmunition => new AbilityName($"{nameof(Mortar)}{nameof(DeadlyAmmunition)}".ToKebabCase());
            public static AbilityName Reload => new AbilityName($"{nameof(Mortar)}{nameof(Reload)}".ToKebabCase());
            public static AbilityName PiercingBlast => new AbilityName($"{nameof(Mortar)}{nameof(PiercingBlast)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static AbilityName TacticalGoggles => new AbilityName($"{nameof(Hawk)}{nameof(TacticalGoggles)}".ToKebabCase());
            public static AbilityName Leadership => new AbilityName($"{nameof(Hawk)}{nameof(Leadership)}".ToKebabCase());
            public static AbilityName HealthKit => new AbilityName($"{nameof(Hawk)}{nameof(HealthKit)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static AbilityName AssembleMachine => new AbilityName($"{nameof(Engineer)}{nameof(AssembleMachine)}".ToKebabCase());
            public static AbilityName Operate => new AbilityName($"{nameof(Engineer)}{nameof(Operate)}".ToKebabCase());
            public static AbilityName Repair => new AbilityName($"{nameof(Engineer)}{nameof(Repair)}".ToKebabCase());
        }

        public static class Cannon
        {
            public static AbilityName Assembling => new AbilityName($"{nameof(Cannon)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new AbilityName($"{nameof(Cannon)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName HeatUp => new AbilityName($"{nameof(Cannon)}{nameof(HeatUp)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static AbilityName Assembling => new AbilityName($"{nameof(Ballista)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new AbilityName($"{nameof(Ballista)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName AddOn => new AbilityName($"{nameof(Ballista)}{nameof(AddOn)}".ToKebabCase());
            public static AbilityName Aim => new AbilityName($"{nameof(Ballista)}{nameof(Aim)}".ToKebabCase());
        }

        public static class Radar
        {
            public static AbilityName Assembling => new AbilityName($"{nameof(Radar)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new AbilityName($"{nameof(Radar)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName ResonatingSweep => new AbilityName($"{nameof(Radar)}{nameof(ResonatingSweep)}".ToKebabCase());
            public static AbilityName RadioLocation => new AbilityName($"{nameof(Radar)}{nameof(RadioLocation)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static AbilityName Machine => new AbilityName($"{nameof(Vessel)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName AbsorbentField => new AbilityName($"{nameof(Vessel)}{nameof(AbsorbentField)}".ToKebabCase());
            public static AbilityName Fortify => new AbilityName($"{nameof(Vessel)}{nameof(Fortify)}".ToKebabCase());
        }

        public static class Omen
        {
            public static AbilityName Rendition => new AbilityName($"{nameof(Omen)}{nameof(Rendition)}".ToKebabCase());
            public static AbilityName RenditionPlacement => new AbilityName($"{nameof(Omen)}{nameof(RenditionPlacement)}".ToKebabCase());
        }
    }
}