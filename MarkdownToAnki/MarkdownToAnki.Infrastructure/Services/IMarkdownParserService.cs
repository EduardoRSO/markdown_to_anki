using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IMarkdownParserService
{
    (DeckDefinition, List<FlashCardNote>) ParseFile(string filePath);
}

