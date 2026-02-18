using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Main Anki deck generation service.
/// Orchestrates note type factory, deck hierarchy builder, and card generator.
/// Creates nested deck structure based on markdown header hierarchy.
/// </summary>
public class AnkiGeneratorService : IAnkiGeneratorService
{
    private readonly IAnkiNoteTypeFactory _noteTypeFactory;
    private readonly IAnkiCardGenerator _cardGenerator;

    public AnkiGeneratorService(
        IAnkiNoteTypeFactory noteTypeFactory,
        IAnkiCardGenerator cardGenerator)
    {
        _noteTypeFactory = noteTypeFactory ?? throw new ArgumentNullException(nameof(noteTypeFactory));
        _cardGenerator = cardGenerator ?? throw new ArgumentNullException(nameof(cardGenerator));
    }

    async Task IAnkiGeneratorService.GenerateApkg(IMarkdownParserService markdownParser, string inputPath, string outputPath)
    {
        var (deckDefinition, flashCardNotes) = await markdownParser.ParseFileAsync(inputPath);

        // Create AnkiNet structures from templates
        var ankiCollection = new AnkiCollection();
        var noteTypeMap = new Dictionary<string, long>();

        foreach (var template in deckDefinition.Templates)
        {
            var ankiNoteType = _noteTypeFactory.CreateNoteType(template);
            var noteTypeId = ankiCollection.CreateNoteType(ankiNoteType);
            noteTypeMap[template.Name] = noteTypeId;
        }

        // Create root deck
        var rootDeckId = ankiCollection.CreateDeck(deckDefinition.DeckName);

        // Initialize deck hierarchy builder for this collection
        var hierarchyBuilder = new DeckHierarchyBuilder(ankiCollection);

        // Process each flashcard note
        foreach (var cardNote in flashCardNotes)
        {
            // Build hierarchy from tags (tags represent the header hierarchy)
            var headerHierarchy = RebuildHierarchyFromTags(cardNote.Tags);

            // Get or create the nested deck for this hierarchy
            long targetDeckId = hierarchyBuilder.GetOrCreateDeckForHierarchy(rootDeckId, headerHierarchy);

            // Create the card in the target deck
            long noteTypeId = noteTypeMap[cardNote.Template.Name];
            _cardGenerator.CreateCardInDeck(ankiCollection, targetDeckId, noteTypeId, cardNote);
        }

        // Write the collection to file
        string outputDir = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
        string fileName = Path.GetFileNameWithoutExtension(outputPath);
        string fullOutputPath = Path.Combine(outputDir, $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}.apkg");

        await AnkiFileWriter.WriteToFileAsync(fullOutputPath, ankiCollection);
    }

    private HeaderHierarchy RebuildHierarchyFromTags(List<string> tags)
    {
        var hierarchy = new HeaderHierarchy();

        // Tags are normalized versions of headers, representing the hierarchy chain
        // Each tag corresponds to a header level (H1, H2, H3, etc.)
        for (int i = 0; i < tags.Count; i++)
        {
            hierarchy.UpdateHeader(i + 1, tags[i]);
        }

        return hierarchy;
    }
}
