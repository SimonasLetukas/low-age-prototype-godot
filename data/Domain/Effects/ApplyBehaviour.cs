using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
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
                target ?? Location.Inherited,
                validators ?? new List<Validator>())
        {
            BehavioursToApply = behavioursToApply;
            Filters = filters ?? new List<IFilterItem>();
            BehaviourOwner = behaviourOwner;
            WaitForInitialEffects = waitForInitialEffects ?? false;
        }

        public IList<BehaviourId> BehavioursToApply { get; }
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
