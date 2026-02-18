using System.Text.RegularExpressions;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Extracts and parses code blocks from markdown body.
/// Identifies template code blocks and extracts field content.
/// </summary>
public class FlashCardContentExtractor : IFlashCardContentExtractor
{
    private const string CodeBlockStartPattern = @"^```([A-Za-z0-9_]+)\s*$";

    public IEnumerable<CodeBlockContent> ExtractCodeBlocks(string markdownBody)
    {
        var lines = markdownBody.Split('\n');
        var currentCodeBlock = new List<string>();
        string currentTemplate = null;
        int startLineIndex = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            // Check if this is a code block start
            var blockStartMatch = Regex.Match(line, CodeBlockStartPattern);
            if (blockStartMatch.Success && currentTemplate == null)
            {
                currentTemplate = blockStartMatch.Groups[1].Value;
                startLineIndex = i;
                currentCodeBlock.Clear();
                continue;
            }

            // Check if this is a code block end
            if (line.TrimStart().StartsWith("```") && currentTemplate != null)
            {
                yield return new CodeBlockContent
                {
                    TemplateName = currentTemplate,
                    Content = string.Join("\n", currentCodeBlock),
                    LineIndex = startLineIndex
                };

                currentTemplate = null;
                currentCodeBlock.Clear();
                continue;
            }

            // If we're in a code block, accumulate the content
            if (currentTemplate != null)
            {
                currentCodeBlock.Add(line);
            }
        }
    }

    public FlashCardNote ParseCodeBlock(CodeBlockContent blockContent, TemplateDefinition template, string separator, ITagNormalizer tagNormalizer, HeaderHierarchy headerHierarchy)
    {
        var fieldValues = new Dictionary<string, string>();
        var content = blockContent.Content;

        // Split content by separator
        var values = content.Split([separator], StringSplitOptions.None)
            .Select(v => v.Trim())
            .Select(v => v.Replace("\n", "<br>"))  // Convert newlines to HTML line breaks
            .ToList();

        // Map values to fields
        for (int j = 0; j < template.Fields.Count && j < values.Count; j++)
        {
            fieldValues[template.Fields[j]] = values[j];
        }

        // Get tags from header hierarchy, normalized
        var tags = headerHierarchy.GetHierarchyPath()
            .Select(h => tagNormalizer.NormalizeTag(h))
            .ToList();

        var note = new FlashCardNote
        {
            Template = template,
            FieldValues = fieldValues,
            Tags = tags
        };

        return note;
    }
}

/// <summary>
/// Represents the content of a code block extracted from markdown.
/// </summary>
public class CodeBlockContent
{
    public string TemplateName { get; set; }
    public string Content { get; set; }
    public int LineIndex { get; set; }
}
