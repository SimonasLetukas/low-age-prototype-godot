using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Shared;

namespace low_age_data.Collections
{
    public static class Abilities
    {
        public static List<Ability> Get()
        {
            return new List<Ability>
            {
                new Passive(
                    AbilityName.Leader.AllForOne,
                    nameof(AbilityName.Leader.AllForOne).CamelCaseToWords(),
                    "Revelators faction loses if Leader dies.",
                    EffectName.Leader.AllForOneApplyBehaviour),

                new Passive(
                    AbilityName.Leader.MenacingPresence,
                    nameof(AbilityName.Leader.MenacingPresence).CamelCaseToWords(),
                    "All friendly and enemy units that enter 6 Attack Distance around Leader " +
                    "have their Melee Damage and Ranged Damage reduced by 2 (total minimum of 1).",
                    EffectName.Leader.MenacingPresenceSearch),

                new Target(
                    AbilityName.Leader.OneForAll,
                    TurnPhase.Planning,
                    nameof(AbilityName.Leader.OneForAll).CamelCaseToWords(),
                    "Select an adjacent Obelisk and sap its energy to give all friendly units " +
                    "+2 Health. This Obelisk cannot be sapped again for 10 turns.",
                    1,
                    EffectName.Leader.OneForAllApplyBehaviourObelisk),

                new Build(
                    AbilityName.Slave.Build,
                    nameof(AbilityName.Slave.Build).CamelCaseToWords(),
                    "Start building a Revelators' structure on an adjacent tile. Multiple Slaves " +
                    "can build the structure, each additional one after the first provides half of the " +
                    "Celestium production to the construction than the previous Slave.",
                    1,
                    new List<StructureName>
                    {
                        StructureName.Hut,
                        StructureName.Obelisk,
                        StructureName.Shack,
                        StructureName.Smith,
                        StructureName.Fletcher,
                        StructureName.Alchemy,
                        StructureName.Depot,
                        StructureName.Workshop,
                        StructureName.Outpost,
                        StructureName.Barricade
                    },
                    true,
                    true),

                new Target(
                    AbilityName.Slave.Repair,
                    TurnPhase.Planning,
                    nameof(AbilityName.Slave.Repair).CamelCaseToWords(),
                    "Select an adjacent structure. At the start of the next planning phase the " +
                    "selected structure receives +1 Health. Multiple Slaves can stack their repairs.",
                    1,
                    EffectName.Slave.RepairApplyBehaviourStructure),

                new Target(
                    AbilityName.Slave.ManualLabour,
                    TurnPhase.Planning,
                    nameof(AbilityName.Slave.ManualLabour).CamelCaseToWords(),
                    "Select an adjacent Hut. At the start of the next planning phase receive +2 " +
                    "Scraps. Maximum of one Slave per Hut.",
                    1,
                    EffectName.Slave.ManualLabourApplyBehaviourHut)
            };
        }
    }
}
