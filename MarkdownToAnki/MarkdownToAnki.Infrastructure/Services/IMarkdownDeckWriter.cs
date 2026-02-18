using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IMarkdownDeckWriter
{
    Task WriteAsync(string outputPath, DeckDefinition deckDefinition, List<FlashCardNote> notes);
}
