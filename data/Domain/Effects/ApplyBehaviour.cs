using low_age_data.Domain.Behaviours;
using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class ApplyBehaviour : Effect
    {
        public ApplyBehaviour(
            EffectId id,
            IList<BehaviourId> behavioursToApply,
            Location? target = null,
            IList<IFilterItem>? filters = null,
            Location? behaviourOwner = null,
            bool? waitForInitialEffects = null,
            IList<Validator>? validators = null)
            : base(
                id, 
                validators ?? new List<Validator>())
        {
            BehavioursToApply = behavioursToApply;
            Target = target ?? Location.Inherited;
            Filters = filters ?? new List<IFilterItem>();
            BehaviourOwner = behaviourOwner;
            WaitForInitialEffects = waitForInitialEffects ?? false;
        }

        public IList<BehaviourId> BehavioursToApply { get; }
        public Location Target { get; }
        public IList<IFilterItem> Filters { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner.
        /// </summary>
        public Location? BehaviourOwner { get; }
        
        /// <summary>
        /// When <see cref="ApplyBehaviour"/> is called from <see cref="Ability"/>, this flag makes sure that
        /// <see cref="Behaviour"/> <see cref="Effect"/>s are executed until the <see cref="Ability"/> is considered
        /// executed. Otherwise the <see cref="Ability"/> is cancelled and action is not consumed, if applicable.
        /// </summary>
        public bool WaitForInitialEffects { get; }
    }
}
