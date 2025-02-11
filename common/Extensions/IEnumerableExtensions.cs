namespace LowAgeCommon.Extensions;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable.Any() is false;
        
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> o) where T : class 
        => o.Where(x => x != null)!;
}