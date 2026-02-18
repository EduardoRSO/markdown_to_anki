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
        result.Templates[0].Fields.Should().ContainInOrder("Term", "Definition");
        result.Templates[1].Name.Should().Be("Question");
        result.Templates[1].Fields.Should().HaveCount(3);
    }

    [Fact]
    public void ParseDeckConfig_WithCustomSeparator_ParsesCorrectly()
    {
        // ARRANGE
        var yamlContent = """
            deck_name: "Custom Sep"
            source: "Test"
            separator: "||"
            templates:
              - name: "Format"
                fields: [A, B]
                html_question_format: "{{A}}"
                html_answer_format: "{{B}}"
                css_format: ""
            """;
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
}
