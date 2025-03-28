﻿using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Logic;

namespace LowAgeData.Domain.Effects
{
    public class CreateEntity : Effect
    {
        public CreateEntity(
            EffectId id,
            EntityId entityToCreate,
            IList<BehaviourId>? initialEntityBehaviours = null,
            Location? behaviourOwner = null,
            IList<Validator>? validators = null) : base(id, validators ?? new List<Validator>())
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
