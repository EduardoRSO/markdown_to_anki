using AnkiNet;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IAnkiPackageReader
{
    Task<AnkiCollection> ReadAsync(string filePath);
}
