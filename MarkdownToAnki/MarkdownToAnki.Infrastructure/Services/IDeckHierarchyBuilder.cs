using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

public interface IDeckHierarchyBuilder
{
    long GetOrCreateDeckForHierarchy(long rootDeckId, HeaderHierarchy headerHierarchy);
}
