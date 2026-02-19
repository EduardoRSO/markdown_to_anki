using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IAnkiCardGenerator
{
    void CreateCardInDeck(
        AnkiCollection collection,
        long deckId,
        long noteTypeId,
        AnkiNoteTypeModelType modelType,
        FlashCardNote cardNote);
}
