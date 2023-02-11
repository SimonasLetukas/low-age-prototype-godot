using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Effects;
using low_age_data.Domain.Entities;

namespace low_age_data.Domain.Shared.Flags
{
    public class FilterFlag : ValueObject<FilterFlag>
    {
        public override string ToString()
        {
            return $"{nameof(FilterFlag)}.{Value}";
        }

        /// <summary>
        /// <see cref="Entity"/> is first in the <see cref="Effect"/> chain
        /// </summary>
        public static FilterFlag Origin => new(FilterFlags.Origin);

        /// <summary>
        /// <see cref="Entity"/> is previous in the <see cref="Effect"/> chain.
        /// </summary>
        public static FilterFlag Source => new(FilterFlags.Source);

        /// <summary>
        /// <see cref="Entity"/> is itself. <see cref="Self"/> is never included to any of the other
        /// <see cref="Shared.Filters"/>, so it has to be explicitly added as one.
        /// </summary>
        public static FilterFlag Self => new(FilterFlags.Self); // TODO go through all abilities to see where self could be applied 

        /// <summary>
        /// <see cref="Entity"/> is owned by the same player.
        /// </summary>
        public static FilterFlag Player => new(FilterFlags.Player);

        /// <summary>
        /// <see cref="Entity"/> is on the same team, but a different player. 
        /// </summary>
        public static FilterFlag Ally => new(FilterFlags.Ally);

        /// <summary>
        /// <see cref="Entity"/> is on an enemy team.
        /// </summary>
        public static FilterFlag Enemy => new(FilterFlags.Enemy);

        /// <summary>
        /// <see cref="Entity"/> is a <see cref="Unit"/>.
        /// </summary>
        public static FilterFlag Unit => new(FilterFlags.Unit);

        /// <summary>
        /// <see cref="Entity"/> is a <see cref="Structure"/>.
        /// </summary>
        public static FilterFlag Structure => new(FilterFlags.Structure);

        private FilterFlag(FilterFlags @enum)
        {
            Value = @enum;
        }

        private FilterFlags Value { get; }

        private enum FilterFlags
        {
            Origin,
            Source,
            Self,
            Player,
            Ally,
            Enemy,
            Unit,
            Structure
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}