using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public class MarkdownParserService : IMarkdownParserService
{
    public Task<(DeckDefinition, List<FlashCardNote>)> ParseFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}