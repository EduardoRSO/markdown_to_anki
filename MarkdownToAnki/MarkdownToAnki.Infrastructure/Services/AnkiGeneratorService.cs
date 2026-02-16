using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public class AnkiGeneratorService : IAnkiGeneratorService
{
    async Task IAnkiGeneratorService.GenerateApkg(IMarkdownParserService markdownParser, string inputPath, string outputPath)
    {
        var (deckDefinition, flashCardNotes) = await markdownParser.ParseFileAsync(inputPath);

        List<AnkiNoteType> ankiNotes = new();
        for (int i = 0; i < deckDefinition.Templates.Count; i++)
        {
            var currentTemplate = deckDefinition.Templates[i];
            var cardType = new AnkiCardType(
                $"{currentTemplate.Name} card",
                0,
                currentTemplate.HtmlQuestionFormat,
                currentTemplate.HtmlAnswerFormat
            );
            var noteType = new AnkiNoteType(
                $"{currentTemplate.Name} template",
                [cardType],
                [.. currentTemplate.Fields],
                currentTemplate.CssFormat
            );
            ankiNotes.Add(noteType);
        }
        var ankiCollection = new AnkiCollection();
        var deckId = ankiCollection.CreateDeck(deckDefinition.DeckName);
        
        // Create a map from template name to note type ID
        var noteIdMap = new Dictionary<string, long>();
        for (int i = 0; i < deckDefinition.Templates.Count; i++)
        {
            var template = deckDefinition.Templates[i];
            var ankiNote = ankiNotes[i];
            var noteTypeId = ankiCollection.CreateNoteType(ankiNote);
            noteIdMap[template.Name] = noteTypeId;
        }
        
        for (int c = 0; c < flashCardNotes.Count; c++)
        {
            var currentCard = flashCardNotes[c];
            ankiCollection.CreateNote(
                deckId,
                noteIdMap[currentCard.Template.Name],
                currentCard.FieldValues.Values.ToArray()
            );
        }
        await AnkiFileWriter.WriteToFileAsync($"{deckDefinition.DeckName}_{DateTime.Now:yyyyMMddHHmmss}.apkg", ankiCollection);
    }
}