using System.Runtime.InteropServices;
using MarkdownToAnki.Infrastructure.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"--> Command line arguments received:\n{args}");
        if(args.Length == 0)
        {
            Console.WriteLine("--> InputPath is required and OutputPath is optional");
            Environment.Exit(0);
        }
        string filePath = args[0];
        string outputPath = args.Length == 2 ? args[1] : Path.Join(Directory.GetCurrentDirectory(),$"ankiDeck_{DateTime.Now}");
        IMarkdownParserService parser = new MarkdownParserService();
        IAnkiGeneratorService generator = new AnkiGeneratorService();
        generator.GenerateApkg(parser, outputPath);
        Console.WriteLine($"--> Deck generated at: {outputPath}");
    }
}


