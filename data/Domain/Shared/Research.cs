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
            public static Research PoisonedSlits => new Research(Researches.PoisonedSlits);
            public static Research SpikedRope => new Research(Researches.SpikedRope);
            public static Research QuestionableCargo => new Research(Researches.QuestionableCargo);
            public static Research HumanfleshRations => new Research(Researches.HumanfleshRations);
            public static Research AdaptiveDigestion => new Research(Researches.AdaptiveDigestion);
        }

        public static class Uee
        {
            public static Research HoverboardReignition => new Research(Researches.HoverboardReignition);
            public static Research ExplosiveShrapnel => new Research(Researches.ExplosiveShrapnel);
            public static Research MDPractice => new Research(Researches.MDPractice);
            public static Research CelestiumCoatedMaterials => new Research(Researches.CelestiumCoatedMaterials);
            public static Research HardenedMatrix => new Research(Researches.HardenedMatrix);
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
