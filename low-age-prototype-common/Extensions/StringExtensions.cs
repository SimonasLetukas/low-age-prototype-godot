namespace low_age_prototype_common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimForLogs(this string input) => input.Length < 2000 
            ? input
            : input.Substring(0, 2000);
    
        public static string WrapToLines(this string inputText, int lineLength) 
        {
            var stringSplit = inputText.Split(' ');
            var charCounter = 0;
            var finalString = "";
 
            foreach (var t in stringSplit)
            {
                finalString += t + " ";
                charCounter += t.Length;

                if (charCounter <= lineLength) 
                    continue;
            
                finalString += "\n";
                charCounter = 0;
            }
        
            return finalString;
        }

        public static bool IsNotNullOrEmpty(this string input) => string.IsNullOrEmpty(input) is false;
    }
}