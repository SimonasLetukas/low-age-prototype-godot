using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    /// <summary>
    /// Used to control events when the effects should be applied or removed when <see cref="SearchTriggerFlag"/> is
    /// used as a <see cref="Passive.PeriodicEffect"/>. When counteracting the applied effects, any
    /// <see cref="Behaviours.Buff"/>s down the line are allowed to execute their
    /// <see cref="Behaviours.Buff.ConditionalEffects"/> before removal.
    /// </summary>
    [JsonConverter(typeof(SearchFlagJsonConverter))]
    public class SearchTriggerFlag : EnumValueObject<SearchTriggerFlag, SearchTriggerFlag.SearchFlags>
    {
        /// <summary>
        /// Applies <see cref="Effects.Search"/> whenever a new <see cref="Entity"/> enters the
        /// <see cref="Effects.Search.Shape"/>).
        /// </summary>
        public static SearchTriggerFlag AppliedOnEnter => new SearchTriggerFlag(SearchFlags.AppliedOnEnter);
                
        /// <summary>
        /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> (at the end of action
        /// for the <see cref="FilterFlag.Source"/> <see cref="Entity"/> -- which issued this
        /// <see cref="Passive.PeriodicEffect"/>).
        /// </summary>
        public static SearchTriggerFlag AppliedOnSourceAction => new SearchTriggerFlag(SearchFlags.AppliedOnSourceAction);
                
        /// <summary>
        /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> for every action in
        /// action phase.
        /// </summary>
        public static SearchTriggerFlag AppliedOnEveryAction => new SearchTriggerFlag(SearchFlags.AppliedOnEveryAction);

        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the start of every action phase.
        /// </summary>
        public static SearchTriggerFlag AppliedOnActionPhaseStart => new SearchTriggerFlag(SearchFlags.AppliedOnActionPhaseStart);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the end of every action phase.
        /// </summary>
        public static SearchTriggerFlag AppliedOnActionPhaseEnd => new SearchTriggerFlag(SearchFlags.AppliedOnActionPhaseEnd);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the start of every planning phase.
        /// </summary>
        public static SearchTriggerFlag AppliedOnPlanningPhaseStart => new SearchTriggerFlag(SearchFlags.AppliedOnPlanningPhaseStart);
                
        /// <summary>
        /// <see cref="Effects.Search"/> is applied at the end of every planning phase.
        /// </summary>
        public static SearchTriggerFlag AppliedOnPlanningPhaseEnd => new SearchTriggerFlag(SearchFlags.AppliedOnPlanningPhaseEnd);
                
        /// <summary>
        /// Counteracts any <see cref="Effects"/> added as part of <see cref="Effects.Search"/>
        /// <see cref="Effects.Search.Effects"/> for an <see cref="Entity"/> that leaves the
        /// <see cref="Effects.Search.Shape"/>.
        /// </summary>
        public static SearchTriggerFlag RemovedOnExit => new SearchTriggerFlag(SearchFlags.RemovedOnExit);
                
        /// <summary>
        /// At the start of every planning phase, counteracts any <see cref="Effects"/> added as part of
        /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
        /// </summary>
        public static SearchTriggerFlag RemovedOnPlanningPhaseStart => new SearchTriggerFlag(SearchFlags.RemovedOnPlanningPhaseStart);
                
        /// <summary>
        /// At the end of every planning phase, counteracts any <see cref="Effects"/> added as part of
        /// <see cref="Effects.Search"/> <see cref="Effects.Search.Effects"/>.
        /// </summary>
        public static SearchTriggerFlag RemovedOnPlanningPhaseEnd => new SearchTriggerFlag(SearchFlags.RemovedOnPlanningPhaseEnd);

        private SearchTriggerFlag(SearchFlags @enum) : base(@enum) { }
        
        private SearchTriggerFlag(string? from) : base(from) { }
        
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
                return objectType == typeof(SearchTriggerFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (SearchTriggerFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new SearchTriggerFlag(value);
            }
        }
    }
}