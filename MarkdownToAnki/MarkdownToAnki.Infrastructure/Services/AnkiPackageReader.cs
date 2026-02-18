using AnkiNet;

namespace MarkdownToAnki.Infrastructure.Services;

public class AnkiPackageReader : IAnkiPackageReader
{
    public Task<AnkiCollection> ReadAsync(string filePath)
    {
        return AnkiFileReader.ReadFromFileAsync(filePath);
    }
}
