namespace MarkdownToAnki.Domain.Models;

public class DeckDefinition
{
    public string DeckName {get; set;}
    public string Source {get; set;}
    public string Separator {get; set;}
}

public class TemplateDefinition
{
    public string Name {get; set;}
    public List<string> Fields {get; set;} = new();
    public string HtmlFormat {get; set;}
    public string Usage {get; set;}
}

public class FlashCardNote{
    public TemplateDefinition Template {get; set;}
    public Dictionary<string,string> FieldValues {get; set;} = new();
    public List<string> Tags {get;set;} = new();
}
