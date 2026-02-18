using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IDeckConfigParser
{
    DeckDefinition ParseDeckConfig(string yamlContent);
}
