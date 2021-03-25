using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Conditions;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Collections
{
    public static class Effects
    {
        public static List<Effect> Get()
        {
            return new List<Effect>
            {
                new ApplyBehaviour(
                    EffectName.Leader.AllForOneApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.AllForOneBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ModifyPlayer(
                    EffectName.Leader.AllForOnePlayerLoses,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    new List<Flag>
                    {
                        Flag.Effect.ModifyPlayer.GameLost
                    }),

                new Search(
                    EffectName.Leader.MenacingPresenceSearch,
                    6,
                    new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnEnter,
                        Flag.Effect.Search.AppliedOnAction,
                        Flag.Effect.Search.RemovedOnExit
                    },
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }, 
                    new List<EffectName>
                    {
                        EffectName.Leader.MenacingPresenceApplyBehaviour
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Leader.MenacingPresenceApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.MenacingPresenceBuff
                    }),

                new ApplyBehaviour(
                    EffectName.Leader.OneForAllApplyBehaviourObelisk,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllObeliskBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Structure.Obelisk
                    },
                    new List<Condition>
                    {
                        Condition.Behaviour.BehaviourToApplyDoesNotAlreadyExist
                    }),

                new Search(
                    EffectName.Leader.OneForAllSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Leader.OneForAllApplyBehaviourHeal
                    },
                    Location.Self,
                    Shape.Map),

                new ApplyBehaviour(
                    EffectName.Leader.OneForAllApplyBehaviourHeal,
                    new List<BehaviourName>
                    {
                        BehaviourName.Leader.OneForAllHealBuff
                    },
                    Location.Self)
            };
        }
    }
}
