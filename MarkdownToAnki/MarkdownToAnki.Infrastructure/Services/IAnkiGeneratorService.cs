namespace MarkdownToAnki.Infrastructure.Services;

public interface IAnkiGeneratorService
{
    Task GenerateApkg(IMarkdownParserService markdownParser, string inputPath, string outputPath);
}