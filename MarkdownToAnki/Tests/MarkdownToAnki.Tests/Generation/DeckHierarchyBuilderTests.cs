namespace MarkdownToAnki.Tests.Generation;

using MarkdownToAnki.Tests.Fixtures;
using AnkiNet;

public class DeckHierarchyBuilderTests
{
    [Fact]
    public void GetOrCreateDeckForHierarchy_WithEmptyHierarchy_ReturnsRootDeckId()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var rootDeckId = collection.CreateDeck("TestDeck");
        var builder = new DeckHierarchyBuilder(collection, "TestDeck");
        var emptyHierarchy = new HeaderHierarchy();

        // ACT
        var result = builder.GetOrCreateDeckForHierarchy(rootDeckId, emptyHierarchy);

        // ASSERT
        result.Should().Be(rootDeckId);
    }

    [Fact]
    public void GetOrCreateDeckForHierarchy_WithSingleLevel_CreateSubdeck()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var rootDeckId = collection.CreateDeck("Root");
        var builder = new DeckHierarchyBuilder(collection, "Root");
        var hierarchy = TestDataBuilder.CreateHierarchy("Math");

        // ACT
        var result = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy);

        // ASSERT
        result.Should().NotBe(rootDeckId);
    }

    [Fact]
    public void GetOrCreateDeckForHierarchy_WithMultipleLevels_CreateNestedDecks()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var rootDeckId = collection.CreateDeck("Science");
        var builder = new DeckHierarchyBuilder(collection, "Science");
        var hierarchy = TestDataBuilder.CreateHierarchy("Physics", "Mechanics", "Newton");

        // ACT
        var result = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy);

        // ASSERT
        result.Should().NotBe(rootDeckId);
    }

    [Fact]
    public void GetOrCreateDeckForHierarchy_SameHierarchyTwice_ReturnsCached()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var rootDeckId = collection.CreateDeck("Root");
        var builder = new DeckHierarchyBuilder(collection, "Root");
        var hierarchy = TestDataBuilder.CreateHierarchy("Topic");

        // ACT
        var firstCall = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy);
        var secondCall = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy);

        // ASSERT
        firstCall.Should().Be(secondCall);
    }

    [Fact]
    public void GetOrCreateDeckForHierarchy_DifferentHierarchies_ReturnDifferentIds()
    {
        // ARRANGE
        var collection = new AnkiCollection();
        var rootDeckId = collection.CreateDeck("Root");
        var builder = new DeckHierarchyBuilder(collection, "Root");
        var hierarchy1 = TestDataBuilder.CreateHierarchy("TopicA");
        var hierarchy2 = TestDataBuilder.CreateHierarchy("TopicB");

        // ACT
        var deckA = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy1);
        var deckB = builder.GetOrCreateDeckForHierarchy(rootDeckId, hierarchy2);

        // ASSERT
        deckA.Should().NotBe(deckB);
    }
}
