using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public class ApkgToMarkdownService : IApkgToMarkdownService
{
    private const string DefaultSeparator = "---";
    private const string NoteTypeSuffix = " template";
    private const string CardTypeSuffix = " card";

    private readonly IAnkiPackageReader _reader;
    private readonly IMarkdownDeckWriter _writer;

    public ApkgToMarkdownService(IAnkiPackageReader reader, IMarkdownDeckWriter writer)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public async Task GenerateMarkdownAsync(string inputPath, string outputPath)
    {
        var collection = await _reader.ReadAsync(inputPath);
        var deckDefinition = BuildDeckDefinition(collection);
        var notes = BuildFlashCardNotes(collection, deckDefinition);

        await _writer.WriteAsync(outputPath, deckDefinition, notes);
    }

    private static DeckDefinition BuildDeckDefinition(AnkiCollection collection)
    {
        var templates = new List<TemplateDefinition>();
        var templateMapById = new Dictionary<long, TemplateDefinition>();

        foreach (var noteType in collection.NoteTypes.OrderBy(noteType => noteType.Name, StringComparer.Ordinal))
        {
            var template = MapTemplate(noteType);
            templates.Add(template);
            templateMapById[noteType.Id] = template;
        }

        return new DeckDefinition
        {
            DeckName = DetermineDeckName(collection),
            Source = string.Empty,
            Separator = DefaultSeparator,
            MediaRoot = "./media",
            Templates = templates
        };
    }

    private static List<FlashCardNote> BuildFlashCardNotes(AnkiCollection collection, DeckDefinition deckDefinition)
    {
        var templateMap = deckDefinition.Templates.ToDictionary(t => t.Name, StringComparer.Ordinal);
        var templateByNoteTypeId = collection.NoteTypes.ToDictionary(
            noteType => noteType.Id,
            noteType => templateMap[MapTemplateName(noteType)],
            EqualityComparer<long>.Default);

        var notesById = new Dictionary<long, FlashCardNote>();

        foreach (var deck in collection.Decks)
        {
            foreach (var card in deck.Cards)
            {
                var note = card.Note;
                if (notesById.ContainsKey(note.Id))
                {
                    continue;
                }

                if (!templateByNoteTypeId.TryGetValue(note.NoteTypeId, out var template))
                {
                    continue;
                }

                var flashCard = new FlashCardNote
                {
                    Template = template,
                    FieldValues = BuildFieldValues(template, note.Fields),
                    Tags = ParseTags(note.Tags)
                };

                notesById[note.Id] = flashCard;
            }
        }

        return notesById.Values.ToList();
    }

    private static Dictionary<string, string> BuildFieldValues(TemplateDefinition template, string[] fields)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        for (int i = 0; i < template.Fields.Count; i++)
        {
            if (i >= fields.Length)
            {
                break;
            }

            values[template.Fields[i]] = fields[i];
        }

        return values;
    }

    private static TemplateDefinition MapTemplate(AnkiNoteType noteType)
    {
        string templateName = MapTemplateName(noteType);
        var cardType = SelectCardType(noteType, templateName);

        return new TemplateDefinition
        {
            Name = templateName,
            ModelType = noteType.ModelType == AnkiNoteTypeModelType.Cloze ? TemplateModelType.Cloze : TemplateModelType.Standard,
            Fields = noteType.FieldNames.ToList(),
            Usage = string.Empty,
            HtmlQuestionFormat = cardType.QuestionFormat,
            HtmlAnswerFormat = cardType.AnswerFormat,
            CssFormat = noteType.Css ?? string.Empty
        };
    }

    private static string MapTemplateName(AnkiNoteType noteType)
    {
        if (noteType.Name.EndsWith(NoteTypeSuffix, StringComparison.Ordinal))
        {
            var candidate = noteType.Name[..^NoteTypeSuffix.Length];
            if (noteType.CardTypes.Any(cardType => cardType.Name == candidate + CardTypeSuffix))
            {
                return candidate;
            }
        }

        return noteType.Name;
    }

    private static AnkiCardType SelectCardType(AnkiNoteType noteType, string templateName)
    {
        var match = noteType.CardTypes.FirstOrDefault(cardType => cardType.Name == templateName + CardTypeSuffix);
        return match.Name == null ? noteType.CardTypes[0] : match;
    }

    private static List<string> ParseTags(string tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return new List<string>();
        }

        return tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static string DetermineDeckName(AnkiCollection collection)
    {
        var deckNames = collection.Decks
            .Select(deck => deck.Name)
            .Where(name => !string.Equals(name, "Default", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (deckNames.Count == 0)
        {
            return "Default";
        }

        var roots = new List<string>();
        foreach (var name in deckNames)
        {
            var root = name.Split("::", StringSplitOptions.None)[0];
            if (!roots.Contains(root, StringComparer.Ordinal))
            {
                roots.Add(root);
            }
        }

        return roots[0];
    }
}
