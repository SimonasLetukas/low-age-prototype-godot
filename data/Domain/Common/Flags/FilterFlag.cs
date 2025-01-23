using System;
using LowAgeData.Domain.Effects;
using LowAgeData.Domain.Entities;
using low_age_prototype_common;
using Newtonsoft.Json;

namespace LowAgeData.Domain.Common.Flags
{
    [JsonConverter(typeof(FilterFlagJsonConverter))]
    public class FilterFlag : EnumValueObject<FilterFlag, FilterFlag.FilterFlags>
    {
        /// <summary>
        /// <see cref="Entity"/> is first in the <see cref="Effect"/> chain
        /// </summary>
        public static FilterFlag Origin => new FilterFlag(FilterFlags.Origin);

        /// <summary>
        /// <see cref="Entity"/> is previous in the <see cref="Effect"/> chain.
        /// </summary>
        public static FilterFlag Source => new FilterFlag(FilterFlags.Source);

        /// <summary>
        /// <see cref="Entity"/> is itself. <see cref="Self"/> is never included to any of the other
        /// <see cref="Common.Filters"/>, so it has to be explicitly added as one.
        /// </summary>
        public static FilterFlag Self => new FilterFlag(FilterFlags.Self);

        /// <summary>
        /// <see cref="Entity"/> is owned by the same player.
        /// </summary>
        public static FilterFlag Player => new FilterFlag(FilterFlags.Player);

        /// <summary>
        /// <see cref="Entity"/> is on the same team, but a different player. 
        /// </summary>
        public static FilterFlag Ally => new FilterFlag(FilterFlags.Ally);

        /// <summary>
        /// <see cref="Entity"/> is on an enemy team.
        /// </summary>
        public static FilterFlag Enemy => new FilterFlag(FilterFlags.Enemy);

        /// <summary>
        /// <see cref="Entity"/> is a <see cref="Unit"/>.
        /// </summary>
        public static FilterFlag Unit => new FilterFlag(FilterFlags.Unit);

        /// <summary>
        /// <see cref="Entity"/> is a <see cref="Structure"/>.
        /// </summary>
        public static FilterFlag Structure => new FilterFlag(FilterFlags.Structure);

        private FilterFlag(FilterFlags @enum) : base(@enum) { }

        private FilterFlag(string? from) : base(from) { }
        
        public enum FilterFlags
        {
            Origin,
            Source,
            Self,
            Player,
            Ally,
            Enemy,
            Unit,
            Structure
        }
        
        private class FilterFlagJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(FilterFlag);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (FilterFlag)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) 
                    return null;
                
                var value = serializer.Deserialize<string>(reader);
                return new FilterFlag(value);
            }
        }
    }
}