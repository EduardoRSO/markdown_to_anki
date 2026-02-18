using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Converts domain model templates to AnkiNet structures.
/// </summary>
public class AnkiNoteTypeFactory : IAnkiNoteTypeFactory
{
    public AnkiNoteType CreateNoteType(TemplateDefinition template)
    {
        var cardType = new AnkiCardType(
            $"{template.Name} card",
            0,
            template.HtmlQuestionFormat,
            template.HtmlAnswerFormat
        );
        
        var noteType = new AnkiNoteType(
            $"{template.Name} template",
            [cardType],
            [.. template.Fields],
            template.CssFormat
        );
        
        return noteType;
    }
}
