using System.Globalization;

namespace Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string? NormalizeString(this string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.ToLower().Trim();
        }

        public static string? ToTitleCase(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var lowercaseText = new HashSet<string>
            {
                "de", "del", "la", "las", "el", "los", "y", "o", "a",
                "en", "con", "por", "para", "al"
            };

            var words = value.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var textInfo = new CultureInfo("es-ES", false).TextInfo;

            for (int i = 0; i < words.Length; i++)
            {
                if (i == 0 || !lowercaseText.Contains(words[i]))
                {
                    words[i] = textInfo.ToTitleCase(words[i]);
                }
            }

            return string.Join(" ", words);
        }
    }
}
