namespace MarkdownToAnki.Tests.Parsers;

using MarkdownToAnki.Tests.Fixtures;

public class FlashCardContentExtractorTests
{
    [Fact]
    public void ExtractCodeBlocks_WithSingleCodeBlock_ReturnsBlock()
    {
        // ARRANGE
        var markdown = """
            # Topic
            
            ```BasicTemplate
            Content
            ```
            """;
        var extractor = new FlashCardContentExtractor();

        // ACT
        var results = extractor.ExtractCodeBlocks(markdown).ToList();

        // ASSERT
        results.Should().HaveCount(1);
        results[0].TemplateName.Should().Be("BasicTemplate");
        results[0].Content.Should().Contain("Content");
    }

    [Fact]
    public void ExtractCodeBlocks_WithMultipleBlocks_ReturnsAllBlocks()
    {
        // ARRANGE
        var markdown = """
            ```Template1
            Block 1
            ```
            
            ```Template2
            Block 2
            ```
            """;
        var extractor = new FlashCardContentExtractor();

        // ACT
        var results = extractor.ExtractCodeBlocks(markdown).ToList();

        // ASSERT
        results.Should().HaveCount(2);
        results[0].TemplateName.Should().Be("Template1");
        results[1].TemplateName.Should().Be("Template2");
    }

    [Fact]
    public void ParseCodeBlock_WithSeparator_SplitsFieldsCorrectly()
    {
        // ARRANGE
        var template = TestDataBuilder.CreateBasicTemplate();
        var hierarchy = TestDataBuilder.CreateHierarchy("Topic");
        var normalizer = new TagNormalizer();
        
        var codeBlock = new CodeBlockContent
        {
            TemplateName = "Basic",
            Content = "Question?\n---\nAnswer",
            LineIndex = 0
        };

        var extractor = new FlashCardContentExtractor();

        // ACT
        var result = extractor.ParseCodeBlock(codeBlock, template, "---", normalizer, hierarchy);

        // ASSERT
        result.FieldValues.Should().HaveCount(2);
        result.FieldValues["Question"].Should().Be("Question?");
        result.FieldValues["Answer"].Should().Be("Answer");
        result.Tags.Should().Equal("topic");
    }

    [Fact]
    public void ParseCodeBlock_WithMultilineFields_PreservesContent()
    {
        // ARRANGE
        var template = TestDataBuilder.CreateBasicTemplate();
        var hierarchy = TestDataBuilder.CreateHierarchy("Topic");
        var normalizer = new TagNormalizer();
        
        var codeBlock = new CodeBlockContent
        {
            TemplateName = "Basic",
            Content = "Multi\nline\nquestion\n---\nMulti\nline\nanswer",
            LineIndex = 0
        };

        var extractor = new FlashCardContentExtractor();

        // ACT
        var result = extractor.ParseCodeBlock(codeBlock, template, "---", normalizer, hierarchy);

        // ASSERT
        result.FieldValues["Question"].Should().Contain("<br>");
        result.FieldValues["Answer"].Should().Contain("<br>");
    }

    [Fact]
    public void ExtractCodeBlocks_WithEmptyMarkdown_ReturnsEmpty()
    {
        // ARRANGE
        var markdown = "Just some text without code blocks";
        var extractor = new FlashCardContentExtractor();

        // ACT
        var results = extractor.ExtractCodeBlocks(markdown).ToList();

        // ASSERT
        results.Should().BeEmpty();
    }
}
