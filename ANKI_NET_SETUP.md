# Using Development Version of Anki.NET

## Current Setup

You're now using Anki.NET 2.0.0 from a local NuGet feed with the new metadata and field properties features.

### Local NuGet Feed Location

- **Feed Path**: `C:\Repos\NugetLocal`
- **Feed Name**: `Local-Anki-Dev`
- **Package**: `Anki.NET.2.0.0.nupkg`

### Source Location

- **Fork**: `C:\Repos\forks\Anki.NET\src\AnkiNet`
- **Branch**: `feature/expose-note-metadata-and-field-properties`
- **Upstream URL**: https://github.com/EduardoRSO/Anki.NET/tree/feature/expose-note-metadata-and-field-properties?tab=readme-ov-file

## Using the New Features

### 1. Create Notes with Custom Metadata

```csharp
using AnkiNet;

var collection = new AnkiCollection();
var noteTypeId = collection.CreateNoteType(...);
var deckId = collection.CreateDeck("My Deck");

// Create a note with full metadata control
collection.CreateNoteWithMetadata(
    deckId: deckId,
    noteTypeId: noteTypeId,
    fields: new[] { "Front", "Back" },
    guid: "custom-guid-123",
    tags: " vocabulary important ",
    modifiedDateTime: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    updateSequenceNumber: 1
);
```

### 2. Define Custom Field Properties

```csharp
// Create field definitions with custom properties
var fields = new[]
{
    new AnkiField("Question", 0)
    {
        Font = "Verdana",
        FontSize = 16,
        IsSticky = true,
        Description = "The question to ask"
    },
    new AnkiField("Answer", 1)
    {
        Font = "Georgia",
        FontSize = 14,
        IsRightToLeft = false
    }
};

// Create note type with custom field properties
var noteType = new AnkiNoteType(
    name: "Custom Type",
    cardTypes: new[] { cardType },
    fields: fields,
    css: "@media print { ... }"
);

var noteTypeId = collection.CreateNoteType(noteType);
```

### 3. Access Note Metadata

```csharp
// Read a collection
var collection = await AnkiFileReader.ReadFromFileAsync("collection.apkg");

// Access note metadata
foreach (var deck in collection.Decks)
{
    foreach (var card in deck.Cards)
    {
        var note = card.Note;
        
        // New metadata properties are available
        Console.WriteLine($"Guid: {note.Guid}");
        Console.WriteLine($"Tags: {note.Tags}");
        Console.WriteLine($"Modified: {note.ModifiedDateTime}");
        Console.WriteLine($"Sync: {note.UpdateSequenceNumber}");
    }
}
```

### 4. Access Field Properties

```csharp
// Field definitions now include formatting properties
foreach (var noteType in collection.NoteTypes)
{
    foreach (var field in noteType.Fields)
    {
        Console.WriteLine($"Field: {field.Name}");
        Console.WriteLine($"  Font: {field.Font}");
        Console.WriteLine($"  Size: {field.FontSize}pt");
        Console.WriteLine($"  RTL: {field.IsRightToLeft}");
        Console.WriteLine($"  Sticky: {field.IsSticky}");
        Console.WriteLine($"  Description: {field.Description}");
    }
}
```

## Updating the Local Package

When you make changes to the Anki.NET fork, rebuild the local package:

```bash
# Build from the fork
cd C:\Repos\forks\Anki.NET\src\AnkiNet
dotnet pack -c Release -o "c:\Repos\NugetLocal"

# Clear NuGet cache to force re-download
dotnet nuget locals all --clear

# Restore packages in your project
cd C:\Repos\markdown_to_anki\MarkdownToAnki
dotnet restore
```

## Switching Between Versions

### Use Local Development Version (Current Setup)

```bash
cd C:\Repos\markdown_to_anki\MarkdownToAnki
dotnet restore
```

### Switch Back to NuGet Package (When PR is merged)

1. Remove the local NuGet source:

```bash
dotnet nuget remove source "Local-Anki-Dev"
```

1. Update to the official package:

```bash
dotnet package update Anki.NET --interactive
```

## Troubleshooting

### Force Re-download of Local Package

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

### Verify Which Package is Being Used

```bash
# Check the lock file
cat C:\Repos\markdown_to_anki\MarkdownToAnki\packages.lock.json | grep -A 5 "Anki.NET"

# Or check the package location
cd C:\Repos\markdown_to_anki\MarkdownToAnki
dotnet build --verbosity diagnostic 2>&1 | grep -i "anki"
```

### Local NuGet Sources Configuration

View your NuGet configuration:

```bash
dotnet nuget list source
```

Should show:

```
Registered Sources:
  1.  Local-Anki-Dev [Enabled]
      C:\Repos\NugetLocal
  2.  https://api.nuget.org/v3/index.json [Enabled]
      nuget.org
```

## Project Structure

```
C:\Repos\
├── forks\Anki.NET\
│   └── src\AnkiNet\        ← Fork with new features (branch: feature/expose-note-metadata-and-field-properties)
├── NugetLocal\             ← Local NuGet feed (contains Anki.NET.2.0.0.nupkg)
└── markdown_to_anki\
    └── MarkdownToAnki\     ← Your project (uses local Anki.NET package)
```

## Important Notes

⚠️ **Before PR Submission**:

- Changes to the fork are only available locally
- markdown_to_anki uses the development version
- Once PR is merged to main, update to use the official NuGet package

✅ **After PR is Merged**:

- Remove local NuGet source
- NuGet.org will have the new version with metadata/field features
- No local setup needed for new clones

## Next Steps

1. **Develop & Test**: Use the new metadata/field features in markdown_to_anki
2. **Submit PR**: Push the Anki.NET fork to the main repo
3. **Wait for Merge**: Official package will be published to NuGet.org
4. **Switch to Official**: Update markdown_to_anki to use published package

---

**Last Updated**: February 18, 2026
**Local Package Version**: 2.0.0
**Fork Branch**: feature/expose-note-metadata-and-field-properties
