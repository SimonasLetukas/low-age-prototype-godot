using System.Collections.Generic;
using LowAgeData.Domain.Behaviours;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities.Actors;
using low_age_prototype_common;

namespace LowAgeData.Domain.Resources
{
    public class Resource : IDisplayable
    {
        public Resource(
            ResourceId id, 
            string displayName, 
            string description, 
            string sprite,
            bool hasLimit, 
            bool isConsumable, 
            bool hasBank, ResourceId? storedAs = null,
            bool? attachesToNewActors = null,
            IList<EffectId>? positiveIncomeEffects = null,
            IList<EffectId>? negativeIncomeEffects = null,
            string? negativeIncomeDescription = null,
            bool? effectAmountMultipliedByResourceAmount = null)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Sprite = sprite;
            CenterOffset = new Vector2<int>(0, 0);
            HasLimit = hasLimit;
            IsConsumable = isConsumable;
            HasBank = hasBank;
            StoredAs = storedAs ?? id;
            AttachesToNewActors = attachesToNewActors ?? false;
            PositiveIncomeEffects = positiveIncomeEffects ?? new List<EffectId>();
            NegativeIncomeEffects = negativeIncomeEffects ?? new List<EffectId>();
            NegativeIncomeDescription = negativeIncomeDescription ?? string.Empty;
            EffectAmountMultipliedByResourceAmount = effectAmountMultipliedByResourceAmount ?? false;
        }

        public ResourceId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }

        /// <summary>
        /// If true, max and current values are separate and going over the max value normally is not allowed.
        /// </summary>
        public bool HasLimit { get; }
        
        /// <summary>
        /// If true, spending this <see cref="Resource"/> as part of <see cref="Payment"/> deducts the value from the
        /// current amount. Otherwise, the payment is continuous (used for production).
        /// </summary>
        public bool IsConsumable { get; }
        
        /// <summary>
        /// If true, adding current or max amount accumulates into a bank of resources. Otherwise, only the incomes
        /// are added together to always represent the current value. 
        /// </summary>
        public bool HasBank { get; }
        
        /// <summary>
        /// Specify the <see cref="Resource"/> used for storing purposes, using and making its max amount shared
        /// between other <see cref="Resource"/>s with the same <see cref="StoredAs"/> value. This can be
        /// done to better control the <see cref="Income"/> of the stored resource and to group certain resources to
        /// be stored together. <see cref="StoredAs"/> is identical to the <see cref="Id"/> by default.
        /// </summary>
        public ResourceId StoredAs { get; }
        
        /// <summary>
        /// If true, spending this resource as part of <see cref="Payment"/> during <see cref="Selection{T}"/> retains the
        /// value spent for as long as the <see cref="Actor"/> that was spawned is not destroyed (technically, each
        /// such <see cref="Actor"/> gets a negative <see cref="Income"/> for as long as it's not destroyed, so it
        /// mostly makes sense to use this property together with <see cref="HasBank"/> set to false, unless funky
        /// behaviour is required). False by default.
        /// </summary>
        public bool AttachesToNewActors { get; }
        
        /// <summary>
        /// List of <see cref="Effect"/>s to be executed at the start of each action phase if the current value of this
        /// <see cref="Resource"/> is positive (zero excluded).
        /// </summary>
        public IList<EffectId> PositiveIncomeEffects { get; }

        /// <summary>
        /// List of <see cref="Effect"/>s to be executed at the start of each action phase if the current value of this
        /// <see cref="Resource"/> is negative (zero excluded).
        /// </summary>
        public IList<EffectId> NegativeIncomeEffects { get; }
        
        /// <summary>
        /// Additional text added to the <see cref="Description"/> when the income for this <see cref="Resource"/> is
        /// negative (zero excluded).
        /// </summary>
        public string NegativeIncomeDescription { get; }
        
        /// <summary>
        /// If true, each time <see cref="PositiveIncomeEffects"/> or <see cref="NegativeIncomeEffects"/> are
        /// triggered, the amount of effects executed is multiplied by the current amount of resources (positive
        /// or negative, zero excluded). False by default.
        /// </summary>
        public bool EffectAmountMultipliedByResourceAmount { get; }

        public string? Sprite { get; }
        
        public Vector2<int> CenterOffset { get; }
    }
}