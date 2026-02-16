using System.IO.Compression;
using System.Data.SQLite;

namespace MarkdownToAnki.Infrastructure.Services;

public class AnkiTagService
{
    public static void AddTagsToApkg(string apkgFilePath, List<List<string>> tagsPerNote)
    {
        if (!File.Exists(apkgFilePath))
            throw new FileNotFoundException($"APKG file not found: {apkgFilePath}");

        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        
        try
        {
            Directory.CreateDirectory(tempDir);

            // Extract the .apkg (which is a ZIP file)
            ZipFile.ExtractToDirectory(apkgFilePath, tempDir);

            // Find collection.anki2/collection.anki21 - search recursively
            string dbPath = Directory.EnumerateFiles(tempDir, "collection.anki*", SearchOption.AllDirectories)
                .FirstOrDefault();
            
            if (dbPath == null)
            {
                // Log what files were extracted for debugging
                var filesFound = Directory.EnumerateFiles(tempDir, "*", SearchOption.AllDirectories).ToList();
                throw new FileNotFoundException($"collection.anki database file not found in APKG. Files found: {string.Join(", ", filesFound)}");
            }

            // Add tags to the database
            AddTagsToDatabase(dbPath, tagsPerNote);

            // Re-create the ZIP file
            string backupPath = apkgFilePath + ".bak";
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            
            File.Move(apkgFilePath, backupPath);

            using (var zipArchive = ZipFile.Open(apkgFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.EnumerateFiles(tempDir, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(tempDir, file);
                    zipArchive.CreateEntryFromFile(file, relativePath);
                }
            }

            // Delete backup if successful
            if (File.Exists(backupPath))
                File.Delete(backupPath);
        }
        finally
        {
            // Clean up temp directory
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private static void AddTagsToDatabase(string dbPath, List<List<string>> tagsPerNote)
    {
        string connectionString = $"Data Source={dbPath};Version=3;";
        
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Get all notes ordered by creation (id ascending)
            var notes = new List<(long id, string tags)>();
            using (var command = new SQLiteCommand("SELECT id, tags FROM notes ORDER BY id ASC", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = (long)reader[0];
                        string tags = reader[1] != DBNull.Value ? reader[1].ToString() ?? "" : "";
                        notes.Add((id, tags));
                    }
                }
            }

            // Apply tags to each note
            // Tags in Anki are stored as a space-separated string
            for (int i = 0; i < notes.Count && i < tagsPerNote.Count; i++)
            {
                long noteId = notes[i].id;
                var newTags = tagsPerNote[i];
                
                if (newTags.Count == 0)
                    continue;

                // Get current tags and append new ones
                string currentTags = notes[i].tags.Trim();
                var allTags = new HashSet<string>();
                
                if (!string.IsNullOrWhiteSpace(currentTags))
                {
                    foreach (var tag in currentTags.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        allTags.Add(tag);
                    }
                }
                
                foreach (var tag in newTags)
                {
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        allTags.Add(tag.Trim());
                    }
                }
                
                // Update the note with combined tags (space-separated)
                string updatedTags = string.Join(" ", allTags);
                using (var command = new SQLiteCommand(
                    "UPDATE notes SET tags = @tags WHERE id = @noteId",
                    connection))
                {
                    command.Parameters.AddWithValue("@tags", updatedTags);
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        }
    }
}
