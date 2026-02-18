using System.Text.RegularExpressions;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Extracts markdown header hierarchy from markdown body.
/// Parses headers and maintains state as the markdown is processed.
/// </summary>
public class MarkdownHeaderHierarchyExtractor : IMarkdownHeaderHierarchyExtractor
{
    private const string HeaderPattern = @"^(#{1,6})\s+(.+)$";

    public IEnumerable<(int lineIndex, HeaderHierarchy hierarchy)> ExtractHeaderHierarchy(string markdownBody)
    {
        var lines = markdownBody.Split('\n');
        var currentHierarchy = new HeaderHierarchy();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd();
            var headerMatch = Regex.Match(line, HeaderPattern);

            if (headerMatch.Success)
            {
                int level = headerMatch.Groups[1].Value.Length;
                string headerText = headerMatch.Groups[2].Value.TrimEnd();

                currentHierarchy.UpdateHeader(level, headerText);
                yield return (i, currentHierarchy.Clone());
            }
        }
    }

    public HeaderHierarchy ExtractHierarchyAtLine(string markdownBody, int targetLineIndex)
    {
        var lines = markdownBody.Split('\n');
        var currentHierarchy = new HeaderHierarchy();

        for (int i = 0; i < lines.Length && i <= targetLineIndex; i++)
        {
            var line = lines[i].TrimEnd();
            var headerMatch = Regex.Match(line, HeaderPattern);

            if (headerMatch.Success)
            {
                int level = headerMatch.Groups[1].Value.Length;
                string headerText = headerMatch.Groups[2].Value.TrimEnd();
                currentHierarchy.UpdateHeader(level, headerText);
            }
        }

        return currentHierarchy;
    }
}
