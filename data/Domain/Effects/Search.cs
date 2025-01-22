using LowAgeData.Domain.Logic;
using System.Collections.Generic;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Common.Filters;
using LowAgeData.Domain.Common.Flags;
using LowAgeData.Domain.Common.Shape;
using LowAgeData.Domain.Entities;

namespace LowAgeData.Domain.Effects
{
    public class Search : Effect
    {
        public Search(
            EffectId id,
            IShape shape,
            IList<SearchFlag> searchFlags,
            IList<IFilterItem> filters,
            IList<EffectId>? effects = null,
            Location? location = null,
            IList<Validator>? validators = null,
            bool? usedForValidator = null) : base(id, validators ?? new List<Validator>())
        {
            Shape = shape;
            SearchFlags = searchFlags;
            Filters = filters;
            Effects = effects ?? new List<EffectId>();
            Location = location ?? Location.Inherited;
            UsedForValidator = usedForValidator ?? false;
        }
        
        /// <summary>
        /// Type of the <see cref="Shape"/> and its area which should be <see cref="Search"/>ed.
        /// </summary>
        public IShape Shape { get; }
        
        /// <summary>
        /// For <see cref="SearchFlag"/> flags.
        /// </summary>
        public IList<SearchFlag> SearchFlags { get; }
        
        /// <summary>
        /// For <see cref="Common.Filters"/>.
        /// </summary>
        public IList<IFilterItem> Filters { get; }
        
        /// <summary>
        /// Effects executed on each <see cref="Entity"/> found.
        /// </summary>
        public IList<EffectId> Effects { get; } 
        
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
