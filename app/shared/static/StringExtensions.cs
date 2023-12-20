public static class StringExtensions
{
    public static string TrimForLogs(this string input) => input.Length < 2000 
        ? input
        : input.Substring(0, 2000);
}