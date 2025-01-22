using System;
using LowAgeData.Domain.Common;
using LowAgeData.Domain.Entities;
using LowAgeData.Domain.Masks;
using LowAgeData.Shared;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Logic
{
    /// <summary>
    /// An <see cref="Event"/> describes WHEN, while a <see cref="Validator"/> could afterwards describe IF. The event
    /// names reference <see cref="Location"/> in the effect chain, in which <see cref="Entity"/> refers to
    /// <see cref="Location.Self"/>.
    /// </summary>
    [JsonConverter(typeof(EventJsonConverter))]
    public class Event : EnumValueObject<Event, Event.Events>
    {
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
        public static Event EntityMeleeAttacks => new Event(Events.EntityMeleeAttacks);
        public static Event EntityRangedAttacks => new Event(Events.EntityRangedAttacks);
        public static Event EntityStartedActionPhase => new Event(Events.EntityStartsActionPhase);
        public static Event EntityStartedAction => new Event(Events.EntityStartsAction);
        
        /// <summary>
        /// Triggers when a <see cref="Mask"/> is added or removed. 
        /// </summary>
        public static Event EntityMaskChanged => new Event(Events.EntityMaskChanged);

        private Event(Events @enum) : base(@enum) { }

        private Event(string? from) : base(from) { }

        public enum Events
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
        
        private class EventJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Event);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (Event)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new Event(value);
            }
        }
    }
}
