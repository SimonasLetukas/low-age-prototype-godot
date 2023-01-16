using System.Collections.Generic;
using low_age_data.Common;
using low_age_data.Domain.Entities;
using low_age_data.Domain.Masks;
using low_age_data.Domain.Shared;

namespace low_age_data.Domain.Logic
{
    /// <summary>
    /// An <see cref="Event"/> describes WHEN, while a <see cref="Validator"/> could afterwards describe IF. The event
    /// names reference <see cref="Location"/> in the effect chain, in which <see cref="Entity"/> refers to
    /// <see cref="Location.Self"/>.
    /// </summary>
    public class Event : ValueObject<Event>
    {
        public override string ToString()
        {
            return $"{nameof(Event)}.{Value}";
        }

        /// <summary>
        /// Interruption is considered anything that disables the use of abilities (NOT attacks): stuns, silences, etc.
        /// </summary>
        public static Event OriginIsInterrupted => new(Events.OriginIsInterrupted);
        public static Event OriginIsDestroyed => new(Events.OriginIsDestroyed);
        public static Event SourceIsDestroyed => new(Events.SourceIsDestroyed);
        public static Event SourceIsNotAdjacent => new(Events.SourceIsNotAdjacent);
        public static Event EntityIsAboutToMove => new(Events.EntityIsAboutToMove);
        public static Event EntityFinishedMoving => new(Events.EntityFinishedMoving);
        public static Event EntityIsAttacked => new(Events.EntityIsAttacked);
        public static Event EntityMeleeAttacks => new(Events.EntityMeleeAttacks);
        public static Event EntityRangedAttacks => new(Events.EntityRangedAttacks);
        public static Event EntityStartedActionPhase => new(Events.EntityStartsActionPhase);
        public static Event EntityStartedAction => new(Events.EntityStartsAction);
        
        /// <summary>
        /// Triggers when a <see cref="Mask"/> is added or removed. 
        /// </summary>
        public static Event EntityMaskChanged => new(Events.EntityMaskChanged);

        private Event(Events @enum)
        {
            Value = @enum;
        }

        private Events Value { get; }

        private enum Events
        {
            OriginIsInterrupted,
            OriginIsDestroyed,
            SourceIsDestroyed,
            SourceIsNotAdjacent,
            EntityIsAboutToMove,
            EntityFinishedMoving,
            EntityIsAttacked,            
            EntityMeleeAttacks,
            EntityRangedAttacks,
            EntityStartsActionPhase,
            EntityStartsAction,
            EntityMaskChanged
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
