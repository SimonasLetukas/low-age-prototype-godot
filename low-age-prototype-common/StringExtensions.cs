using System.Text;

namespace low_age_prototype_common
{
    public static class StringExtensions
    {
        public static string ToKebabCase(this string text)
        {
            if (text.Length < 2)
            {
                return text;
            }

            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));

            for (var i = 1; i < text.Length; ++i)
            {
                var c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('-');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string CamelCaseToWords(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                input, 
                "([A-Z])", 
                " $1", 
                System.Text.RegularExpressions.RegexOptions.Compiled)
                .Trim();
        }

        public static string KebabCaseToWords(this string input)
        {
            var builder = new StringBuilder();
            var caseFlag = true;
            foreach (var c in input)
            {
                if (c == '-')
                {
                    caseFlag = true;
                }
                else if (caseFlag)
                {
                    builder.Append(" ");
                    builder.Append(char.ToUpper(c));
                    caseFlag = false;
                }
                else
                {
                    builder.Append(char.ToLower(c));
                }
            }
            return builder.ToString().Trim();
        }
    }
}
