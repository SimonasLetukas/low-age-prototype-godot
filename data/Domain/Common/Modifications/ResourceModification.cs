﻿using low_age_data.Domain.Resources;

namespace low_age_data.Domain.Shared.Modifications
{
    public class ResourceModification : Modification
    {
        public ResourceModification(
            Change change, 
            float amount,
            ResourceId resource) : base($"{nameof(Modification)}.{nameof(ResourceModification)}", change, amount)
        {
            Resource = resource;
        }

        public ResourceId Resource { get; }
    }
}