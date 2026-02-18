namespace MarkdownToAnki.Tests.Parsers;

using MarkdownToAnki.Tests.Fixtures;

public class MarkdownHeaderHierarchyExtractorTests
{
    [Fact]
    public void ExtractHierarchyAtLine_WithSingleHeader_ReturnsHierarchy()
    {
        // ARRANGE
        var markdown = """
            # Topic
            
            Some content
            """;
        var extractor = new MarkdownHeaderHierarchyExtractor();

        // ACT
        var result = extractor.ExtractHierarchyAtLine(markdown, 0);

        // ASSERT
        result.GetHierarchyPath().Should().ContainSingle().Which.Should().Be("Topic");
    }

    [Fact]
    public void ExtractHierarchyAtLine_WithNestedHeaders_ReturnsFullPath()
    {
        // ARRANGE
        var markdown = """
            # Level1
            ## Level2
            ### Level3
            
            Content
            """;
        var extractor = new MarkdownHeaderHierarchyExtractor();

        // ACT
        var result = extractor.ExtractHierarchyAtLine(markdown, 2);

        // ASSERT
        result.GetHierarchyPath().Should().Equal("Level1", "Level2", "Level3");
    }

    [Fact]
    public void ExtractHierarchyAtLine_WithHeaderRemovedByDeeper_UpdatesHierarchy()
    {
        // ARRANGE
        var markdown = """
            # Header1
            ## Header2
            # Header3
            
            Content
            """;
        var extractor = new MarkdownHeaderHierarchyExtractor();

        // ACT - Extract at Header3 line
        var result = extractor.ExtractHierarchyAtLine(markdown, 2);

        // ASSERT - Header2 should be removed when Header1 level appears again
        result.GetHierarchyPath().Should().Equal("Header3");
    }

    [Fact]
    public void ExtractHierarchyAtLine_BeforeAnyHeader_ReturnsEmpty()
    {
        // ARRANGE
        var markdown = """
            Some content without header
            
            # Header
            """;
        var extractor = new MarkdownHeaderHierarchyExtractor();

        // ACT
        var result = extractor.ExtractHierarchyAtLine(markdown, 0);

        // ASSERT
        result.GetHierarchyPath().Should().BeEmpty();
    }

    [Fact]
    public void ExtractHeaderHierarchy_ReturnsAllHeaderPoints()
    {
        // ARRANGE
        var markdown = """
            # H1
            ## H2
            content
            # H1b
            ## H2b
            """;
        var extractor = new MarkdownHeaderHierarchyExtractor();

        // ACT
        var results = extractor.ExtractHeaderHierarchy(markdown).ToList();

        // ASSERT
        results.Should().HaveCount(4);
        results[0].hierarchy.GetHierarchyPath().Should().Equal("H1");
        results[1].hierarchy.GetHierarchyPath().Should().Equal("H1", "H2");
        results[2].hierarchy.GetHierarchyPath().Should().Equal("H1b");
        results[3].hierarchy.GetHierarchyPath().Should().Equal("H1b", "H2b");
    }
}
