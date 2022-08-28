using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class Research : ValueObject<Research>
    {
        public override string ToString()
        {
            return $"{nameof(Research)}.{Value}";
        }

        public static class Revelators
        {
            public static Research PoisonedSlits => new(Researches.PoisonedSlits);
            public static Research SpikedRope => new(Researches.SpikedRope);
            public static Research QuestionableCargo => new(Researches.QuestionableCargo);
            public static Research HumanfleshRations => new(Researches.HumanfleshRations);
            public static Research AdaptiveDigestion => new(Researches.AdaptiveDigestion);
        }

        public static class Uee
        {
            public static Research HoverboardReignition => new(Researches.HoverboardReignition);
            public static Research ExplosiveShrapnel => new(Researches.ExplosiveShrapnel);
            public static Research MDPractice => new(Researches.MDPractice);
            public static Research CelestiumCoatedMaterials => new(Researches.CelestiumCoatedMaterials);
            public static Research HardenedMatrix => new(Researches.HardenedMatrix);
        }

        private Research(Researches @enum)
        {
            Value = @enum;
        }

        private Researches Value { get; }

        private enum Researches
        {
            PoisonedSlits,
            SpikedRope,
            QuestionableCargo,
            HumanfleshRations,
            AdaptiveDigestion,
            HoverboardReignition,
            ExplosiveShrapnel,
            MDPractice,
            CelestiumCoatedMaterials,
            HardenedMatrix
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
