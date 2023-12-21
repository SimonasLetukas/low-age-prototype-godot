using System;
using System.Collections.Generic;
using System.Linq;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Common;
using low_age_data.Domain.Common.Filters;
using low_age_data.Domain.Common.Modifications;
using low_age_data.Domain.Common.Shape;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities.Actors.Structures;
using low_age_data.Domain.Entities.Actors.Units;
using low_age_data.Domain.Entities.Doodads;
using low_age_data.Domain.Logic;
using Newtonsoft.Json.Serialization;

namespace low_age_data.Shared
{
    public class KnownTypesBinder : ISerializationBinder
    {
        private IEnumerable<Type> KnownTypes { get; } = new List<Type>
        {
            // Abilities:
            typeof(Build), typeof(Instant), typeof(Passive), typeof(Produce), typeof(Research), typeof(Target), 
            typeof(Toggle), 
            
            // Behaviours:
            typeof(Ammunition), typeof(Ascendable), typeof(Buff), typeof(Buildable), typeof(Counter), 
            typeof(ExtraAttack), typeof(HighGround), typeof(Income), typeof(InterceptDamage), typeof(MaskProvider), 
            typeof(MovementBlock), typeof(Tether), typeof(Wait),
            
            // Filters:
            typeof(FilterGroup), typeof(SpecificCombatAttribute), typeof(SpecificEntity), typeof(SpecificFaction), 
            typeof(SpecificFlag), 
            
            // Modifications:
            typeof(AttackModification), typeof(DurationModification), typeof(ResourceModification), 
            typeof(SizeModification), typeof(StatCopyModification), typeof(StatModification), 
            
            // Shapes:
            typeof(Circle), typeof(Custom), typeof(Line), typeof(Map), 

            // Stats:
            typeof(AttackStat), typeof(CombatStat),

            // Effects:
            typeof(ApplyBehaviour), typeof(CreateEntity), typeof(Damage), typeof(Destroy), typeof(ExecuteAbility), 
            typeof(Force), typeof(ModifyAbility), typeof(ModifyCounter), typeof(ModifyPlayer), typeof(ModifyResearch), 
            typeof(Reload), typeof(RemoveBehaviour), typeof(Search), typeof(Teleport), 

            // Entities:
            typeof(Structure), typeof(Unit), typeof(Doodad), 
            
            // Logic:
            typeof(BehaviourCondition), typeof(EntityCondition), typeof(MaskCondition), typeof(ResultValidator), 
            typeof(TileCondition), 
        };

        public Type BindToType(string? assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName)
                   ?? throw new UnknownTypeException($"Type '{typeName}' was not found in the collection of " +
                                                     "known blueprint types.");
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
    
    [Serializable]
    public class UnknownTypeException : Exception
    {
        public UnknownTypeException()
        {
        }

        public UnknownTypeException(string message)
            : base(message)
        {
        }

        public UnknownTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
