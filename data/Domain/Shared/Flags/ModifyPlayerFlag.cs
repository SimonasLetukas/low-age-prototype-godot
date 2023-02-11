using System.Collections.Generic;
using low_age_data.Common;

namespace low_age_data.Domain.Shared.Flags
{
    public class ModifyPlayerFlag : ValueObject<ModifyPlayerFlag>
    {
        public override string ToString()
        {
            return $"{nameof(ModifyPlayerFlag)}.{Value}";
        }
        
        public static ModifyPlayerFlag GameLost => new(ModifyPlayerFlags.GameLost);

        private ModifyPlayerFlag(ModifyPlayerFlags @enum)
        {
            Value = @enum;
        }

        private ModifyPlayerFlags Value { get; }

        private enum ModifyPlayerFlags
        {
            GameLost
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}