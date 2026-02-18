using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IFlashCardContentExtractor
{
    IEnumerable<CodeBlockContent> ExtractCodeBlocks(string markdownBody);
    FlashCardNote ParseCodeBlock(CodeBlockContent blockContent, TemplateDefinition template, string separator, ITagNormalizer tagNormalizer, HeaderHierarchy headerHierarchy);
}
