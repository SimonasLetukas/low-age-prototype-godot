using System.Collections.Generic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;

namespace low_age_data.Domain.Effects
{
    public class Search : Effect
    {
        public Search(
            EffectName name,
            int distance,
            IList<Flag> searchFlags,
            IList<Flag> filterFlags,
            IList<EffectName> effects,
            Location? location = null,
            Shape? shape = null) : base(name, $"{nameof(Effect)}.{nameof(Search)}")
        {
            Distance = distance;
            SearchFlags = searchFlags;
            FilterFlags = filterFlags;
            Effects = effects;
            Location = location ?? Location.Inherited;
            Shape = shape ?? Shape.Circle;
        }

        public int Distance { get; }
        public IList<Flag> SearchFlags { get; }
        public IList<Flag> FilterFlags { get; }
        public IList<EffectName> Effects { get; }
        public Location Location { get; }
        public Shape Shape { get; }
    }
}
