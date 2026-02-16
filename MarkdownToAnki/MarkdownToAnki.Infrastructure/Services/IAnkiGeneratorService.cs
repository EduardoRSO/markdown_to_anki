namespace MarkdownToAnki.Infrastructure.Services;

public interface IAnkiGeneratorService
{
    void GenerateApkg(IMarkdownParserService markdownParser, string outputPath);
}