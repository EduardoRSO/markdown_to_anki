namespace MarkdownToAnki.Infrastructure.Services;

public class AnkiGeneratorService : IAnkiGeneratorService
{
    Task IAnkiGeneratorService.GenerateApkg(IMarkdownParserService markdownParser, string outputPath)
    {
        throw new NotImplementedException();
    }
}