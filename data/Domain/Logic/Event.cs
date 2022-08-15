using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Logic
{
    public class Event : ValueObject<Event>
    {
        public override string ToString()
        {
            return $"{nameof(Event)}.{Value}";
        }

        /// <summary>
        /// Interruption is considered anything that disables the use of abilities (NOT attacks): stuns, silences, etc.
        /// </summary>
        public static Event OriginIsInterrupted => new Event(Events.OriginIsInterrupted);
        public static Event OriginIsDestroyed => new Event(Events.OriginIsDestroyed);
        public static Event SourceIsDestroyed => new Event(Events.SourceIsDestroyed);
        public static Event SourceIsNotAdjacent => new Event(Events.SourceIsNotAdjacent);
        public static Event EntityIsAboutToMove => new Event(Events.EntityIsAboutToMove);
        public static Event EntityFinishedMoving => new Event(Events.EntityFinishedMoving);
        public static Event EntityIsAttacked => new Event(Events.EntityIsAttacked);
        
        /// <summary>
        /// Triggers either at the start of action if entity can have an action, or on action phase if entity has no
        /// initiative and action (i.e. building, feature).
        /// </summary>
        public static Event EntityStartsActionNotOnPower => new Event(Events.EntityStartsActionNotOnPower);
        public static Event EntityReceivedPower => new Event(Events.EntityReceivedPower);

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
            EntityStartsActionNotOnPower,
            EntityReceivedPower,
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
