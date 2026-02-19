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
        var act = () => generator.CreateCardInDeck(collection, deckId, noteTypeId, AnkiNoteTypeModelType.Standard, card);
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
        var act = () => generator.CreateCardInDeck(collection, deckId, noteTypeId, AnkiNoteTypeModelType.Standard, card);
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
            generator.CreateCardInDeck(collection, deckId, noteTypeId, AnkiNoteTypeModelType.Standard, card));
        ex.Should().BeNull();
    }

    [Fact]
    public void CreateCardInDeck_WithClozeModel_ExecutesWithoutException()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var deckId = collection.CreateDeck("TestDeck");

        var noteTypeId = collection.CreateNoteType(
            new AnkiNoteType(
                name: "Omissao cloze template",
                cardTypes: [new AnkiCardType("Cloze", 0, "{{cloze:Text}}", "{{cloze:Text}}<br>{{Back Extra}}")],
                fieldNames: ["Text", "Back Extra"],
                css: "",
                modelType: AnkiNoteTypeModelType.Cloze
            )
        );

        var template = new TemplateDefinition
        {
            Name = "Omissao",
            Fields = ["Texto", "Comentário"]
        };

        var card = new FlashCardNote
        {
            Template = template,
            FieldValues = new Dictionary<string, string>
            {
                { "Texto", "{{c1::Paris}} fica na {{c2::França}}" },
                { "Comentário", "Capital e país" }
            },
            Tags = new List<string> { "geo" }
        };

        var generator = new AnkiCardGenerator();

        // ACT & ASSERT
        var ex = Record.Exception(() =>
            generator.CreateCardInDeck(collection, deckId, noteTypeId, AnkiNoteTypeModelType.Cloze, card));

        ex.Should().BeNull();
    }
}
