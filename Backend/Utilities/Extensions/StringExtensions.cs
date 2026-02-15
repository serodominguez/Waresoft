using System.Globalization;

namespace Utilities.Extensions
{
    public static class StringExtensions
    {
        // Reemplaza guiones bajos con espacios
        public static string? ReplaceUnderscoresWithSpace(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("_", " ");
        }


        //Convierte el texto a formato en minúsculas
        public static string? NormalizeString(this string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.ToLower().Trim();
        }

        //Convierte el texto a formato de primera letra en mayúscula
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

        //Convierte el texto a formato de oración: primera letra mayúscula, resto minúsculas
        public static string? ToSentenceCase(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim().ToLower();

            if (trimmed.Length == 0)
                return null;

            return char.ToUpper(trimmed[0]) + trimmed.Substring(1);
        }

        // Convierte el texto a formato de múltiples oraciones (mayúscula después de . ! ?)
        public static string? ToSentenceCaseMultiple(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var result = value.Trim().ToLower();
            var chars = result.ToCharArray();
            bool capitalizeNext = true;

            for (int i = 0; i < chars.Length; i++)
            {
                if (capitalizeNext && char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i], CultureInfo.GetCultureInfo("es-ES"));
                    capitalizeNext = false;
                }
                else if (chars[i] == '.' || chars[i] == '!' || chars[i] == '?')
                {
                    capitalizeNext = true;
                }
            }

            return new string(chars);
        }
    }
}
