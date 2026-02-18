namespace MarkdownToAnki.Tests.Fixtures;

/// <summary>
/// Helper class to build domain objects for testing.
/// </summary>
public static class TestDataBuilder
{
    public static TemplateDefinition CreateBasicTemplate(
        string name = "Basic",
        string questionFormat = "<div>{{Question}}</div>",
        string answerFormat = "<div>{{Answer}}</div>")
    {
        return new TemplateDefinition
        {
            Name = name,
            Fields = new List<string> { "Question", "Answer" },
            HtmlQuestionFormat = questionFormat,
            HtmlAnswerFormat = answerFormat,
            CssFormat = ".card { font-family: arial; }",
            Usage = "Standard question-answer format"
        };
    }

    public static TemplateDefinition CreateConceptTemplate(
        string name = "Concept",
        string[] fields = null)
    {
        fields ??= ["Term", "Definition", "Example"];
        return new TemplateDefinition
        {
            Name = name,
            Fields = new List<string>(fields),
            HtmlQuestionFormat = "<b>{{Term}}</b>",
            HtmlAnswerFormat = "{{Definition}}<br><i>{{Example}}</i>",
            CssFormat = ".card { }",
            Usage = "Concept with term and definition"
        };
    }

    public static DeckDefinition CreateSimpleDeck(
        string name = "Test Deck",
        string separator = "---",
        TemplateDefinition template = null)
    {
        template ??= CreateBasicTemplate();
        
        return new DeckDefinition
        {
            DeckName = name,
            Source = "Test Source",
            Separator = separator,
            Templates = new List<TemplateDefinition> { template }
        };
    }

    public static DeckDefinition CreateMultiTemplateDeck(
        string name = "Multi Template Deck",
        params TemplateDefinition[] templates)
    {
        templates = templates.Length == 0 
            ? [CreateBasicTemplate(), CreateConceptTemplate()] 
            : templates;

        return new DeckDefinition
        {
            DeckName = name,
            Source = "Test Source",
            Separator = "---",
            Templates = new List<TemplateDefinition>(templates)
        };
    }

    public static FlashCardNote CreateFlashCard(
        TemplateDefinition template = null,
        Dictionary<string, string> fieldValues = null,
        List<string> tags = null)
    {
        template ??= CreateBasicTemplate();
        
        if (fieldValues == null)
        {
            fieldValues = new Dictionary<string, string>
            {
                { "Question", "Sample question?" },
                { "Answer", "Sample answer" }
            };
        }

        tags ??= new List<string> { "test", "sample" };

        return new FlashCardNote
        {
            Template = template,
            FieldValues = fieldValues,
            Tags = tags
        };
    }

    public static HeaderHierarchy CreateHierarchy(params string[] headers)
    {
        var hierarchy = new HeaderHierarchy();
        for (int i = 0; i < headers.Length; i++)
        {
            hierarchy.UpdateHeader(i + 1, headers[i]);
        }
        return hierarchy;
    }
}
