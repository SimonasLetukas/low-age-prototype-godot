using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Conditions;
using low_age_data.Domain.Shared.Durations;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;

namespace low_age_data.Collections
{
    public static class Behaviours
    {
        public static List<Behaviour> Get()
        {
            return new List<Behaviour>
            {
                new Buff(
                    BehaviourName.Leader.AllForOneBuff,
                    nameof(BehaviourName.Leader.AllForOneBuff).CamelCaseToWords(),
                    "Revelators faction loses when this unit dies.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Leader.AllForOnePlayerLoses
                    }),

                new Buff(
                    BehaviourName.Leader.MenacingPresenceBuff,
                    nameof(BehaviourName.Leader.MenacingPresenceBuff).CamelCaseToWords(),
                    "Melee and Range Damage for this unit is reduced by 2.",
                    null,
                    new List<Modification>
                    {
                        new AttackModification(
                            Change.RemoveMax, 
                            2f,
                            Attacks.Melee, 
                            AttackAttribute.MaxAmount),
                        new AttackModification(
                            Change.RemoveMax, 
                            2f,
                            Attacks.Ranged,
                            AttackAttribute.MaxAmount)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Buff(
                    BehaviourName.Leader.OneForAllObeliskBuff,
                    nameof(BehaviourName.Leader.OneForAllObeliskBuff).CamelCaseToWords(),
                    "This unit has recently been sapped for health.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Leader.OneForAllSearch
                    },
                    null,
                    null,
                    EndsAt.StartOf.Tenth.Planning,
                    false,
                    Alignment.Negative),

                new Buff(
                    BehaviourName.Leader.OneForAllHealBuff,
                    nameof(BehaviourName.Leader.OneForAllHealBuff).CamelCaseToWords(),
                    "Heals for 2 Health.",
                    null,
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent, 
                            2f, 
                            Stats.Health)
                    },
                    null,
                    null,
                    null,
                    EndsAt.Instant,
                    false,
                    Alignment.Positive),

                new Buff(
                    BehaviourName.Slave.RepairStructureBuff,
                    nameof(BehaviourName.Slave.RepairStructureBuff).CamelCaseToWords(),
                    "This structure will be repaired by +1 Health at the start of the planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.RepairApplyBehaviourSelf
                    },
                    new List<Modification>
                    {
                        new StatModification(
                            Change.AddCurrent, 
                            1f,
                            Stats.Health)
                    },
                    null,
                    EndsAt.StartOf.Next.Planning,
                    true,
                    Alignment.Positive,
                    new List<Condition>
                    {
                        Condition.Effect.Chain.OriginIsDead
                    },
                    true),

                new Wait(
                    BehaviourName.Slave.RepairWait,
                    nameof(BehaviourName.Slave.RepairWait).CamelCaseToWords(),
                    "Currently repairing a structure.",
                    EndsAt.StartOf.Next.Planning),

                new Buff(
                    BehaviourName.Slave.ManualLabourBuff,
                    nameof(BehaviourName.Slave.ManualLabourBuff).CamelCaseToWords(),
                    "Slave is working on this Hut to provide +2 Scraps at the start of the planning phase.",
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.ManualLabourApplyBehaviourSelf
                    },
                    null,
                    new List<EffectName>
                    {
                        EffectName.Slave.ManualLabourModifyPlayer
                    },
                    EndsAt.StartOf.Next.Planning,
                    false,
                    Alignment.Positive,
                    new List<Condition>
                    {
                        Condition.Effect.Chain.OriginIsDead
                    },
                    true),

                new Wait(
                    BehaviourName.Slave.ManualLabourWait,
                    nameof(BehaviourName.Slave.ManualLabourWait).CamelCaseToWords(),
                    "Currently working on a nearby Hut.",
                    EndsAt.StartOf.Next.Planning),

                new ExtraAttack(
                    BehaviourName.Quickdraw.DoubleshotExtraAttack,
                    nameof(BehaviourName.Quickdraw.DoubleshotExtraAttack).CamelCaseToWords(),
                    "Ranged attacks twice.",
                    new List<Attacks>
                    {
                        Attacks.Ranged
                    }),

                new Buff(
                    BehaviourName.Quickdraw.CrippleBuff,
                    nameof(BehaviourName.Quickdraw.CrippleBuff).CamelCaseToWords(),
                    "This unit has only 60% of their maximum Movement (rounded up) and cannot receive healing " +
                    "from any sources until the end of its action.",
                    new List<Flag>
                    {
                        Flag.Modification.CannotBeHealed
                    },
                    new List<Modification>
                    {
                        new StatModification(
                            Change.MultiplyMax,
                            0.6f,
                            Stats.Movement)
                    },
                    null,
                    null,
                    null,
                    EndsAt.EndOf.This.Action,
                    false,
                    Alignment.Negative,
                    null,
                    false,
                    null,
                    true),

                new Buff(
                    BehaviourName.Gorger.FanaticSuicideBuff,
                    nameof(BehaviourName.Gorger.FanaticSuicideBuff).CamelCaseToWords(),
                    "Upon getting killed or executing a melee attack Gorger explodes dealing its Melee Damage " +
                    "to all friendly and enemy units in 1 Distance.",
                    null,
                    null,
                    null,
                    null,
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideSearch
                    }),

                new Destroy(
                    BehaviourName.Gorger.FanaticSuicideDestroy,
                    nameof(BehaviourName.Gorger.FanaticSuicideDestroy).CamelCaseToWords(),
                    "")
            };
        }
    }
}
