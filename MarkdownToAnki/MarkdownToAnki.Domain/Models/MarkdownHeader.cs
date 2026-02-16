namespace MarkdownToAnki.Domain.Models;

public class TemplateDefinition
{
    public string Name {get; set;}
    public List<string> Fields {get; set;} = new();
    public string HtmlQuestionFormat {get; set;}
    public string HtmlAnswerFormat {get; set;}
    public string CssFormat {get; set;}

    public string Usage {get; set;}
}

public class DeckDefinition
{
    public string DeckName {get; set;}
    public string Source {get; set;}
    public string Separator {get; set;}
    public List<TemplateDefinition> Templates {get; set;} = new();
}


public class FlashCardNote{
    public TemplateDefinition Template {get; set;}
    public Dictionary<string,string> FieldValues {get; set;} = new();
    public List<string> Tags {get;set;} = new();
}
