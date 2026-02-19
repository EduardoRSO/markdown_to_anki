# Using Development Version of Anki.NET

## Current Setup

This project is currently using Anki.NET directly from your local fork source code via `ProjectReference`.

### Local NuGet Feed (Optional)

- **Feed Path**: `C:\Repos\NugetLocal`
- **Feed Name**: `Local-Anki-Dev`
- **Use case**: optional packaging workflow (not the current default in this repo)

### Source Location

- **Fork**: `C:\Repos\forks\Anki.NET\src\AnkiNet`
- **Branch**: `feature/expose-note-metadata-and-field-properties`
- **Upstream URL**: <https://github.com/EduardoRSO/Anki.NET/tree/feature/expose-note-metadata-and-field-properties?tab=readme-ov-file>

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

## Updating Anki.NET Changes (Current Source-Reference Flow)

When you make changes to the Anki.NET fork, rebuild the fork and restore this project:

```bash
# Build from the fork source
cd C:\Repos\forks\Anki.NET\src\AnkiNet
dotnet build

# Restore in your project
cd C:\Repos\markdown_to_anki\MarkdownToAnki
dotnet restore
```

Because the dependency is a `ProjectReference`, no local NuGet cache cleanup is required for fork code changes.

## Optional: Package-Based Flow

If you decide to consume Anki.NET from `C:\Repos\NugetLocal` again:

1. Pack your fork:

```bash
cd C:\Repos\forks\Anki.NET\src\AnkiNet
dotnet pack -c Release -o "c:\Repos\NugetLocal"
```

1. Switch `MarkdownToAnki.Infrastructure.csproj` from `ProjectReference` to `PackageReference`.
2. Run:

```bash
cd C:\Repos\markdown_to_anki\MarkdownToAnki
dotnet nuget locals all --clear
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

### Verify Which Dependency Source Is Being Used

```bash
# Verify source project reference (current setup)
cd C:\Repos\markdown_to_anki\MarkdownToAnki\MarkdownToAnki.Infrastructure
dotnet list reference
```

You should see a reference to `C:\Repos\forks\Anki.NET\src\AnkiNet\AnkiNet.csproj`.

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
        └── MarkdownToAnki\     ← Your project (uses local Anki.NET source via ProjectReference)
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

**Last Updated**: February 19, 2026
**Dependency Mode**: ProjectReference to local fork source
**Fork Branch**: feature/expose-note-metadata-and-field-properties
