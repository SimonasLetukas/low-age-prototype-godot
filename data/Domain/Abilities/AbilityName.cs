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
            public static AbilityName AllForOne => new($"{nameof(Leader)}{nameof(AllForOne)}".ToKebabCase());
            public static AbilityName MenacingPresence => new($"{nameof(Leader)}{nameof(MenacingPresence)}".ToKebabCase());
            public static AbilityName OneForAll => new($"{nameof(Leader)}{nameof(OneForAll)}".ToKebabCase());
        }

        public static class Slave
        {
            public static AbilityName Build => new($"{nameof(Slave)}{nameof(Build)}".ToKebabCase());
            public static AbilityName Repair => new($"{nameof(Slave)}{nameof(Repair)}".ToKebabCase());
            public static AbilityName ManualLabour => new($"{nameof(Slave)}{nameof(ManualLabour)}".ToKebabCase());
        }

        public static class Quickdraw
        {
            public static AbilityName Doubleshot => new($"{nameof(Quickdraw)}{nameof(Doubleshot)}".ToKebabCase());
            public static AbilityName Cripple => new($"{nameof(Quickdraw)}{nameof(Cripple)}".ToKebabCase());
        }

        public static class Gorger
        {
            public static AbilityName FanaticSuicide => new($"{nameof(Gorger)}{nameof(FanaticSuicide)}".ToKebabCase());
            public static AbilityName FanaticSuicidePassive => new($"{nameof(Gorger)}{nameof(FanaticSuicidePassive)}".ToKebabCase());
        }

        public static class Camou
        {
            public static AbilityName SilentAssassin => new($"{nameof(Camou)}{nameof(SilentAssassin)}".ToKebabCase());
            public static AbilityName Climb => new($"{nameof(Camou)}{nameof(Climb)}".ToKebabCase());
            public static AbilityName ClimbPassive => new($"{nameof(Camou)}{nameof(ClimbPassive)}".ToKebabCase());
        }

        public static class Shaman
        {
            public static AbilityName WondrousGoo => new($"{nameof(Shaman)}{nameof(WondrousGoo)}".ToKebabCase());
        }

        public static class Pyre
        {
            public static AbilityName WallOfFlames => new($"{nameof(Pyre)}{nameof(WallOfFlames)}".ToKebabCase());
            public static AbilityName PhantomMenace => new($"{nameof(Pyre)}{nameof(PhantomMenace)}".ToKebabCase());
        }

        public static class BigBadBull
        {
            public static AbilityName UnleashTheRage => new($"{nameof(BigBadBull)}{nameof(UnleashTheRage)}".ToKebabCase());
        }

        public static class Mummy
        {
            public static AbilityName SpawnRoach => new($"{nameof(Mummy)}{nameof(SpawnRoach)}".ToKebabCase());
            public static AbilityName SpawnRoachModified => new($"{nameof(Mummy)}{nameof(SpawnRoachModified)}".ToKebabCase());
            public static AbilityName LeapOfHunger => new($"{nameof(Mummy)}{nameof(LeapOfHunger)}".ToKebabCase());
        }

        public static class Roach
        {
            public static AbilityName DegradingCarapace => new($"{nameof(Roach)}{nameof(DegradingCarapace)}".ToKebabCase());
            public static AbilityName CorrosiveSpit => new($"{nameof(Roach)}{nameof(CorrosiveSpit)}".ToKebabCase());
        }

        public static class Parasite
        {
            public static AbilityName ParalysingGrasp => new($"{nameof(Parasite)}{nameof(ParalysingGrasp)}".ToKebabCase());
        }

        public static class Horrior
        {
            public static AbilityName ExpertFormation => new($"{nameof(Horrior)}{nameof(ExpertFormation)}".ToKebabCase());
            public static AbilityName Mount => new($"{nameof(Horrior)}{nameof(Mount)}".ToKebabCase());
        }

        public static class Marksman
        {
            public static AbilityName CriticalMark => new($"{nameof(Marksman)}{nameof(CriticalMark)}".ToKebabCase());
        }

        public static class Surfer
        {
            public static AbilityName Dismount => new($"{nameof(Surfer)}{nameof(Dismount)}".ToKebabCase());
        }

        public static class Mortar
        {
            public static AbilityName DeadlyAmmunition => new($"{nameof(Mortar)}{nameof(DeadlyAmmunition)}".ToKebabCase());
            public static AbilityName Reload => new($"{nameof(Mortar)}{nameof(Reload)}".ToKebabCase());
            public static AbilityName PiercingBlast => new($"{nameof(Mortar)}{nameof(PiercingBlast)}".ToKebabCase());
        }

        public static class Hawk
        {
            public static AbilityName TacticalGoggles => new($"{nameof(Hawk)}{nameof(TacticalGoggles)}".ToKebabCase());
            public static AbilityName Leadership => new($"{nameof(Hawk)}{nameof(Leadership)}".ToKebabCase());
            public static AbilityName HealthKit => new($"{nameof(Hawk)}{nameof(HealthKit)}".ToKebabCase());
        }

        public static class Engineer
        {
            public static AbilityName AssembleMachine => new($"{nameof(Engineer)}{nameof(AssembleMachine)}".ToKebabCase());
            public static AbilityName Operate => new($"{nameof(Engineer)}{nameof(Operate)}".ToKebabCase());
            public static AbilityName Repair => new($"{nameof(Engineer)}{nameof(Repair)}".ToKebabCase());
        }

        public static class Cannon
        {
            public static AbilityName Assembling => new($"{nameof(Cannon)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new($"{nameof(Cannon)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName HeatUp => new($"{nameof(Cannon)}{nameof(HeatUp)}".ToKebabCase());
        }

        public static class Ballista
        {
            public static AbilityName Assembling => new($"{nameof(Ballista)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new($"{nameof(Ballista)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName AddOn => new($"{nameof(Ballista)}{nameof(AddOn)}".ToKebabCase());
            public static AbilityName Aim => new($"{nameof(Ballista)}{nameof(Aim)}".ToKebabCase());
        }

        public static class Radar
        {
            public static AbilityName Assembling => new($"{nameof(Radar)}{nameof(Assembling)}".ToKebabCase());
            public static AbilityName Machine => new($"{nameof(Radar)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName PowerDependency => new($"{nameof(Radar)}{nameof(PowerDependency)}".ToKebabCase());
            public static AbilityName ResonatingSweep => new($"{nameof(Radar)}{nameof(ResonatingSweep)}".ToKebabCase());
            public static AbilityName RadioLocation => new($"{nameof(Radar)}{nameof(RadioLocation)}".ToKebabCase());
        }

        public static class Vessel
        {
            public static AbilityName Machine => new($"{nameof(Vessel)}{nameof(Machine)}".ToKebabCase());
            public static AbilityName AbsorbentField => new($"{nameof(Vessel)}{nameof(AbsorbentField)}".ToKebabCase());
            public static AbilityName Fortify => new($"{nameof(Vessel)}{nameof(Fortify)}".ToKebabCase());
        }
        
        public static class Omen
        {
            public static AbilityName Rendition => new($"{nameof(Omen)}{nameof(Rendition)}".ToKebabCase());
            public static AbilityName RenditionPlacement => new($"{nameof(Omen)}{nameof(RenditionPlacement)}".ToKebabCase());
        }
    }
}
