using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Effects
{
    public class CreateEntity : Effect
    {
        public CreateEntity(
            EffectId id,
            EntityId entityToCreate,
            IList<BehaviourId>? initialEntityBehaviours = null,
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) : base(id, $"{nameof(Effect)}.{nameof(CreateEntity)}", validators ?? new List<Validator>())
        {
            EntityToCreate = entityToCreate;
            InitialEntityBehaviours = initialEntityBehaviours ?? new List<BehaviourId>();
            BehaviourOwner = behaviourOwner;
        }

        public EntityId EntityToCreate { get; }
        public IList<BehaviourId> InitialEntityBehaviours { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}
