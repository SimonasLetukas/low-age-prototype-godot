using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Any() is false;
}