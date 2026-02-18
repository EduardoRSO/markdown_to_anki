namespace MarkdownToAnki.Domain.Models;

public class FlashCardNote
{
    public TemplateDefinition Template { get; set; }
    public Dictionary<string, string> FieldValues { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
