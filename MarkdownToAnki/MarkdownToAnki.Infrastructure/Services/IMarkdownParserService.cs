using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IMarkdownParserService
{
    Task<(DeckDefinition, List<FlashCardNote>)> ParseFileAsync(string filePath);
}

