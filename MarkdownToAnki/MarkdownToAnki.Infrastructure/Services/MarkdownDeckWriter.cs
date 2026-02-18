using System.Text;
using System.Globalization;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public class MarkdownDeckWriter : IMarkdownDeckWriter
{
    public async Task WriteAsync(string outputPath, DeckDefinition deckDefinition, List<FlashCardNote> notes)
    {
        string separator = string.IsNullOrWhiteSpace(deckDefinition.Separator)
            ? "---"
            : deckDefinition.Separator;

        var builder = new StringBuilder();
        WriteYamlHeader(builder, deckDefinition, separator);
        builder.AppendLine("---");
        builder.AppendLine();

        var root = BuildHeadingTree(notes);
        WriteHeadingNodes(builder, root, separator, 0);

        await File.WriteAllTextAsync(outputPath, builder.ToString());
    }

    private static void WriteYamlHeader(StringBuilder builder, DeckDefinition deckDefinition, string separator)
    {
        builder.AppendLine("---");
        builder.AppendLine($"deck_name: {ToYamlScalar(deckDefinition.DeckName)}");
        builder.AppendLine($"source: {ToYamlScalar(deckDefinition.Source)}");
        builder.AppendLine($"separator: {ToYamlScalar(separator)}");
        builder.AppendLine("templates:");

        foreach (var template in deckDefinition.Templates)
        {
            builder.AppendLine($"  - name: {ToYamlScalar(template.Name)}");
            builder.AppendLine($"    fields: [{string.Join(", ", template.Fields.Select(ToYamlScalar))}]");
            builder.AppendLine($"    usage: {ToYamlScalar(template.Usage)}");
            WriteYamlScalar(builder, "html_question_format", template.HtmlQuestionFormat, 4);
            WriteYamlScalar(builder, "html_answer_format", template.HtmlAnswerFormat, 4);
            WriteYamlScalar(builder, "css_format", template.CssFormat, 4);
        }
    }

    private static void WriteYamlScalar(StringBuilder builder, string key, string? value, int indentSpaces)
    {
        string indent = new string(' ', indentSpaces);
        string safeValue = value ?? string.Empty;

        if (safeValue.Contains('\n') || safeValue.Contains('\r'))
        {
            builder.AppendLine($"{indent}{key}: |");
            AppendIndentedLines(builder, safeValue, indentSpaces + 2);
            return;
        }

        builder.AppendLine($"{indent}{key}: {ToYamlScalar(safeValue)}");
    }

    private static void AppendIndentedLines(StringBuilder builder, string value, int indentSpaces)
    {
        string indent = new string(' ', indentSpaces);
        string normalized = value.Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = normalized.Split('\n');

        if (lines.Length == 0)
        {
            builder.AppendLine(indent);
            return;
        }

        foreach (var line in lines)
        {
            builder.Append(indent);
            builder.AppendLine(line);
        }
    }

    private static string ToYamlScalar(string? value)
    {
        string safeValue = value ?? string.Empty;
        string escaped = safeValue.Replace("\\", "\\\\").Replace("\"", "\\\"");
        return $"\"{escaped}\"";
    }

    private static HeadingNode BuildHeadingTree(List<FlashCardNote> notes)
    {
        var root = new HeadingNode(string.Empty);

        foreach (var note in notes)
        {
            var path = note.Tags.Count > 0 ? note.Tags : new List<string> { "Untagged" };
            root.AddPath(path, note);
        }

        return root;
    }

    private static void WriteHeadingNodes(StringBuilder builder, HeadingNode node, string separator, int level)
    {
        foreach (var child in node.Children)
        {
            int headingLevel = Math.Min(level + 1, 6);
            builder.AppendLine($"{new string('#', headingLevel)} {FormatHeadingTitle(child.Title)}");
            builder.AppendLine();

            foreach (var note in child.Notes)
            {
                WriteNote(builder, note, separator);
                builder.AppendLine();
            }

            WriteHeadingNodes(builder, child, separator, level + 1);
        }
    }

    private static string FormatHeadingTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return title;
        }

        var words = title
            .Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLowerInvariant())
            .ToArray();

        if (words.Length == 0)
        {
            return title;
        }

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(string.Join(' ', words));
    }

    private static void WriteNote(StringBuilder builder, FlashCardNote note, string separator)
    {
        builder.AppendLine($"```{note.Template.Name}");

        var fieldValues = note.Template.Fields
            .Select(fieldName => note.FieldValues.TryGetValue(fieldName, out var value) ? value : string.Empty)
            .Select(NormalizeFieldValue)
            .ToList();

        builder.AppendLine(string.Join($"\n{separator}\n", fieldValues));
        builder.AppendLine("```");
    }

    private static string NormalizeFieldValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        string normalized = value.Replace("\r\n", "\n").Replace("\r", "\n");
        normalized = normalized.Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase);
        normalized = normalized.Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase);
        normalized = normalized.Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase);
        return normalized;
    }

    private sealed class HeadingNode
    {
        private readonly Dictionary<string, HeadingNode> _childrenByTitle;

        public string Title { get; }
        public List<HeadingNode> Children { get; }
        public List<FlashCardNote> Notes { get; }

        public HeadingNode(string title)
        {
            Title = title;
            Children = new List<HeadingNode>();
            Notes = new List<FlashCardNote>();
            _childrenByTitle = new Dictionary<string, HeadingNode>(StringComparer.Ordinal);
        }

        public void AddPath(IReadOnlyList<string> path, FlashCardNote note)
        {
            HeadingNode current = this;

            foreach (var segment in path)
            {
                current = current.GetOrAddChild(segment);
            }

            current.Notes.Add(note);
        }

        private HeadingNode GetOrAddChild(string title)
        {
            if (_childrenByTitle.TryGetValue(title, out var existing))
            {
                return existing;
            }

            var created = new HeadingNode(title);
            _childrenByTitle[title] = created;
            Children.Add(created);
            return created;
        }
    }
}
