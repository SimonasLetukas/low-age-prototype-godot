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
            public static ResearchName PoisonedSlits => new($"{nameof(Revelators)}{nameof(PoisonedSlits)}".ToKebabCase());
            public static ResearchName SpikedRope => new($"{nameof(Revelators)}{nameof(SpikedRope)}".ToKebabCase());
            public static ResearchName QuestionableCargo => new($"{nameof(Revelators)}{nameof(QuestionableCargo)}".ToKebabCase());
            public static ResearchName HumanfleshRations => new($"{nameof(Revelators)}{nameof(HumanfleshRations)}".ToKebabCase());
            public static ResearchName AdaptiveDigestion => new($"{nameof(Revelators)}{nameof(AdaptiveDigestion)}".ToKebabCase());
        }

        public static class Uee
        {
            public static ResearchName FusionCoreUpgrade => new($"{nameof(Uee)}{nameof(FusionCoreUpgrade)}".ToKebabCase());
            public static ResearchName CelestiumCoreUpgrade => new($"{nameof(Uee)}{nameof(CelestiumCoreUpgrade)}".ToKebabCase());
            public static ResearchName HoverboardReignition => new($"{nameof(Uee)}{nameof(HoverboardReignition)}".ToKebabCase());
            public static ResearchName ExplosiveShrapnel => new($"{nameof(Uee)}{nameof(ExplosiveShrapnel)}".ToKebabCase());
            public static ResearchName MdPractice => new($"{nameof(Uee)}{nameof(MdPractice)}".ToKebabCase());
            public static ResearchName CelestiumCoatedMaterials => new($"{nameof(Uee)}{nameof(CelestiumCoatedMaterials)}".ToKebabCase());
            public static ResearchName HardenedMatrix => new($"{nameof(Uee)}{nameof(HardenedMatrix)}".ToKebabCase());
        }
    }
}
