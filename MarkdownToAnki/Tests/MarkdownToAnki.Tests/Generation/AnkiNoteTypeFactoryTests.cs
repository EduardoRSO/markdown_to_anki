namespace MarkdownToAnki.Tests.Generation;

using AnkiNet;
using MarkdownToAnki.Tests.Fixtures;

public class AnkiNoteTypeFactoryTests
{
    [Fact]
    public void CreateNoteType_WithBasicTemplate_CreatesValidAnkiNoteType()
    {
        // ARRANGE
        var template = TestDataBuilder.CreateBasicTemplate("MyTemplate");
        var factory = new AnkiNoteTypeFactory();

        // ACT
        var result = factory.CreateNoteType(template, AnkiNoteTypeModelType.Standard);

        // ASSERT
        result.Should().NotBeNull();
        result.Name.Should().Contain("MyTemplate");
        result.CardTypes.Should().HaveCount(1);
        result.Fields.Should().HaveCount(2);
        result.ModelType.Should().Be(AnkiNoteTypeModelType.Standard);
    }

    [Fact]
    public void CreateNoteType_PreservesTemplateProperties()
    {
        // ARRANGE
        var template = new TemplateDefinition
        {
            Name = "Custom",
            Fields = new List<string> { "Q", "A", "Extra" },
            HtmlQuestionFormat = "<custom>{{Q}}</custom>",
            HtmlAnswerFormat = "<custom>{{A}}</custom>",
            CssFormat = ".custom { color: red; }"
        };
        var factory = new AnkiNoteTypeFactory();

        // ACT
        var result = factory.CreateNoteType(template, AnkiNoteTypeModelType.Standard);

        // ASSERT
        result.Fields.Should().HaveCount(3);
        result.Fields[0].Name.Should().Be("Q");
        result.Fields[2].Name.Should().Be("Extra");
        result.Css.Should().Be(".custom { color: red; }");
    }

    [Fact]
    public void CreateNoteType_WithMultipleFields_CreatesCorrectStructure()
    {
        // ARRANGE
        var template = TestDataBuilder.CreateConceptTemplate("Concept", ["Term", "Def", "Example", "Related"]);
        var factory = new AnkiNoteTypeFactory();

        // ACT
        var result = factory.CreateNoteType(template, AnkiNoteTypeModelType.Standard);

        // ASSERT
        result.Fields.Should().HaveCount(4);
        result.CardTypes.Should().HaveCount(1);
    }

    [Fact]
    public void CreateNoteType_WithClozeModel_UsesClozeModelAndFields()
    {
        // ARRANGE
        var template = TestDataBuilder.CreateBasicTemplate("Omissao");
        var factory = new AnkiNoteTypeFactory();

        // ACT
        var result = factory.CreateNoteType(template, AnkiNoteTypeModelType.Cloze);

        // ASSERT
        result.ModelType.Should().Be(AnkiNoteTypeModelType.Cloze);
        result.FieldNames.Should().Equal("Text", "Back Extra");
        result.CardTypes.Should().ContainSingle();
        result.CardTypes[0].QuestionFormat.Should().Be("{{cloze:Text}}");
    }
}
