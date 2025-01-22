﻿using System.Collections.Generic;
using LowAgeData.Domain.Common.Flags;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Modifications
{
    public class AttackModification : Modification
    {
        [JsonConstructor]
        public AttackModification(
            Change change, 
            float amount,
            Attacks attackType,
            AttackAttribute attribute,
            IList<ModificationFlag>? modificationFlags = null) : base(change, amount)
        {
            AttackType = attackType;
            Attribute = attribute;
            ModificationFlags = modificationFlags ?? new List<ModificationFlag>();
        }
        
        public AttackModification(
            Attacks attackType,
            IList<ModificationFlag> modificationFlags) : base(Change.AddCurrent, 0)
        {
            AttackType = attackType;
            Attribute = AttackAttribute.MaxAmount;
            ModificationFlags = modificationFlags;
        }

        public Attacks AttackType { get; }
        public AttackAttribute Attribute { get; }
        public IList<ModificationFlag> ModificationFlags { get; }
    }
}
