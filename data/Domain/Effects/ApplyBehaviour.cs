using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Shared;
using System.Collections.Generic;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared.Filters;

namespace low_age_data.Domain.Effects
{
    public class ApplyBehaviour : Effect
    {
        public ApplyBehaviour(
            EffectName name,
            IList<BehaviourName> behavioursToApply,
            Location? target = null,
            IList<IFilterItem>? filters = null,
            Location? behaviourOwner = null,
            bool? waitForInitialEffects = null,
            IList<Validator>? validators = null)
            : base(
                name, 
                $"{nameof(Effect)}.{nameof(ApplyBehaviour)}", 
                validators ?? new List<Validator>())
        {
            BehavioursToApply = behavioursToApply;
            Target = target ?? Location.Inherited;
            Filters = filters ?? new List<IFilterItem>();
            BehaviourOwner = behaviourOwner;
            WaitForInitialEffects = waitForInitialEffects ?? false;
        }

        public IList<BehaviourName> BehavioursToApply { get; }
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
