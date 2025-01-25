using System;

namespace LowAgeCommon.Extensions
{
    public static class EnumExtensions
    {
        public static int CountTo<TEnum>(this TEnum enum1, TEnum enum2) where TEnum : struct, Enum
        {
            var delta = Convert.ToInt32(enum2) - Convert.ToInt32(enum1);
            delta = delta < 0 ? delta + Enum.GetNames(typeof(TEnum)).Length : delta;
            return delta;
        }
    }
}