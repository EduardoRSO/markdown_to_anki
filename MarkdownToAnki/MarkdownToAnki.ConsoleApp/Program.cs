using System.Runtime.InteropServices;
using MarkdownToAnki.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                // Register parser services
                services.AddSingleton<IDeckConfigParser, DeckConfigParser>();
                services.AddSingleton<IMarkdownHeaderHierarchyExtractor, MarkdownHeaderHierarchyExtractor>();
                services.AddSingleton<IFlashCardContentExtractor, FlashCardContentExtractor>();
                services.AddSingleton<ITagNormalizer, TagNormalizer>();

                // Register main parser as facade
                services.AddSingleton<IMarkdownParserService, MarkdownParserService>();

                // Register Anki generation services
                services.AddSingleton<IAnkiNoteTypeFactory, AnkiNoteTypeFactory>();
                services.AddSingleton<IAnkiCardGenerator, AnkiCardGenerator>();
                services.AddSingleton<IAnkiGeneratorService, AnkiGeneratorService>();

                // Register Anki reading and markdown writing services
                services.AddSingleton<IAnkiPackageReader, AnkiPackageReader>();
                services.AddSingleton<IMarkdownDeckWriter, MarkdownDeckWriter>();
                services.AddSingleton<IApkgToMarkdownService, ApkgToMarkdownService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        if (args.Length == 0)
        {
            logger.LogInformation("Usage: md2anki <input.md|input.apkg> [output.apkg|output.md]");
            return 1;
        }

        string input = args[0];
        if (!File.Exists(input))
        {
            logger.LogError("Input file not found: {Path}", input);
            return 2;
        }

        bool isApkgInput = string.Equals(Path.GetExtension(input), ".apkg", StringComparison.OrdinalIgnoreCase);
        
        string output;
        if (args.Length > 1)
        {
            output = args[1];
        }
        else
        {
            // Default: current directory with timestamped filename
            string baseName = Path.GetFileNameWithoutExtension(input);
            string extension = isApkgInput ? ".md" : ".apkg";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            output = Path.Combine(Directory.GetCurrentDirectory(), $"{baseName}_{timestamp}{extension}");
        }

        try
        {
            if (isApkgInput)
            {
                var apkgToMarkdown = host.Services.GetRequiredService<IApkgToMarkdownService>();
                await apkgToMarkdown.GenerateMarkdownAsync(input, output);
                logger.LogInformation("Markdown generated at {Path}", output);
            }
            else
            {
                var parser = host.Services.GetRequiredService<IMarkdownParserService>();
                var generator = host.Services.GetRequiredService<IAnkiGeneratorService>();

                await generator.GenerateApkg(parser, input, output);
                logger.LogInformation("Deck generated at {Path}", output);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.StackTrace, ex.Data, "Failed to generate deck");
            return 3;
        }

        return 0;
    }
}


