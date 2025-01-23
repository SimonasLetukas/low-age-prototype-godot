using System.Collections.Generic;
using System.Linq;

namespace low_age_prototype_common.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Any() is false;
    }
}
