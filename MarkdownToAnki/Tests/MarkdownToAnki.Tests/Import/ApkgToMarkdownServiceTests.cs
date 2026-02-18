using FluentAssertions;
using MarkdownToAnki.Infrastructure.Services;

namespace MarkdownToAnki.Tests.Import;

public class ApkgToMarkdownServiceTests
{
    [Fact]
    public async Task GenerateMarkdownAsync_WithApkgInput_WritesMarkdownWithYamlHeader()
    {
        var inputPath = GetTestDataPath("teoria.apkg");
        var outputPath = Path.Combine(Path.GetTempPath(), $"teoria_{Guid.NewGuid()}.md");

        var service = new ApkgToMarkdownService(new AnkiPackageReader(), new MarkdownDeckWriter());

        try
        {
            await service.GenerateMarkdownAsync(inputPath, outputPath);

            File.Exists(outputPath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(outputPath);
            content.Should().StartWith("---");
            content.Should().Contain("deck_name:");
            content.Should().Contain("templates:");
            content.Should().Contain("```");
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    private static string GetTestDataPath(string fileName)
    {
        var testDataDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestData"));

        return Path.Combine(testDataDir, fileName);
    }
}
