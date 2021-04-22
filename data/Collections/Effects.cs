using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using low_age_data.Domain.Shared.Modifications;
using System.Collections.Generic;
using low_age_data.Domain.Entities.Features;

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
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            Condition.BehaviourToApplyDoesNotAlreadyExist
                        })
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
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Slave.RepairApplyBehaviourStructure,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairStructureBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure
                    },
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            Condition.TargetDoesNotHaveFullHealth
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Slave.RepairApplyBehaviourSelf,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.RepairWait
                    },
                    Location.Origin),

                new ApplyBehaviour(
                    EffectName.Slave.ManualLabourApplyBehaviourHut,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Structure,
                        Flag.Structure.Hut
                    },
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            Condition.BehaviourToApplyDoesNotAlreadyExist
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Slave.ManualLabourApplyBehaviourSelf,
                    new List<BehaviourName>
                    {
                        BehaviourName.Slave.ManualLabourWait
                    },
                    Location.Origin),

                new ModifyPlayer(
                    EffectName.Slave.ManualLabourModifyPlayer,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    },
                    null,
                    new List<ResourceModification>
                    {
                        new ResourceModification(
                            Change.AddCurrent,
                            2.0f,
                            Resources.Scraps)
                    }),

                new ApplyBehaviour(
                    EffectName.Quickdraw.DoubleshotApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.DoubleshotExtraAttack
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new ApplyBehaviour(
                    EffectName.Quickdraw.CrippleApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Quickdraw.CrippleBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    }),

                new ApplyBehaviour(
                    EffectName.Gorger.FanaticSuicideApplyBehaviourBuff,
                    new List<BehaviourName>
                    {
                        BehaviourName.Gorger.FanaticSuicideBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new Destroy(
                    EffectName.Gorger.FanaticSuicideDestroy,
                    Location.Origin),

                new Search(
                    EffectName.Gorger.FanaticSuicideSearch,
                    1,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Structure,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Gorger.FanaticSuicideDamage
                    },
                    Location.Self),

                new Damage(
                    EffectName.Gorger.FanaticSuicideDamage,
                    DamageType.OverrideMelee),

                new Damage(
                    EffectName.Camou.SilentAssassinOnHitDamage,
                    DamageType.CurrentMelee,
                    new Amount(
                        0,
                        0.5f,
                        Flag.Amount.FromMissingHealth,
                        new List<Flag>
                        {
                            Flag.Filter.Enemy,
                            Flag.Filter.Unit
                        }),
                    null,
                    null,
                    new List<Validator>
                    {
                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchFriendly,
                            new List<Condition>
                            {
                                Condition.NoActorsFoundFromEffect
                            })
                    }),

                new ApplyBehaviour(
                    EffectName.Camou.SilentAssassinOnHitSilence,
                    new List<BehaviourName>
                    {
                        BehaviourName.Camou.SilentAssassinBuff
                    },
                    Location.Actor,
                    new List<Flag>
                    {
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<Validator>
                    {
                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchFriendly,
                            new List<Condition>
                            {
                                Condition.NoActorsFoundFromEffect
                            }),

                        new ResultValidator(
                            EffectName.Camou.SilentAssassinSearchEnemy,
                            new List<Condition>
                            {
                                Condition.NoActorsFoundFromEffect
                            })
                    }),

                new Search(
                    EffectName.Camou.SilentAssassinSearchFriendly,
                    4,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Ally
                    },
                    null,
                    Location.Origin,
                    Shape.Circle,
                    null,
                    true),

                new Search(
                    EffectName.Camou.SilentAssassinSearchEnemy,
                    4,
                    new List<Flag>(),
                    new List<Flag>
                    {
                        Flag.Filter.Unit,
                        Flag.Filter.Enemy
                    },
                    null,
                    Location.Actor,
                    Shape.Circle,
                    null,
                    true),

                new Teleport(
                    EffectName.Camou.ClimbTeleport,
                    BehaviourName.Camou.ClimbWait,
                    new List<Validator>
                    {
                        new Validator(new List<Condition>
                        {
                            Condition.TargetIsHighGround,
                            Condition.TargetIsUnoccupied
                        })
                    }),

                new ApplyBehaviour(
                    EffectName.Camou.ClimbApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Camou.ClimbBuff
                    },
                    Location.Self,
                    new List<Flag>
                    {
                        Flag.Filter.Self
                    }),

                new CreateEntity(
                    EffectName.Shaman.WondrousGooCreateEntity,
                    FeatureName.WondrousGoo,
                    new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooFeatureWait
                    }),

                new Search(
                    EffectName.Shaman.WondrousGooSearch,
                    0,
                    new List<Flag>
                    {
                        Flag.Effect.Search.AppliedOnAction,
                        Flag.Effect.Search.AppliedOnEnter
                    },
                    new List<Flag>
                    {
                        Flag.Filter.Ally,
                        Flag.Filter.Enemy,
                        Flag.Filter.Unit
                    },
                    new List<EffectName>
                    {
                        EffectName.Shaman.WondrousGooApplyBehaviour
                    },
                    Location.Self),

                new ApplyBehaviour(
                    EffectName.Shaman.WondrousGooApplyBehaviour,
                    new List<BehaviourName>
                    {
                        BehaviourName.Shaman.WondrousGooBuff
                    }),

                new Destroy(
                    EffectName.Shaman.WondrousGooDestroy),

                new Damage(
                    EffectName.Shaman.WondrousGooDamage,
                    DamageType.Pure,
                    new Amount(1))
            };
        }
    }
}
