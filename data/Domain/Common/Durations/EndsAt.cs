using System;
using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Entities.Actors;
using low_age_data.Domain.Entities.Doodads;
using low_age_data.Shared;
using Newtonsoft.Json;

namespace low_age_data.Domain.Common.Durations
{
    /// <summary>
    /// Used to calculate duration. Calculation is done on the last <see cref="Actor"/> in the chain (e.g. when
    /// an <see cref="Actor"/> creates a <see cref="Doodad"/> and there is a <see cref="Buff"/> with
    /// EndsAt.EndOf.Next.Action, this then refers to the <see cref="Actor"/>'s action), unless stated otherwise.
    /// </summary>
    [JsonConverter(typeof(EndsAtJsonConverter))]
    public class EndsAt : EnumValueObject<EndsAt, EndsAt.Durations>
    {
        public static EndsAt Death => new EndsAt(Durations.Death);
        public static EndsAt Instant => new EndsAt(Durations.Instant);

        public static class StartOf
        {
            public static class Next
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfNextPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfNextActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfNextAction);
            }

            public static class Second
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfSecondPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfSecondActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfSecondAction);
            }

            public static class Third
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfThirdPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfThirdActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfThirdAction);
            }

            public static class Fourth
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfFourthPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.StartOfFourthActionPhase);
                public static EndsAt Action => new EndsAt(Durations.StartOfFourthAction);
            }

            public static class Tenth
            {
                public static EndsAt Planning => new EndsAt(Durations.StartOfTenthPlanning);
            }
        }

        public static class EndOf
        {
            public static class This
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfThisPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfThisActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfThisAction);
            }

            public static class Next
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfNextPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfNextActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfNextAction);
            }

            public static class Second
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfSecondPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfSecondActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfSecondAction);
            }

            public static class Third
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfThirdPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfThirdActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfThirdAction);
            }

            public static class Fourth
            {
                public static EndsAt Planning => new EndsAt(Durations.EndOfFourthPlanning);
                public static EndsAt ActionPhase => new EndsAt(Durations.EndOfFourthActionPhase);
                public static EndsAt Action => new EndsAt(Durations.EndOfFourthAction);
            }
        }

        private EndsAt(Durations @enum) : base(@enum) { }

        private EndsAt(string? from) : base(from) { }
        
        public enum Durations
        {
            Death,
            Instant,
            StartOfNextPlanning,
            StartOfNextActionPhase,
            StartOfNextAction,
            StartOfSecondPlanning,
            StartOfSecondActionPhase,
            StartOfSecondAction,
            StartOfThirdPlanning,
            StartOfThirdActionPhase,
            StartOfThirdAction,
            StartOfFourthPlanning,
            StartOfFourthActionPhase,
            StartOfFourthAction,
            StartOfTenthPlanning,
            EndOfThisPlanning,
            EndOfThisActionPhase,
            EndOfThisAction,
            EndOfNextPlanning,
            EndOfNextActionPhase,
            EndOfNextAction,
            EndOfSecondPlanning,
            EndOfSecondActionPhase,
            EndOfSecondAction,
            EndOfThirdPlanning,
            EndOfThirdActionPhase,
            EndOfThirdAction,
            EndOfFourthPlanning,
            EndOfFourthActionPhase,
            EndOfFourthAction
        }

        private class EndsAtJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(EndsAt);
            }
            
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var id = (EndsAt)value!;
                serializer.Serialize(writer, id.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                var value = serializer.Deserialize<string>(reader);
                return new EndsAt(value);
            }
        }
    }
}
