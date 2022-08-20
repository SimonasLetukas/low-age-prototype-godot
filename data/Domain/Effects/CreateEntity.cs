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
            EffectName name,
            EntityName entityToCreate,
            IList<BehaviourName>? initialEntityBehaviours = null,
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(CreateEntity)}", validators ?? new List<Validator>())
        {
            EntityToCreate = entityToCreate;
            InitialEntityBehaviours = initialEntityBehaviours ?? new List<BehaviourName>();
            BehaviourOwner = behaviourOwner;
        }

        public EntityName EntityToCreate { get; }
        public IList<BehaviourName> InitialEntityBehaviours { get; }
        
        /// <summary>
        /// If <see cref="Behaviour.OwnerAllowed"/> is true, this property can be used to specify the owner.
        /// </summary>
        public Location? BehaviourOwner { get; }
    }
}
