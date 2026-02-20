namespace MarkdownToAnki.Domain.Models;

public class TemplateDefinition
{
    public string Name { get; set; }
    public TemplateModelType ModelType { get; set; } = TemplateModelType.Standard;
    public List<TemplateMediaFileDefinition> MediaFiles { get; set; } = new();
    public List<string> Fields { get; set; } = new();
    public string HtmlQuestionFormat { get; set; }
    public string HtmlAnswerFormat { get; set; }
    public string CssFormat { get; set; }
    public string Usage { get; set; }
}

public class TemplateMediaFileDefinition
{
    public string Source { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public enum TemplateModelType
{
    Standard,
    Cloze
}
