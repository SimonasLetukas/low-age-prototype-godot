using System.Collections.Generic;
using low_age_data.Domain.Effects;

namespace low_age_data.Domain.Logic
{
    public class ResultValidator : Validator
    {
        // Usually checks searched area for a condition on resulted targets, could have more applications in the future
        public ResultValidator(EffectName effect, IList<Condition> conditions) : base(conditions, $"{nameof(Validator)}.{nameof(ResultValidator)}")
        {
            Effect = effect;
        }

        public EffectName Effect { get; }
    }
}
