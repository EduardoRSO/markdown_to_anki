namespace MarkdownToAnki.Tests.Parsers;

using MarkdownToAnki.Tests.Fixtures;

public class MarkdownParserServiceIntegrationTests
{
    [Fact]
    public async Task ParseFileAsync_WithSimpleDeckTestDataFile_ReturnsExpectedCards()
    {
        // ARRANGE
        var parser = CreateParser();
        var testFilePath = GetTestDataPath("simple_deck.md");

        // ACT
        var (deckDef, cards) = await parser.ParseFileAsync(testFilePath);

        // ASSERT
        deckDef.DeckName.Should().Be("Simple Test Deck");
        deckDef.Templates.Should().HaveCount(1);
        cards.Should().HaveCount(2);
        cards.ForEach(c => c.Tags.Should().Contain("mathematics"));
        cards.ForEach(c => c.Tags.Should().Contain("arithmetic"));
    }

    [Fact]
    public async Task ParseFileAsync_WithComplexHierarchyTestDataFile_ParsesTemplates()
    {
        // ARRANGE
        var parser = CreateParser();
        var testFilePath = GetTestDataPath("complex_hierarchy.md");

        // ACT
        var (deckDef, cards) = await parser.ParseFileAsync(testFilePath);

        // ASSERT
        deckDef.DeckName.Should().Be("Complex Hierarchy Deck");
        deckDef.Templates.Should().HaveCount(2);
        cards.Should().HaveCount(4);
    }

    [Fact]
    public async Task ParseFileAsync_WithEdgeCasesTestDataFile_ParsesCustomSeparator()
    {
        // ARRANGE
        var parser = CreateParser();
        var testFilePath = GetTestDataPath("edge_cases.md");

        // ACT
        var (deckDef, cards) = await parser.ParseFileAsync(testFilePath);

        // ASSERT
        deckDef.DeckName.Should().Be("Edge Cases Test");
        deckDef.Separator.Should().Be("||");
        cards.Should().HaveCount(3);
    }

    [Fact]
    public async Task ParseFileAsync_WithCompleteMarkdownFile_ReturnsValidDeckAndCards()
    {
        // ARRANGE
        var yamlContent = MarkdownFixtures.SimpleDeckYaml;
        var markdownBody = MarkdownFixtures.NestedHeaderMarkdown;
        var fullContent = $"---\n{yamlContent}\n---\n{markdownBody}";
        
        var configParser = new DeckConfigParser();
        var headerExtractor = new MarkdownHeaderHierarchyExtractor();
        var contentExtractor = new FlashCardContentExtractor();
        var normalizer = new TagNormalizer();
        
        var parser = new MarkdownParserService(configParser, headerExtractor, contentExtractor, normalizer);

        // Create a temporary file
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.md");
        await File.WriteAllTextAsync(tempPath, fullContent);

        try
        {
            // ACT
            var (deckDef, cards) = await parser.ParseFileAsync(tempPath);

            // ASSERT
            deckDef.DeckName.Should().Be("Simple Test Deck");
            deckDef.Templates.Should().HaveCount(1);
            cards.Should().NotBeEmpty();
            cards[0].Tags.Should().NotBeEmpty();
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ParseFileAsync_WithMultipleCards_PreservesHierarchy()
    {
        // ARRANGE
        var yamlContent = MarkdownFixtures.SimpleDeckYaml;
        var markdownBody = """
            # Math
            ## Arithmetic
            
            ```Basic
            2+2=?
            ---
            4
            ```
            
            ```Basic
            3+3=?
            ---
            6
            ```
            """;
        var fullContent = $"---\n{yamlContent}\n---\n{markdownBody}";

        var configParser = new DeckConfigParser();
        var headerExtractor = new MarkdownHeaderHierarchyExtractor();
        var contentExtractor = new FlashCardContentExtractor();
        var normalizer = new TagNormalizer();
        
        var parser = new MarkdownParserService(configParser, headerExtractor, contentExtractor, normalizer);

        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.md");
        await File.WriteAllTextAsync(tempPath, fullContent);

        try
        {
            // ACT
            var (deckDef, cards) = await parser.ParseFileAsync(tempPath);

            // ASSERT
            cards.Should().HaveCount(2);
            cards.ForEach(c => c.Tags.Should().Contain("math"));
            cards.ForEach(c => c.Tags.Should().Contain("arithmetic"));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ParseFileAsync_WithoutYamlDelimiter_ThrowsException()
    {
        // ARRANGE
        var invalidContent = """
            This is invalid markdown without YAML
            # Header
            Some content
            """;

        var configParser = new DeckConfigParser();
        var headerExtractor = new MarkdownHeaderHierarchyExtractor();
        var contentExtractor = new FlashCardContentExtractor();
        var normalizer = new TagNormalizer();
        
        var parser = new MarkdownParserService(configParser, headerExtractor, contentExtractor, normalizer);

        var tempPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.md");
        await File.WriteAllTextAsync(tempPath, invalidContent);

        try
        {
            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => parser.ParseFileAsync(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static string GetTestDataPath(string fileName)
    {
        var testDataDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestData"));

        return Path.Combine(testDataDir, fileName);
    }

    private static MarkdownParserService CreateParser()
    {
        return new MarkdownParserService(
            new DeckConfigParser(),
            new MarkdownHeaderHierarchyExtractor(),
            new FlashCardContentExtractor(),
            new TagNormalizer());
    }
}
