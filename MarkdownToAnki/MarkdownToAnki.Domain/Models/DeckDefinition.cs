namespace MarkdownToAnki.Domain.Models;

public class DeckDefinition
{
    public string DeckName { get; set; }
    public string Source { get; set; }
    public string Separator { get; set; }
    public List<TemplateDefinition> Templates { get; set; } = new();
}
