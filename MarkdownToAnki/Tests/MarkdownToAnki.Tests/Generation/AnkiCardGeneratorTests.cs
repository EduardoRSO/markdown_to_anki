namespace MarkdownToAnki.Tests.Generation;

using MarkdownToAnki.Tests.Fixtures;
using AnkiNet;

public class AnkiCardGeneratorTests
{
    [Fact]
    public void CreateCardInDeck_WithValidCard_ExecutesWithoutException()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var deckId = collection.CreateDeck("TestDeck");
        
        var template = TestDataBuilder.CreateBasicTemplate();
        var noteTypeId = collection.CreateNoteType(
            new AnkiNoteType(
                "Test",
                [new AnkiCardType("Front", 0, "{{Question}}", "{{Answer}}")],
                ["Question", "Answer"],
                ""
            )
        );

        var card = TestDataBuilder.CreateFlashCard(template);
        var generator = new AnkiCardGenerator();

        // ACT & ASSERT
        var act = () => generator.CreateCardInDeck(collection, deckId, noteTypeId, card);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateCardInDeck_WithMultipleTags_ExecutesWithoutException()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var deckId = collection.CreateDeck("TestDeck");
        
        var template = TestDataBuilder.CreateBasicTemplate();
        var noteTypeId = collection.CreateNoteType(
            new AnkiNoteType(
                "Test",
                [new AnkiCardType("Front", 0, "{{Question}}", "{{Answer}}")],
                ["Question", "Answer"],
                ""
            )
        );

        var card = new FlashCardNote
        {
            Template = template,
            FieldValues = new Dictionary<string, string>
            {
                { "Question", "Q" },
                { "Answer", "A" }
            },
            Tags = new List<string> { "tag1", "tag2", "tag3" }
        };

        var generator = new AnkiCardGenerator();

        // ACT & ASSERT
        var act = () => generator.CreateCardInDeck(collection, deckId, noteTypeId, card);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateCardInDeck_WithEmptyTags_CreatesCardWithoutTags()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var deckId = collection.CreateDeck("TestDeck");
        
        var template = TestDataBuilder.CreateBasicTemplate();
        var noteTypeId = collection.CreateNoteType(
            new AnkiNoteType(
                "Test",
                [new AnkiCardType("Front", 0, "{{Question}}", "{{Answer}}")],
                ["Question", "Answer"],
                ""
            )
        );

        var card = new FlashCardNote
        {
            Template = template,
            FieldValues = new Dictionary<string, string>
            {
                { "Question", "Q" },
                { "Answer", "A" }
            },
            Tags = new List<string>()  // Empty tags
        };

        var generator = new AnkiCardGenerator();

        // ACT & ASSERT
        var ex = Record.Exception(() => 
            generator.CreateCardInDeck(collection, deckId, noteTypeId, card));
        ex.Should().BeNull();
    }
}
