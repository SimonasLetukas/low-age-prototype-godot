using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Durations;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors;
using LowAgeData.Domain.Logic;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Behaviours
{
    public class Behaviour : IDisplayable
    {
        protected Behaviour(
            BehaviourId id, 
            string displayName, 
            string description,
            string sprite,
            EndsAt endsAt, 
            Alignment alignment, 
            bool? canStack = null,
            bool? canResetDuration = null,
            IList<Trigger>? triggers = null,
            bool? removeOnConditionsMet = null,
            IList<EffectId>? conditionalEffects = null,
            bool? ownerAllowed = null, 
            bool? hasSameInstanceForAllOwners = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Sprite = sprite;
            CenterOffset = new Vector2<int>(0, 0);
            EndsAt = endsAt;
            Alignment = alignment;
            CanStack = canStack ?? false;
            CanResetDuration = canResetDuration ?? false;
            Triggers = triggers ?? new List<Trigger>();
            RemoveOnConditionsMet = removeOnConditionsMet ?? false;
            ConditionalEffects = conditionalEffects ?? new List<EffectId>();
            OwnerAllowed = ownerAllowed ?? false;
            HasSameInstanceForAllOwners = hasSameInstanceForAllOwners ?? false;
        }

        [JsonProperty(Order = -2)]
        public BehaviourId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        
        /// <summary>
        /// <see cref="Behaviour"/>'s duration.
        /// </summary>
        public EndsAt EndsAt { get; }
        
        /// <summary>
        /// Specifies whether the <see cref="Behaviour"/> icon should be green (positive), red (negative) or gray
        /// (neutral).
        /// </summary>
        public Alignment Alignment { get; }
        
        /// <summary>
        /// If true, multiple <see cref="Behaviour"/>s can be added. False by default.
        /// </summary>
        public bool CanStack { get; }
        
        /// <summary>
        /// If true, applying the same <see cref="Behaviour"/> will reset the duration (to all stacks, if
        /// <see cref="CanStack"/> is true). False by default.
        /// </summary>
        public bool CanResetDuration { get; }

        /// <summary>
        /// Logical <b>OR</b> between the <see cref="Triggers"/>, but <b>AND</b> between the <see cref="Event"/>s
        /// inside.
        /// </summary>
        public IList<Trigger> Triggers { get; }
        
        /// <summary>
        /// If true, behaviour is removed (without triggering <see cref="Buff.FinalModifications"/> or
        /// <see cref="Buff.FinalEffects"/>, if they exist) when all <see cref="Event"/>s and their conditions are met
        /// in any of the <see cref="Triggers"/>. False by default.
        /// </summary>
        public bool RemoveOnConditionsMet { get; }
        
        /// <summary>
        /// Executed when any of the <see cref="Triggers"/> condition is met (before removal of this behaviour, if
        /// <see cref="RemoveOnConditionsMet"/> is true).
        /// </summary>
        public IList<EffectId> ConditionalEffects { get; }

        /// <summary>
        /// If true, <see cref="Behaviour"/> can be owned by <see cref="Actor"/> instance which by default allows to
        /// have multiple behaviour instances (all tracked separately) of the same type on the same entity for each
        /// owner instance. This can also be useful for validation. 
        /// </summary>
        public bool OwnerAllowed { get; }
        
        /// <summary>
        /// If true, <see cref="Behaviour"/>s added and owned by different <see cref="Actor"/>s are overwritten and 
        /// reset (in the context of <see cref="Buff.CanStack"/> and <see cref="Buff.CanResetDuration"/>) each time.
        /// This means that only one instance of behaviour can co-exist among any number of owners. 
        /// </summary>
        public bool HasSameInstanceForAllOwners { get; }

        public string? Sprite { get; }
        
        public Vector2<int> CenterOffset { get; }
    }
}
