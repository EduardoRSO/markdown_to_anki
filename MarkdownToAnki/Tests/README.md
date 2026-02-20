# Test Structure

This folder contains the comprehensive test suite for the MarkdownToAnki project using **xUnit** and **FluentAssertions**.

## Organization

```Structure
Tests/
├── TestData/                          # Markdown test files
│   ├── simple_deck.md                 # Basic single-template test
│   ├── complex_hierarchy.md           # Multiple templates and headers
│   └── edge_cases.md                  # Edge cases and special characters
├── Output/                            # Generated .apkg files (ignored in git)
│   └── .gitignore
└── MarkdownToAnki.Tests/              # xUnit test project
    ├── GlobalUsings.cs                # Shared global usings
    ├── Fixtures/                      # Test data and builders
    │   ├── MarkdownFixtures.cs        # Predefined markdown constants
    │   └── TestDataBuilder.cs         # Fluent builders for domain objects
    ├── Parsers/                       # Tests for parsing services
    │   ├── DeckConfigParserTests.cs
    │   ├── TagNormalizerTests.cs
    │   ├── MarkdownHeaderHierarchyExtractorTests.cs
    │   ├── FlashCardContentExtractorTests.cs
    │   └── MarkdownParserServiceIntegrationTests.cs
    └── Generation/                    # Tests for generation services
        ├── AnkiNoteTypeFactoryTests.cs
        ├── DeckHierarchyBuilderTests.cs
        └── AnkiCardGeneratorTests.cs
```

## Testing Strategy

### AAA Pattern (Arrange-Act-Assert)

Each test follows the AAA pattern for clarity:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // ARRANGE - Set up test data
    var input = TestDataBuilder.CreateSimpleDeck();
    var service = new MyService();
    
    // ACT - Execute the code being tested
    var result = service.ProcessDeck(input);
    
    // ASSERT - Verify the outcome
    result.Should().NotBeNull();
    result.DeckName.Should().Be("Expected");
}
```

### Inline Markdown Data

Rather than using external files, test markdown is defined inline in fixtures:

```csharp
public static string SimpleDeckYaml => """
    deck_name: "Test"
    separator: "---"
        media_root: "./media"
    templates:
      - name: "Basic"
                media_files: []
        ...
    """;
```

**Benefits:**

- ✅ Tests are self-contained and versionable
- ✅ No file path dependencies
- ✅ Each test is independent
- ✅ Easy to run in CI/CD

### No External File Dependencies

- Tests don't rely on external markdown files
- Exception: `.apkg` files (binary output) are stored in `Output/` folder but git-ignored
- Markdown content is defined in `MarkdownFixtures.cs`
- Test helpers are in `TestDataBuilder.cs`

## Running Tests

### Run All Tests

```bash
dotnet test Tests/MarkdownToAnki.Tests/
```

### Run Specific Test Class

```bash
dotnet test Tests/MarkdownToAnki.Tests/ --filter "TagNormalizerTests"
```

### Run Tests with Verbose Output

```bash
dotnet test Tests/MarkdownToAnki.Tests/ -v normal
```

### Watch Mode (requires dotnet-watch)

```bash
dotnet watch test Tests/MarkdownToAnki.Tests/
```

## Test Coverage

### Phase 1: Parsing Layer (Foundation)

- ✅ **DeckConfigParser**: YAML extraction and template parsing
- ✅ **TagNormalizer**: Accent removal, case conversion, special char handling
- ✅ **MarkdownHeaderHierarchyExtractor**: Header level tracking and hierarchy building
- ✅ **FlashCardContentExtractor**: Code block identification and field extraction
- ✅ **MarkdownParserService**: End-to-end parsing integration

### Phase 2: Generation Layer (Foundation)

- ✅ **AnkiNoteTypeFactory**: Domain-to-AnkiNet structure conversion
- ✅ **DeckHierarchyBuilder**: Nested deck creation and caching
- ✅ **AnkiCardGenerator**: Note creation with metadata

### Phase 3: Full Pipelines

- 🚧 **AnkiGeneratorService**: End-to-end generation
- 🚧 **Round-trip validation**: Markdown→APKG→Markdown equivalence

## Test Data Builders

Use `TestDataBuilder` to quickly create test objects:

```csharp
// Create a basic template
var template = TestDataBuilder.CreateBasicTemplate("MyTemplate");

// Create a deck with multiple templates
var deck = TestDataBuilder.CreateMultiTemplateDeck(
    TestDataBuilder.CreateConceptTemplate(),
    TestDataBuilder.CreateBasicTemplate()
);

// Create a header hierarchy
var hierarchy = TestDataBuilder.CreateHierarchy("Math", "Arithmetic", "Addition");

// Create a flashcard with tags
var card = TestDataBuilder.CreateFlashCard(
    template,
    new Dictionary<string, string> { { "Question", "What?" }, { "Answer", "This!" } },
    new List<string> { "math", "arithmetic" }
);
```

## Writing New Tests

### Template for Unit Tests

```csharp
namespace MarkdownToAnki.Tests.ServiceName;

public class ServiceNameTests
{
    [Fact]
    public void MethodName_Scenario_ExpectedResult()
    {
        // ARRANGE
        var input = TestDataBuilder.Create...();
        var service = new ServiceName();

        // ACT
        var result = service.Method(input);

        // ASSERT
        result.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("input1", "expected1")]
    [InlineData("input2", "expected2")]
    public void MethodName_WithVariousInputs_HandlesCorrectly(string input, string expected)
    {
        // ARRANGE
        var service = new ServiceName();

        // ACT
        var result = service.Method(input);

        // ASSERT
        result.Should().Be(expected);
    }
}
```

### Using Theory Tests for Multiple Inputs

```csharp
[Theory]
[InlineData("Spéciál", "special")]
[InlineData("Multi Word", "multi_word")]
[InlineData("UPPERCASE", "uppercase")]
public void NormalizeTag_WithVariousInputs_Normalizes(string input, string expected)
{
    // ARRANGE
    var normalizer = new TagNormalizer();

    // ACT
    var result = normalizer.NormalizeTag(input);

    // ASSERT
    result.Should().Be(expected);
}
```

## Fluent Assertions Chaining

Use fluent assertions for readable test assertions:

```csharp
result.Should()
    .NotBeNull()
    .And.HaveCount(3)
    .And.AllSatisfy(x => x.IsValid.Should().BeTrue());

collection.Tags.Should()
    .Contain("expected")
    .And.NotContain("unexpected");

string.Should().Match("*pattern*");
```

## Future Additions

- Integration tests for full pipeline
- Performance benchmarks for large files
- ApkgToMarkdown converter tests (Phase 3)
- Round-trip validation tests
