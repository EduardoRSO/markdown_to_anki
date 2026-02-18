using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IAnkiNoteTypeFactory
{
    AnkiNoteType CreateNoteType(TemplateDefinition template);
}
