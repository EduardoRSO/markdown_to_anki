using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Utility service for normalizing tag strings.
/// Removes accents, converts to lowercase, and replaces special characters.
/// </summary>
public class TagNormalizer : ITagNormalizer
{
    public string NormalizeTag(string tag)
    {
        // Remove accents using NFD decomposition
        string normalized = tag.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        string result = sb.ToString();

        // Convert to lowercase and replace spaces/special chars with underscores
        result = result.ToLower();
        result = Regex.Replace(result, @"[^a-z0-9]+", "_");
        result = Regex.Replace(result, @"^_+|_+$", ""); // Remove leading/trailing underscores

        return result;
    }
}
