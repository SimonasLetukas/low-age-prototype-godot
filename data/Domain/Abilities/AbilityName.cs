using low_age_data.Common;

namespace low_age_data.Domain.Abilities
{
    public class AbilityName : Name
    {
        private AbilityName(string value) : base($"ability-{value}")
        {
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
            public static AbilityName PowerDependency => new AbilityName($"{nameof(Radar)}{nameof(PowerDependency)}".ToKebabCase());
            public static AbilityName ResonatingSweep => new AbilityName($"{nameof(Radar)}{nameof(ResonatingSweep)}".ToKebabCase());
            public static AbilityName RadioLocation => new AbilityName($"{nameof(Radar)}{nameof(RadioLocation)}".ToKebabCase());
        }

        public static class Vessel
        {
            
        }
        
        public static class Omen
        {
            
        }
    }
}
