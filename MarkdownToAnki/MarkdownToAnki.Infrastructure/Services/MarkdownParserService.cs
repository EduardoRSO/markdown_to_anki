using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public class MarkdownParserService : IMarkdownParserService
{
    public (DeckDefinition, List<FlashCardNote>) ParseFile(string filePath)
    {
        throw new NotImplementedException();
    }
}