namespace MarkdownToAnki.Tests.Parsers;

using MarkdownToAnki.Tests.Fixtures;

public class TagNormalizerTests
{
    [Fact]
    public void NormalizeTag_WithSimpleText_ReturnsLowercase()
    {
        // ARRANGE
        var normalizer = new TagNormalizer();
        var input = "MyTag";

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be("mytag");
    }

    [Fact]
    public void NormalizeTag_WithAccents_RemovesAccents()
    {
        // ARRANGE
        var normalizer = new TagNormalizer();
        var input = "Spécial Crèmé";  // Accented characters that normalize to ASCII

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be("special_creme");
    }

    [Fact]
    public void NormalizeTag_WithSpaces_ReplacesWithUnderscores()
    {
        // ARRANGE
        var normalizer = new TagNormalizer();
        var input = "Multi Word Tag";

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be("multi_word_tag");
    }

    [Fact]
    public void NormalizeTag_WithSpecialCharacters_RemovesSpecialChars()
    {
        // ARRANGE
        var normalizer = new TagNormalizer();
        var input = "Tag-With_Special+Chars!";

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be("tag_with_special_chars");
    }

    [Fact]
    public void NormalizeTag_WithLeadingTrailingSpaces_Trims()
    {
        // ARRANGE
        var normalizer = new TagNormalizer();
        var input = "  Trimmed Tag  ";

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be("trimmed_tag");
    }

    [Theory]
    [InlineData("Matemática", "matematica")]
    [InlineData("Razão e Proporção", "razao_e_proporcao")]
    [InlineData("Adição, Subtração", "adicao_subtracao")]
    [InlineData("UPPERCASE", "uppercase")]
    public void NormalizeTag_WithVariousInputs_NormalizesCorrectly(string input, string expected)
    {
        // ARRANGE
        var normalizer = new TagNormalizer();

        // ACT
        var result = normalizer.NormalizeTag(input);

        // ASSERT
        result.Should().Be(expected);
    }
}
