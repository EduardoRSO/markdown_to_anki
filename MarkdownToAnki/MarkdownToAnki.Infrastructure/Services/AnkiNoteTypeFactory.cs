using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Converts domain model templates to AnkiNet structures.
/// </summary>
public class AnkiNoteTypeFactory : IAnkiNoteTypeFactory
{
    public AnkiNoteType CreateNoteType(TemplateDefinition template, AnkiNoteTypeModelType modelType)
    {
        return modelType == AnkiNoteTypeModelType.Cloze
            ? CreateClozeNoteType(template)
            : CreateStandardNoteType(template);
    }

    private static AnkiNoteType CreateStandardNoteType(TemplateDefinition template)
    {
        var cardType = new AnkiCardType(
            $"{template.Name} card",
            0,
            template.HtmlQuestionFormat,
            template.HtmlAnswerFormat
        );

        return new AnkiNoteType(
            name: $"{template.Name} template",
            cardTypes: [cardType],
            fieldNames: [.. template.Fields],
            css: template.CssFormat,
            modelType: AnkiNoteTypeModelType.Standard
        );
    }

    private static AnkiNoteType CreateClozeNoteType(TemplateDefinition template)
    {
        var cardType = new AnkiCardType(
            $"{template.Name} cloze card",
            0,
            "{{cloze:Text}}",
            "{{cloze:Text}}<br>{{Back Extra}}"
        );

        return new AnkiNoteType(
            name: $"{template.Name} cloze template",
            cardTypes: [cardType],
            fieldNames: ["Text", "Back Extra"],
            css: template.CssFormat,
            modelType: AnkiNoteTypeModelType.Cloze
        );
    }
}
