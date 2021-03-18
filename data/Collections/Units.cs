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
        }
    }
}
