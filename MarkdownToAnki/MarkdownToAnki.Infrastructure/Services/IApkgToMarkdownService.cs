namespace MarkdownToAnki.Infrastructure.Services;

public interface IApkgToMarkdownService
{
    Task GenerateMarkdownAsync(string inputPath, string outputPath);
}
