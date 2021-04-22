using System.Collections.Generic;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Effects
{
    public class CreateEntity : Effect
    {
        public CreateEntity(
            EffectName name,
            EntityName entityToCreate,
            IList<BehaviourName>? initialEntityBehaviours = null,
            IList<Validator>? validators = null) : base(name, $"{nameof(Effect)}.{nameof(CreateEntity)}", validators ?? new List<Validator>())
        {
            EntityToCreate = entityToCreate;
            InitialEntityBehaviours = initialEntityBehaviours ?? new List<BehaviourName>();
        }

        public EntityName EntityToCreate { get; }
        public IList<BehaviourName> InitialEntityBehaviours { get; }
    }
}
