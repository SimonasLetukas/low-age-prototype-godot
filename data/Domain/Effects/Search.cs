using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;

namespace low_age_data.Domain.Effects
{
    public class Search : Effect
    {
        public Search(
            EffectName name,
            int radius,
            IList<Flag> searchFlags,
            IList<Flag> filterFlags,
            IList<EffectName>? effects = null,
            Location? location = null,
            Shape? shape = null,
            int? ignoreRadius = null,
            IList<Validator>? validators = null,
            bool? usedForValidator = null) : base(name, $"{nameof(Effect)}.{nameof(Search)}", validators ?? new List<Validator>())
        {
            Radius = radius;
            SearchFlags = searchFlags;
            FilterFlags = filterFlags;
            Effects = effects ?? new List<EffectName>();
            Location = location ?? Location.Inherited;
            Shape = shape ?? Shape.Circle;
            IgnoreRadius = ignoreRadius ?? -1;
            UsedForValidator = usedForValidator ?? false;
        }

        public int Radius { get; }
        public IList<Flag> SearchFlags { get; }
        public IList<Flag> FilterFlags { get; }
        
        /// <summary>
        /// Effects executed on each entity found
        /// </summary>
        public IList<EffectName> Effects { get; } 
        
        /// <summary>
        /// Indicates where the search originates from
        /// </summary>
        public Location Location { get; }
        public Shape Shape { get; }
        
        /// <summary>
        /// -1 to not ignore anything (default value). 0 to ignore center. 1 to ignore all adjacent entities (if the
        /// <see cref="Shape"/> is <see cref="Shared.Shape.Circle"/> or appropriate).
        /// </summary>
        public int IgnoreRadius { get; }
        
        public bool UsedForValidator { get; }
    }
}
