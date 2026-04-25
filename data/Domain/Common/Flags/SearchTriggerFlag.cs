using LowAgeData.Domain.Abilities;
using LowAgeData.Domain.Entities;
using LowAgeCommon;
using LowAgeData.Domain.Behaviours;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags;

/// <summary>
/// <para>
/// Used to control events when the effects should be applied or removed when <see cref="SearchTriggerFlag"/> is
/// used as a <see cref="Passive.PeriodicSearchEffect"/>. When counteracting the applied effects, any
/// <see cref="Behaviours.Buff"/>s down the line are allowed to execute their
/// <see cref="Behaviours.Buff.ConditionalEffects"/> before removal.
/// </para>
/// <para>
/// Always tries to counteract applied effects when the underlying <see cref="Passive"/> ability gets
/// disabled or when the owner actor is destroyed.
/// </para>
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
    /// Counteracts any <see cref="Effects"/> added as part of <see cref="Effects.Search"/>
    /// <see cref="Effects.Search.Effects"/> for an <see cref="Entity"/> that leaves the
    /// <see cref="Effects.Search.Shape"/>.
    /// </summary>
    public static SearchTriggerFlag RemovedOnExit => new SearchTriggerFlag(SearchFlags.RemovedOnExit);
                
    /// <summary>
    /// Applies <see cref="Effects.Search"/> after <see cref="Durations.EndsAt"/> (at the end of action
    /// for the <see cref="FilterFlag.Source"/> <see cref="Entity"/> -- which issued this
    /// <see cref="Passive.PeriodicSearchEffect"/>).
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

    private SearchTriggerFlag(SearchFlags @enum) : base(@enum) { }
        
    private SearchTriggerFlag(string? from) : base(from) { }
        
    public enum SearchFlags
    {
        AppliedOnEnter,
        RemovedOnExit,
        AppliedOnSourceAction,
        AppliedOnEveryAction,
        AppliedOnActionPhaseStart,
        AppliedOnActionPhaseEnd,
        AppliedOnPlanningPhaseStart,
        AppliedOnPlanningPhaseEnd,
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