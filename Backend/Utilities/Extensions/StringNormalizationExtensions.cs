namespace Utilities.Extensions
{
    public static class StringNormalizationExtensions
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
    }
}
