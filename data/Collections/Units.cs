using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Units
    {
        public static List<Unit> Get()
        {
            return new List<Unit>
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
                        new AttackStat(
                            1, 
                            false, 
                            Attacks.Melee, 
                            1, 
                            1)
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
                new Unit(
                    UnitName.Leader,
                    nameof(UnitName.Leader),
                    "",
                    new List<Stat>
                    {
                        new CombatStat(50, true, Stats.Health),
                        new CombatStat(1, false, Stats.MeleeArmour),
                        new CombatStat(5, false, Stats.RangedArmour),
                        new CombatStat(3, true, Stats.Movement),
                        new CombatStat(28, false, Stats.Initiative),
                        new AttackStat(
                            5, 
                            false, 
                            Attacks.Melee, 
                            1, 
                            2, 
                            CombatAttributes.Biological, 
                            7)
                    },
                    Factions.Revelators,
                    new List<CombatAttributes>
                    {
                        CombatAttributes.Armoured,
                        CombatAttributes.Celestial
                    },
                    new List<AbilityName>
                    {
                        AbilityName.Leader.AllForOne,
                        AbilityName.Leader.MenacingPresence,
                        AbilityName.Leader.OneForAll
                    }),
            };
        }
    }
}
