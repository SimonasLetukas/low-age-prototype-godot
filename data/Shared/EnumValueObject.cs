using System;
using System.Collections.Generic;

namespace LowAgeData.Shared
{
    public abstract class EnumValueObject<TClass, TEnum> : ValueObject<TClass> 
        where TClass : ValueObject<TClass>
        where TEnum : struct, IConvertible
    {
        public override string ToString()
        {
            return $"{typeof(TClass).Name}.{Value}";
        }
        
        protected EnumValueObject(TEnum @enum)
        {
            Value = @enum;
        }
        
        protected EnumValueObject(string? from)
        {
            if (from is null)
            {
                Value = default;
                return;
            }
            
            from = from.Substring($"{typeof(TClass).Name}.".Length);
            Value = Enum.TryParse(from, out TEnum @enum) 
                ? @enum 
                : default;
        }
        
        protected TEnum Value { get; }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}