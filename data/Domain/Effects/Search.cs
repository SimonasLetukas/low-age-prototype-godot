using low_age_data.Domain.Logic;
using low_age_data.Domain.Shared;
using low_age_data.Domain.Shared.Flags;
using System.Collections.Generic;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Shared.Shape;

namespace low_age_data.Domain.Effects
{
    public class Search : Effect
    {
        public Search(
            EffectName name,
            Shape shape,
            IList<SearchFlag> searchFlags,
            IList<Flag> filterFlags,
            IList<EffectName>? effects = null,
            Location? location = null,
            IList<Validator>? validators = null,
            bool? usedForValidator = null) : base(name, $"{nameof(Effect)}.{nameof(Search)}", validators ?? new List<Validator>())
        {
            Shape = shape;
            SearchFlags = searchFlags;
            FilterFlags = filterFlags;
            Effects = effects ?? new List<EffectName>();
            Location = location ?? Location.Inherited;
            UsedForValidator = usedForValidator ?? false;
        }
        
        /// <summary>
        /// Type of the <see cref="Shape"/> and its area which should be <see cref="Search"/>ed.
        /// </summary>
        public Shape Shape { get; }
        
        /// <summary>
        /// For <see cref="SearchFlag"/> flags.
        /// </summary>
        public IList<SearchFlag> SearchFlags { get; }
        
        /// <summary>
        /// For <see cref="Flag.Filter"/> flags.
        /// </summary>
        public IList<Flag> FilterFlags { get; }
        
        /// <summary>
        /// Effects executed on each <see cref="Entity"/> found.
        /// </summary>
        public IList<EffectName> Effects { get; } 
        
        /// <summary>
        /// Indicates where the <see cref="Search"/> originates from. Can be used to display selection overlays. 
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// If true, this <see cref="Search"/> can only be used inside the <see cref="ResultValidator"/>. False
        /// by default.
        /// </summary>
        public bool UsedForValidator { get; }
    }
}
