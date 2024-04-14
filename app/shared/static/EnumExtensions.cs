using System;

public static class EnumExtensions
{
    public static int CountTo<TEnum>(this TEnum rotation, TEnum targetRotation) where TEnum : struct, Enum
    {
        var delta = Convert.ToInt32(targetRotation) - Convert.ToInt32(rotation);
        delta = delta < 0 ? delta + Enum.GetNames(typeof(TEnum)).Length : delta;
        return delta;
    }

    public static int GetCountFromDefaultTo<TEnum>(ActorRotation targetRotation) where TEnum : struct, Enum 
        => CountTo(default, targetRotation);
}