using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Creates individual flashcard notes in the correct deck with metadata.
/// </summary>
public class AnkiCardGenerator : IAnkiCardGenerator
{
    public void CreateCardInDeck(
        AnkiCollection collection,
        long deckId,
        long noteTypeId,
        FlashCardNote cardNote)
    {
        // Convert tags list to space-separated string format (Anki tag format)
        string tags = cardNote.Tags.Count > 0
            ? " " + string.Join(" ", cardNote.Tags) + " "
            : "";

        // Create note with metadata
        collection.CreateNoteWithMetadata(
            deckId,
            noteTypeId,
            [.. cardNote.FieldValues.Values],
            guid: Guid.NewGuid().ToString(),
            tags: tags,
            modifiedDateTime: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            updateSequenceNumber: -1
        );
    }
}
