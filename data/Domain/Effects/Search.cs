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
            bool? ignoreCenter = null,
            IList<Validator>? validators = null,
            bool? usedForValidator = null) : base(name, $"{nameof(Effect)}.{nameof(Search)}", validators ?? new List<Validator>())
        {
            Radius = radius;
            SearchFlags = searchFlags;
            FilterFlags = filterFlags;
            Effects = effects ?? new List<EffectName>();
            Location = location ?? Location.Inherited;
            Shape = shape ?? Shape.Circle;
            IgnoreCenter = ignoreCenter ?? false;
            UsedForValidator = usedForValidator ?? false;
        }

        public int Radius { get; }
        public IList<Flag> SearchFlags { get; }
        public IList<Flag> FilterFlags { get; }
        public IList<EffectName> Effects { get; } // Effects executed on each entity found
        public Location Location { get; }
        public Shape Shape { get; }
        public bool IgnoreCenter { get; }
        public bool UsedForValidator { get; }
    }
}
