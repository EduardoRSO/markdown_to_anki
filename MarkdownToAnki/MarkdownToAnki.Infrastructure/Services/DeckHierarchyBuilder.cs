using AnkiNet;
using MarkdownToAnki.Domain.Models;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Manages nested deck hierarchy creation and navigation.
/// Maps header hierarchies (H1::H2::H3) to Anki subdeck structure.
/// </summary>
public class DeckHierarchyBuilder : IDeckHierarchyBuilder
{
    private readonly AnkiCollection _collection;
    private readonly Dictionary<string, long> _deckPathCache;

    public DeckHierarchyBuilder(AnkiCollection collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        _deckPathCache = new Dictionary<string, long>();
    }

    public long GetOrCreateDeckForHierarchy(long rootDeckId, HeaderHierarchy headerHierarchy)
    {
        var hierarchyPath = headerHierarchy.GetHierarchyPath();
        
        if (hierarchyPath.Count == 0)
            return rootDeckId;

        // Create a cache key from the hierarchy path
        string cacheKey = string.Join("::", hierarchyPath);
        
        if (_deckPathCache.TryGetValue(cacheKey, out long cachedDeckId))
            return cachedDeckId;

        // Navigate/create the deck hierarchy
        long currentDeckId = rootDeckId;
        var pathSoFar = new List<string>();
        
        foreach (var header in hierarchyPath)
        {
            pathSoFar.Add(header);
            string currentPath = string.Join("::", pathSoFar);
            
            if (_deckPathCache.TryGetValue(currentPath, out long existingDeckId))
            {
                currentDeckId = existingDeckId;
            }
            else
            {
                // Create new subdeck - AnkiNet handles hierarchy via "::" in deck names
                string deckName = string.Join("::", pathSoFar);
                currentDeckId = _collection.CreateDeck(deckName);
                _deckPathCache[currentPath] = currentDeckId;
            }
        }

        _deckPathCache[cacheKey] = currentDeckId;
        return currentDeckId;
    }
}
