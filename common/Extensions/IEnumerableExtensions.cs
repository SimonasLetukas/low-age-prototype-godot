using System.Collections.Generic;
using System.Linq;

namespace LowAgeCommon.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Any() is false;
    }
}
