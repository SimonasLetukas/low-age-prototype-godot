using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class RemoveBehaviour : Effect
    {
        public RemoveBehaviour(
            EffectId id,
            IList<BehaviourId> behavioursToRemove,
            Location? location = null,
            IList<IFilterItem>? filters = null,
            bool? treatAsDeath = null,
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) 
            : base(
                id, 
                validators ?? new List<Validator>())
        {
            BehavioursToRemove = behavioursToRemove;
            Location = location ?? Location.Inherited;
            Filters = filters ?? new List<IFilterItem>();
            TreatAsDeath = treatAsDeath ?? false;
            BehaviourOwner = behaviourOwner;
        }

        public IList<BehaviourId> BehavioursToRemove { get; }
        public Location Location { get; }
        public IList<IFilterItem> Filters { get; }
        
        /// <summary>
        /// If true, each <see cref="BehavioursToRemove"/> trigger their on-death functionalities (e.g.
        /// <see cref="Buff"/> would execute <see cref="Buff.FinalEffects"/>). False by default.
        /// </summary>
        public bool TreatAsDeath { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner and only
        /// remove the owner's behaviour.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}