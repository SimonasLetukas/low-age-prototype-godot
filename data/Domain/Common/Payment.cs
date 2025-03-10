﻿using LowAgeData.Domain.Resources;

namespace LowAgeData.Domain.Common
{
    public class Payment
    {
        public Payment(ResourceId resource, int amount = 0)
        {
            Resource = resource;
            Amount = amount;
        }

        public ResourceId Resource { get; }
        public int Amount { get; }
    }
}
