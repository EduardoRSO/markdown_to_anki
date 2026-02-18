namespace MarkdownToAnki.Domain.Models;

/// <summary>
/// Represents the current active markdown header hierarchy.
/// Tracks headers from H1 to H6 and provides the path as tags.
/// </summary>
public class HeaderHierarchy
{
    private readonly Dictionary<int, string> _headersByLevel = new();

    /// <summary>
    /// Updates the hierarchy with a new header at the specified level.
    /// Removes all headers at deeper levels to maintain a valid hierarchy.
    /// </summary>
    public void UpdateHeader(int level, string headerText)
    {
        if (level < 1 || level > 6)
            throw new ArgumentException("Header level must be between 1 and 6", nameof(level));

        // Remove all headers at deeper levels
        var keysToRemove = _headersByLevel.Keys.Where(k => k > level).ToList();
        foreach (var key in keysToRemove)
            _headersByLevel.Remove(key);

        // Add or update the header at this level
        _headersByLevel[level] = headerText;
    }

    /// <summary>
    /// Gets all active headers in order from H1 to deepest level.
    /// </summary>
    public List<string> GetHierarchyPath()
    {
        var result = new List<string>();
        for (int i = 1; i <= 6; i++)
        {
            if (_headersByLevel.TryGetValue(i, out var header))
                result.Add(header);
            else
                break;
        }
        return result;
    }

    /// <summary>
    /// Gets the deepest active header level.
    /// </summary>
    public int GetDeepestLevel()
    {
        return _headersByLevel.Count == 0 ? 0 : _headersByLevel.Keys.Max();
    }

    /// <summary>
    /// Clears all headers from the hierarchy.
    /// </summary>
    public void Clear()
    {
        _headersByLevel.Clear();
    }

    /// <summary>
    /// Creates a deep copy of the current hierarchy.
    /// </summary>
    public HeaderHierarchy Clone()
    {
        var clone = new HeaderHierarchy();
        foreach (var kvp in _headersByLevel)
            clone._headersByLevel[kvp.Key] = kvp.Value;
        return clone;
    }
}
