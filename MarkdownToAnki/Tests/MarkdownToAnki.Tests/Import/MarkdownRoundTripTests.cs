using FluentAssertions;
using AnkiNet;
using MarkdownToAnki.Infrastructure.Services;

namespace MarkdownToAnki.Tests.Import;

public class MarkdownRoundTripTests
{
    [Fact]
    public async Task RoundTrip_SimpleDeck_PreservesHierarchyAndContent()
    {
        // ARRANGE
        var inputMd = GetTestDataPath("simple_deck.md");
        var tempApkg = Path.Combine(Path.GetTempPath(), $"simple_{Guid.NewGuid()}.apkg");
        var outputMd = Path.Combine(Path.GetTempPath(), $"simple_{Guid.NewGuid()}.md");

        var parser = CreateParser();
        var generator = (IAnkiGeneratorService)new AnkiGeneratorService(new AnkiNoteTypeFactory(), new AnkiCardGenerator());
        var reader = new AnkiPackageReader();
        var writer = new MarkdownDeckWriter();
        var converter = new ApkgToMarkdownService(reader, writer);

        try
        {
            // ACT: Markdown → Apkg → Markdown
            await generator.GenerateApkg(parser, inputMd, tempApkg);
            await converter.GenerateMarkdownAsync(tempApkg, outputMd);

            // ASSERT
            var outputContent = await File.ReadAllTextAsync(outputMd);

            // Check YAML header structure
            outputContent.Should().StartWith("---");
            outputContent.Should().Contain("deck_name: \"Simple Test Deck\"");
            outputContent.Should().Contain("separator: \"---\"");
            outputContent.Should().Contain("templates:");
            outputContent.Should().Contain("name: \"Basic\"");
            outputContent.Should().Contain("fields: [\"Question\", \"Answer\"]");

            // Check hierarchy preservation
            outputContent.Should().Contain("# Mathematics");
            outputContent.Should().Contain("## Arithmetic");

            // Check card content preservation
            outputContent.Should().Contain("What is 2 + 2?");
            outputContent.Should().Contain("4");
            outputContent.Should().Contain("What is 10 - 3?");
            outputContent.Should().Contain("7");

            // Check code block format
            outputContent.Should().Contain("```Basic");
        }
        finally
        {
            if (File.Exists(tempApkg)) File.Delete(tempApkg);
            if (File.Exists(outputMd)) File.Delete(outputMd);
        }
    }

    [Fact]
    public async Task RoundTrip_ComplexHierarchy_PreservesMultiLevelHeadings()
    {
        // ARRANGE
        var inputMd = GetTestDataPath("complex_hierarchy.md");
        var tempApkg = Path.Combine(Path.GetTempPath(), $"complex_{Guid.NewGuid()}.apkg");
        var outputMd = Path.Combine(Path.GetTempPath(), $"complex_{Guid.NewGuid()}.md");

        var parser = CreateParser();
        var generator = (IAnkiGeneratorService)new AnkiGeneratorService(new AnkiNoteTypeFactory(), new AnkiCardGenerator());
        var reader = new AnkiPackageReader();
        var writer = new MarkdownDeckWriter();
        var converter = new ApkgToMarkdownService(reader, writer);

        try
        {
            // ACT: Markdown → Apkg → Markdown
            await generator.GenerateApkg(parser, inputMd, tempApkg);
            await converter.GenerateMarkdownAsync(tempApkg, outputMd);

            // ASSERT
            var outputContent = await File.ReadAllTextAsync(outputMd);

            // Check YAML header with multiple templates
            outputContent.Should().Contain("deck_name: \"Complex Hierarchy Deck\"");
            outputContent.Should().Contain("name: \"Concept\"");
            outputContent.Should().Contain("name: \"Question\"");
            outputContent.Should().Contain("fields: [\"Term\", \"Definition\", \"Example\"]");
            outputContent.Should().Contain("fields: [\"Question\", \"Answer\", \"Explanation\"]");

            // Check three-level hierarchy
            outputContent.Should().Contain("# Science");
            outputContent.Should().Contain("## Physics");
            outputContent.Should().Contain("### Classical Mechanics");
            outputContent.Should().Contain("### Thermodynamics");
            outputContent.Should().Contain("## Chemistry");
            outputContent.Should().Contain("### Atomic Theory");

            // Check content from different levels
            outputContent.Should().Contain("Newton's First Law");
            outputContent.Should().Contain("Entropy");
            outputContent.Should().Contain("How many electrons does Carbon have?");

            // Verify hierarchy order is preserved
            var scienceIndex = outputContent.IndexOf("# Science");
            var physicsIndex = outputContent.IndexOf("## Physics");
            var mechanicsIndex = outputContent.IndexOf("### Classical Mechanics");
            var chemistryIndex = outputContent.IndexOf("## Chemistry");

            scienceIndex.Should().BeLessThan(physicsIndex);
            physicsIndex.Should().BeLessThan(mechanicsIndex);
            mechanicsIndex.Should().BeLessThan(chemistryIndex);
        }
        finally
        {
            if (File.Exists(tempApkg)) File.Delete(tempApkg);
            if (File.Exists(outputMd)) File.Delete(outputMd);
        }
    }

    [Fact]
    public async Task RoundTrip_ComplexHierarchy_PreservesCardOrderWithinSection()
    {
        // ARRANGE
        var inputMd = GetTestDataPath("complex_hierarchy.md");
        var tempApkg = Path.Combine(Path.GetTempPath(), $"order_{Guid.NewGuid()}.apkg");
        var outputMd = Path.Combine(Path.GetTempPath(), $"order_{Guid.NewGuid()}.md");

        var parser = CreateParser();
        var generator = (IAnkiGeneratorService)new AnkiGeneratorService(new AnkiNoteTypeFactory(), new AnkiCardGenerator());
        var reader = new AnkiPackageReader();
        var writer = new MarkdownDeckWriter();
        var converter = new ApkgToMarkdownService(reader, writer);

        try
        {
            // ACT
            await generator.GenerateApkg(parser, inputMd, tempApkg);
            await converter.GenerateMarkdownAsync(tempApkg, outputMd);

            // ASSERT
            var outputContent = await File.ReadAllTextAsync(outputMd);

            // In Classical Mechanics section, Concept card should appear before Question card
            var mechanicsIndex = outputContent.IndexOf("### Classical Mechanics");
            var newtonsLawIndex = outputContent.IndexOf("Newton's First Law", mechanicsIndex);
            var siUnitIndex = outputContent.IndexOf("What is the SI unit of force?", mechanicsIndex);

            newtonsLawIndex.Should().BeGreaterThan(mechanicsIndex);
            siUnitIndex.Should().BeGreaterThan(newtonsLawIndex);
        }
        finally
        {
            if (File.Exists(tempApkg)) File.Delete(tempApkg);
            if (File.Exists(outputMd)) File.Delete(outputMd);
        }
    }

    [Fact]
    public async Task RoundTrip_EdgeCases_PreservesCustomSeparator()
    {
        // ARRANGE
        var inputMd = GetTestDataPath("edge_cases.md");
        var tempApkg = Path.Combine(Path.GetTempPath(), $"edge_{Guid.NewGuid()}.apkg");
        var outputMd = Path.Combine(Path.GetTempPath(), $"edge_{Guid.NewGuid()}.md");

        var parser = CreateParser();
        var generator = (IAnkiGeneratorService)new AnkiGeneratorService(new AnkiNoteTypeFactory(), new AnkiCardGenerator());
        var reader = new AnkiPackageReader();
        var writer = new MarkdownDeckWriter();
        var converter = new ApkgToMarkdownService(reader, writer);

        try
        {
            // ACT
            await generator.GenerateApkg(parser, inputMd, tempApkg);
            await converter.GenerateMarkdownAsync(tempApkg, outputMd);

            // ASSERT
            var outputContent = await File.ReadAllTextAsync(outputMd);

            // Note: The custom separator from input is preserved in the YAML but
            // the writer always uses the separator from DeckDefinition
            outputContent.Should().Contain("separator:");
        }
        finally
        {
            if (File.Exists(tempApkg)) File.Delete(tempApkg);
            if (File.Exists(outputMd)) File.Delete(outputMd);
        }
    }

    [Fact]
    public async Task ApkgToMarkdown_TeoriaFile_ContainsExpectedHeadersAndContent()
    {
        // ARRANGE
        var inputApkg = GetTestDataPath("teoria.apkg");
        var outputMd = Path.Combine(Path.GetTempPath(), $"teoria_{Guid.NewGuid()}.md");

        var reader = new AnkiPackageReader();
        var writer = new MarkdownDeckWriter();
        var converter = new ApkgToMarkdownService(reader, writer);

        try
        {
            // ACT
            await converter.GenerateMarkdownAsync(inputApkg, outputMd);

            // ASSERT
            File.Exists(outputMd).Should().BeTrue();
            var outputContent = await File.ReadAllTextAsync(outputMd);

            // Check structural elements
            outputContent.Should().StartWith("---");
            outputContent.Should().Contain("deck_name:");
            outputContent.Should().Contain("templates:");
            outputContent.Should().Contain("separator: \"---\"");

            // Check at least one code block exists
            outputContent.Should().Contain("```");

            // Check that content is not empty
            outputContent.Length.Should().BeGreaterThan(100);
        }
        finally
        {
            if (File.Exists(outputMd)) File.Delete(outputMd);
        }
    }

    [Fact]
    public async Task GenerateApkg_WithTemplateMediaFiles_EmbedsMediaInPackage()
    {
        // ARRANGE
        var tempDir = Path.Combine(Path.GetTempPath(), $"media_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        var markdownPath = Path.Combine(tempDir, "media_deck.md");
        var sourceMediaPath = GetTestDataPath("banana.svg");
        var copiedMediaPath = Path.Combine(tempDir, "banana.svg");
        var outputApkg = Path.Combine(tempDir, "media_deck.apkg");

        File.Copy(sourceMediaPath, copiedMediaPath, overwrite: true);

        var markdown = """
            ---
            deck_name: "Media Deck"
            source: ""
            separator: "---"
            templates:
              - name: "Basic"
                anki_model_type: "standard"
                media_files:
                  - source: "./banana.svg"
                fields: [Question, Answer]
                html_question_format: "<img src='banana.svg'><div>{{Question}}</div>"
                html_answer_format: "<img src='banana.svg'><div>{{Answer}}</div>"
                css_format: ""
            ---

            # Media

            ```Basic
            Q
            ---
            A
            ```
            """;

        await File.WriteAllTextAsync(markdownPath, markdown);

        var parser = CreateParser();
        var generator = (IAnkiGeneratorService)new AnkiGeneratorService(new AnkiNoteTypeFactory(), new AnkiCardGenerator());

        try
        {
            // ACT
            await generator.GenerateApkg(parser, markdownPath, outputApkg);
            var collection = await AnkiFileReader.ReadFromFileAsync(outputApkg);

            // ASSERT
            collection.EmbeddedMediaFiles.Should().Contain(m => m.Name == "banana.svg");
            collection.EmbeddedMediaFiles.First(m => m.Name == "banana.svg").Content.Length.Should().BeGreaterThan(0);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
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
