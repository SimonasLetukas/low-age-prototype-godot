using low_age_data.Common;

namespace low_age_data.Domain.Shared
{
    public class ResearchName : Name
    {
        private ResearchName(string value) : base($"research-{value}")
        {
        }

        public static class Revelators
        {
            public static ResearchName PoisonedSlits => new ResearchName($"{nameof(Revelators)}{nameof(PoisonedSlits)}".ToKebabCase());
            public static ResearchName SpikedRope => new ResearchName($"{nameof(Revelators)}{nameof(SpikedRope)}".ToKebabCase());
            public static ResearchName QuestionableCargo => new ResearchName($"{nameof(Revelators)}{nameof(QuestionableCargo)}".ToKebabCase());
            public static ResearchName HumanfleshRations => new ResearchName($"{nameof(Revelators)}{nameof(HumanfleshRations)}".ToKebabCase());
            public static ResearchName AdaptiveDigestion => new ResearchName($"{nameof(Revelators)}{nameof(AdaptiveDigestion)}".ToKebabCase());
        }

        public static class Uee
        {
            public static ResearchName FusionCoreUpgrade => new ResearchName($"{nameof(Uee)}{nameof(FusionCoreUpgrade)}".ToKebabCase());
            public static ResearchName CelestiumCoreUpgrade => new ResearchName($"{nameof(Uee)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase());
            public static ResearchName HeightenedConductivity => new ResearchName($"{nameof(Uee)}{nameof(HeightenedConductivity)}".ToKebabCase());
            public static ResearchName HoverboardReignition => new ResearchName($"{nameof(Uee)}{nameof(HoverboardReignition)}".ToKebabCase());
            public static ResearchName ExplosiveShrapnel => new ResearchName($"{nameof(Uee)}{nameof(ExplosiveShrapnel)}".ToKebabCase());
            public static ResearchName MdPractice => new ResearchName($"{nameof(Uee)}{nameof(MdPractice)}".ToKebabCase());
            public static ResearchName CelestiumCoatedMaterials => new ResearchName($"{nameof(Uee)}{nameof(CelestiumCoatedMaterials)}".ToKebabCase());
            public static ResearchName HardenedMatrix => new ResearchName($"{nameof(Uee)}{nameof(HardenedMatrix)}".ToKebabCase());
        }
    }
}
