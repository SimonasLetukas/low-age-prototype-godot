using LowAgeData.Domain.Logic;
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
            SearchHeight height,
            IList<IFilterItem> filters,
            IList<SearchTriggerFlag>? triggerFlags = null,
            IList<EffectId>? effects = null,
            Location? target = null,
            IList<Validator>? validators = null,
            bool? usedForValidator = null) 
            : base(
                id, 
                target ?? Location.Inherited,
                validators ?? new List<Validator>())
        {
            Shape = shape;
            Height = height;
            Filters = filters;
            TriggerFlags = triggerFlags ?? [];
            Effects = effects ?? new List<EffectId>();
            Location = target ?? Location.Inherited;
            UsedForValidator = usedForValidator ?? false;
        }
        
        /// <summary>
        /// Type of the <see cref="Shape"/> and its area which should be <see cref="Search"/>ed.
        /// </summary>
        public IShape Shape { get; }
        
        /// <summary>
        /// Specifies the <see cref="SearchHeight"/> that the <see cref="Search"/> targets.
        /// </summary>
        public SearchHeight Height { get; }
        
        /// <summary>
        /// <see cref="Common.Filters"/> done for all found results.
        /// </summary>
        public IList<IFilterItem> Filters { get; }
        
        /// <summary>
        /// For <see cref="SearchTriggerFlag"/> flags.
        /// </summary>
        public IList<SearchTriggerFlag> TriggerFlags { get; }
        
        /// <summary>
        /// Effects executed on each <see cref="Entity"/> found.
        /// </summary>
        public IList<EffectId> Effects { get; } 
        
        /// <summary>
        /// Indicates where the <see cref="Search"/> originates from. Can be used to display selection overlays.
        ///
        /// <see cref="Location.Inherited"/> will try to return all entities on the map if <see cref="Shape"/> is
        /// <see cref="Map"/>. Otherwise, it will default to <see cref="Location.Entity"/>.
        ///
        /// Note: Used for documentation purposes only, will be automatically resolved to <see cref="Effect.Target"/>.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// If true, this <see cref="Search"/> can only be used inside the <see cref="ResultValidator"/>. False
        /// by default.
        /// </summary>
        public bool UsedForValidator { get; }
    }
}
