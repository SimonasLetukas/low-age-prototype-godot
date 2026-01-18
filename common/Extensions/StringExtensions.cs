using System.Text;

namespace LowAgeCommon.Extensions
{
    public static class StringExtensions
    {
        public static string TrimForLogs(this string input, int length = 2000) => input.Length < length 
            ? input
            : input[..length];
    
        public static string WrapToLines(this string inputText, int lineLength)
        {
            var lines = inputText.Split('\n');
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var charCounter = 0;

                foreach (var word in words)
                {
                    if (charCounter + word.Length + (charCounter > 0 ? 1 : 0) > lineLength)
                    {
                        result.Append('\n');
                        charCounter = 0;
                    }

                    if (charCounter > 0)
                    {
                        result.Append(' ');
                        charCounter++;
                    }

                    result.Append(word);
                    charCounter += word.Length;
                }

                result.Append('\n');
            }

            return result.ToString().TrimEnd('\n');
        }

        public static bool IsNotNullOrEmpty(this string? input) => string.IsNullOrEmpty(input) is false;
    }
}