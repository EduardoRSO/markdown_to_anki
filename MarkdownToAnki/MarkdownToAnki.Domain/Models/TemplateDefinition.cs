namespace MarkdownToAnki.Domain.Models;

public class TemplateDefinition
{
    public string Name { get; set; }
    public List<string> Fields { get; set; } = new();
    public string HtmlQuestionFormat { get; set; }
    public string HtmlAnswerFormat { get; set; }
    public string CssFormat { get; set; }
    public string Usage { get; set; }
}
