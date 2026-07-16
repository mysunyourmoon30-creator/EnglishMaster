namespace EnglishMaster.Domain.Common;

internal static class SlugGenerator
{
    public static string Generate(
        string? value,
        string fieldName,
        string parameterName,
        int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", parameterName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", parameterName);
        }

        var characters = new List<char>(normalized.Length);
        var lastWasSeparator = false;

        foreach (var character in normalized.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                characters.Add(character);
                lastWasSeparator = false;
                continue;
            }

            if (char.IsWhiteSpace(character) || character is '-' or '_')
            {
                if (!lastWasSeparator && characters.Count > 0)
                {
                    characters.Add('-');
                    lastWasSeparator = true;
                }
            }
        }

        var slug = new string(characters.ToArray()).Trim('-');
        return slug.Length == 0
            ? throw new ArgumentException($"{fieldName} must contain at least one letter or digit.", parameterName)
            : slug;
    }
}
