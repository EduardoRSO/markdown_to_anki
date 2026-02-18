using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Main markdown parser service that orchestrates specialized parsing services.
/// Maintains backward compatibility with IMarkdownParserService interface.
/// </summary>
public class MarkdownParserService : IMarkdownParserService
{
    private readonly IDeckConfigParser _deckConfigParser;
    private readonly IMarkdownHeaderHierarchyExtractor _headerExtractor;
    private readonly IFlashCardContentExtractor _contentExtractor;
    private readonly ITagNormalizer _tagNormalizer;

    public MarkdownParserService(
        IDeckConfigParser deckConfigParser,
        IMarkdownHeaderHierarchyExtractor headerExtractor,
        IFlashCardContentExtractor contentExtractor,
        ITagNormalizer tagNormalizer)
    {
        _deckConfigParser = deckConfigParser ?? throw new ArgumentNullException(nameof(deckConfigParser));
        _headerExtractor = headerExtractor ?? throw new ArgumentNullException(nameof(headerExtractor));
        _contentExtractor = contentExtractor ?? throw new ArgumentNullException(nameof(contentExtractor));
        _tagNormalizer = tagNormalizer ?? throw new ArgumentNullException(nameof(tagNormalizer));
    }

    public async Task<(DeckDefinition, List<FlashCardNote>)> ParseFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);

        // Split YAML front-matter from markdown body
        // Find the first occurrence of "---" at the start of a line
        var firstDelimiter = content.IndexOf("---");
        if (firstDelimiter == -1)
            throw new InvalidOperationException("File must start with ---");

        // Find the second "---" that appears at the start of a line (after a newline)
        var searchStartIndex = firstDelimiter + 3;
        var secondDelimiter = content.IndexOf("\n---", searchStartIndex);
        if (secondDelimiter == -1)
            throw new InvalidOperationException("File must contain closing --- for YAML front-matter");

        // Extract YAML content between the two delimiters
        var yamlContent = content.Substring(firstDelimiter + 3, secondDelimiter - firstDelimiter - 3).Trim();
        var markdownBody = content.Substring(secondDelimiter + 5).Trim(); // +5 to skip "\n---"

        // Parse deck configuration using specialized service
        var deckDefinition = _deckConfigParser.ParseDeckConfig(yamlContent);

        // Extract flash cards from code blocks using specialized services
        var flashCards = ParseFlashCards(markdownBody, deckDefinition);

        return (deckDefinition, flashCards);
    }

    private List<FlashCardNote> ParseFlashCards(string markdownBody, DeckDefinition deckDefinition)
    {
        var flashCards = new List<FlashCardNote>();
        var templateMap = deckDefinition.Templates.ToDictionary(t => t.Name);

        // Extract code blocks from markdown
        var codeBlocks = _contentExtractor.ExtractCodeBlocks(markdownBody);

        foreach (var codeBlock in codeBlocks)
        {
            if (!templateMap.ContainsKey(codeBlock.TemplateName))
                continue;

            var template = templateMap[codeBlock.TemplateName];

            // Get the header hierarchy at the line where this code block appears
            var headerHierarchy = _headerExtractor.ExtractHierarchyAtLine(markdownBody, codeBlock.LineIndex);

            // Parse the code block into a flashcard note
            var note = _contentExtractor.ParseCodeBlock(codeBlock, template, deckDefinition.Separator, _tagNormalizer, headerHierarchy);

            flashCards.Add(note);
        }

        return flashCards;
    }
}
