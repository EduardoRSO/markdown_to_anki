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
                services.AddSingleton<IMarkdownParserService, MarkdownParserService>();
                services.AddSingleton<IAnkiGeneratorService, AnkiGeneratorService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        if (args.Length == 0)
        {
            logger.LogInformation("Usage: md2anki <input.md> [output.apkg]");
            return 1;
        }

        string input = args[0];
        if (!File.Exists(input))
        {
            logger.LogError("Input file not found: {Path}", input);
            return 2;
        }

        string output = args.Length > 1
            ? args[1]
            : Path.Combine(Directory.GetCurrentDirectory(), $"ankiDeck_{DateTime.Now:yyyyMMddHHmmss}.apkg");

        try
        {
            var parser = host.Services.GetRequiredService<IMarkdownParserService>();
            var generator = host.Services.GetRequiredService<IAnkiGeneratorService>();

            await generator.GenerateApkg(parser, input, output);
            logger.LogInformation("Deck generated at {Path}", output);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.StackTrace, ex.Data, "Failed to generate deck");
            return 3;
        }

        return 0;
    }
}


