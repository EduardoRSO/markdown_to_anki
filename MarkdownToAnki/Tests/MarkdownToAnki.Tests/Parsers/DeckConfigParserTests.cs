namespace MarkdownToAnki.Tests.Parsers;

using MarkdownToAnki.Tests.Fixtures;

public class DeckConfigParserTests
{
    [Fact]
    public void ParseDeckConfig_WithValidYaml_ReturnsCorrectDeckDefinition()
    {
        // ARRANGE
        var yamlContent = MarkdownFixtures.SimpleDeckYaml;
        var parser = new DeckConfigParser();

        // ACT
        var result = parser.ParseDeckConfig(yamlContent);

        // ASSERT
        result.DeckName.Should().Be("Simple Test Deck");
        result.Source.Should().Be("Test Source");
        result.Separator.Should().Be("---");
        result.Templates.Should().HaveCount(1);
        result.Templates[0].Name.Should().Be("Basic");
        result.Templates[0].ModelType.Should().Be(TemplateModelType.Standard);
    }

    [Fact]
    public void ParseDeckConfig_WithMultipleTemplates_ParsesAllTemplates()
    {
        // ARRANGE
        var yamlContent = MarkdownFixtures.MultiTemplateDeckYaml;
        var parser = new DeckConfigParser();

        // ACT
        var result = parser.ParseDeckConfig(yamlContent);

        // ASSERT
        result.Templates.Should().HaveCount(2);
        result.Templates[0].Name.Should().Be("Concept");
        result.Templates[0].ModelType.Should().Be(TemplateModelType.Standard);
        result.Templates[0].Fields.Should().ContainInOrder("Term", "Definition");
        result.Templates[1].Name.Should().Be("Question");
        result.Templates[1].ModelType.Should().Be(TemplateModelType.Standard);
        result.Templates[1].Fields.Should().HaveCount(3);
    }

    [Fact]
    public void ParseDeckConfig_WithCustomSeparator_ParsesCorrectly()
    {
        // ARRANGE
                var yamlContent =
                    "deck_name: \"Custom Sep\"\n" +
                    "source: \"Test\"\n" +
                    "separator: \"||\"\n" +
                    "templates:\n" +
                    "  - name: \"Format\"\n" +
                    "    anki_model_type: \"standard\"\n" +
                    "    fields: [A, B]\n" +
                    "    html_question_format: \"{{A}}\"\n" +
                    "    html_answer_format: \"{{B}}\"\n" +
                    "    css_format: \"\"";
        var parser = new DeckConfigParser();

        // ACT
        var result = parser.ParseDeckConfig(yamlContent);

        // ASSERT
        result.Separator.Should().Be("||");
    }

    [Fact]
    public void ParseDeckConfig_WithEmptyYaml_ThrowsException()
    {
        // ARRANGE
        var emptyYaml = "";
        var parser = new DeckConfigParser();

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => parser.ParseDeckConfig(emptyYaml));
    }

    [Fact]
    public void ParseDeckConfig_PreservesFieldOrder()
    {
        // ARRANGE
        var yamlContent = MarkdownFixtures.MultiTemplateDeckYaml;
        var parser = new DeckConfigParser();

        // ACT
        var result = parser.ParseDeckConfig(yamlContent);

        // ASSERT
        result.Templates[1].Fields.Should().Equal("Question", "Answer", "Explanation");
    }

    [Fact]
    public void ParseDeckConfig_WithoutAnkiModelType_ThrowsException()
    {
        // ARRANGE
        var yamlContent = """
            deck_name: "No Model"
            source: ""
            separator: "---"
            templates:
              - name: "Basic"
                fields: [Question, Answer]
                html_question_format: "{{Question}}"
                html_answer_format: "{{Answer}}"
                css_format: ""
            """;
        var parser = new DeckConfigParser();

        // ACT
        var act = () => parser.ParseDeckConfig(yamlContent);

        // ASSERT
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*anki_model_type*");
    }
}
