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
        AnkiNoteTypeModelType modelType,
        FlashCardNote cardNote)
    {
        // Convert tags list to space-separated string format (Anki tag format)
        string tags = cardNote.Tags.Count > 0
            ? " " + string.Join(" ", cardNote.Tags) + " "
            : "";

        var fields = BuildFields(cardNote, modelType);

        // Create note with metadata
        collection.CreateNoteWithMetadata(
            deckId,
            noteTypeId,
            fields,
            guid: Guid.NewGuid().ToString(),
            tags: tags,
            modifiedDateTime: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            updateSequenceNumber: -1
        );
    }

    private static string[] BuildFields(FlashCardNote cardNote, AnkiNoteTypeModelType modelType)
    {
        if (modelType == AnkiNoteTypeModelType.Cloze)
        {
            var primaryFieldName = cardNote.Template.Fields.FirstOrDefault();
            var secondaryFieldName = cardNote.Template.Fields.Skip(1).FirstOrDefault();

            var textValue = !string.IsNullOrWhiteSpace(primaryFieldName) && cardNote.FieldValues.TryGetValue(primaryFieldName, out var primaryValue)
                ? primaryValue
                : string.Empty;

            var backExtraValue = !string.IsNullOrWhiteSpace(secondaryFieldName) && cardNote.FieldValues.TryGetValue(secondaryFieldName, out var secondaryValue)
                ? secondaryValue
                : string.Empty;

            return [textValue, backExtraValue];
        }

        var orderedFields = new List<string>(cardNote.Template.Fields.Count);
        foreach (var fieldName in cardNote.Template.Fields)
        {
            orderedFields.Add(cardNote.FieldValues.TryGetValue(fieldName, out var value) ? value : string.Empty);
        }

        return [.. orderedFields];
    }
}
