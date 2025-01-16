using System;
using low_age_data.Domain.Abilities;
using low_age_data.Domain.Entities;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common.Flags
{
    /// <summary>
    /// Used to control events when the effects should be applied or removed when <see cref="SearchFlag"/> is
    /// used as a <see cref="Passive.PeriodicEffect"/>. When counteracting the applied effects, any
    /// <see cref="Behaviours.Buff"/>s down the line are allowed to execute their
    /// <see cref="Behaviours.Buff.ConditionalEffects"/> before removal.
    /// </summary>
    [JsonConverter(typeof(SearchFlagJsonConverter))]
    public class SearchFlag : EnumValueObject<SearchFlag, SearchFlag.SearchFlags>
    {
        /// <summary>
        /// Applies <see cref="Effects.Search"/> whenever a new actor enters the
        /// <see cref="Effects.Search.Shape"/>).
        /// </summary>
        public static SearchFlag AppliedOnEnter => new SearchFlag(SearchFlags.AppliedOnEnter);
                
        /// <summary>
        /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> (at the end of action
        /// for the <see cref="FilterFlag.Source"/> <see cref="Entity"/> -- which issued this
        /// <see cref="Passive.PeriodicEffect"/>).
        /// </summary>
        public static SearchFlag AppliedOnSourceAction => new SearchFlag(SearchFlags.AppliedOnSourceAction);
                
        /// <summary>
        /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> for every action in
        /// action phase.
        /// </summary>
        public static SearchFlag AppliedOnEveryAction => new SearchFlag(SearchFlags.AppliedOnEveryAction);

        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the start of every action phase.
        /// </summary>
        public static SearchFlag AppliedOnActionPhaseStart => new SearchFlag(SearchFlags.AppliedOnActionPhaseStart);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the end of every action phase.
        /// </summary>
        public static SearchFlag AppliedOnActionPhaseEnd => new SearchFlag(SearchFlags.AppliedOnActionPhaseEnd);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the start of every planning phase.
        /// </summary>
        public static SearchFlag AppliedOnPlanningPhaseStart => new SearchFlag(SearchFlags.AppliedOnPlanningPhaseStart);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the end of every planning phase.
        /// </summary>
        public static SearchFlag AppliedOnPlanningPhaseEnd => new SearchFlag(SearchFlags.AppliedOnPlanningPhaseEnd);
                
        /// <summary>
        /// Counteracts any <see cref="Effects"/> added as part of <see cref="Effects.Search"/>
        /// <see cref="Effects.Search.Effects"/> for an <see cref="Entities.Actors.Actor"/> that leaves the
        /// <see cref="Effects.Search.Shape"/>.
        /// </summary>
        public static SearchFlag RemovedOnExit => new SearchFlag(SearchFlags.RemovedOnExit);
                
        /// <summary>
        /// At the start of every planning phase, counteracts any <see cref="Effects"/> added as part of
        /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
        /// </summary>
        public static SearchFlag RemovedOnPlanningPhaseStart => new SearchFlag(SearchFlags.RemovedOnPlanningPhaseStart);
                
        /// <summary>
        /// At the end of every planning phase, counteracts any <see cref="Effects"/> added as part of
        /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
        /// </summary>
        public static SearchFlag RemovedOnPlanningPhaseEnd => new SearchFlag(SearchFlags.RemovedOnPlanningPhaseEnd);

        private SearchFlag(SearchFlags @enum) : base(@enum) { }
        
        private SearchFlag(string? from) : base(from) { }
        
        public enum SearchFlags
        {
            AppliedOnEnter,
            AppliedOnSourceAction,
            AppliedOnEveryAction,
            AppliedOnActionPhaseStart,
            AppliedOnActionPhaseEnd,
            AppliedOnPlanningPhaseStart,
            AppliedOnPlanningPhaseEnd,
            RemovedOnExit,
            RemovedOnPlanningPhaseStart,
            RemovedOnPlanningPhaseEnd
        }
        
        private class SearchFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(SearchFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (SearchFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new SearchFlag(value);
            }
        }
    }
}