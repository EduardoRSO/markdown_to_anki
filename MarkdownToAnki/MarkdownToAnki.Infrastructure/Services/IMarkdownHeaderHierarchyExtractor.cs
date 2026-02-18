using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IMarkdownHeaderHierarchyExtractor
{
    IEnumerable<(int lineIndex, HeaderHierarchy hierarchy)> ExtractHeaderHierarchy(string markdownBody);
    HeaderHierarchy ExtractHierarchyAtLine(string markdownBody, int targetLineIndex);
}
