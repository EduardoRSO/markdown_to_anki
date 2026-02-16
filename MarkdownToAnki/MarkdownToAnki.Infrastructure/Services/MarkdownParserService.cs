using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using MarkdownToAnki.Domain.Models;
using YamlDotNet.RepresentationModel;

namespace MarkdownToAnki.Infrastructure.Services;

public class MarkdownParserService : IMarkdownParserService
{
    public async Task<(DeckDefinition, List<FlashCardNote>)> ParseFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        
        // Split YAML front-matter from markdown body
        // Find the first occurrence of "---" at the start of a line
        var firstDelimiter = content.IndexOf("---");
        if (firstDelimiter == -1)
            throw new InvalidOperationException("File must start with ---");
        
        // Find the second "---" that appears at the start of a line (after a newline)
        var searchStartIndex = firstDelimiter + 3;
        var secondDelimiter = content.IndexOf("\n---", searchStartIndex);
        if (secondDelimiter == -1)
            throw new InvalidOperationException("File must contain closing --- for YAML front-matter");
        
        // Extract YAML content between the two delimiters
        var yamlContent = content.Substring(firstDelimiter + 3, secondDelimiter - firstDelimiter - 3).Trim();
        var markdownBody = content.Substring(secondDelimiter + 5).Trim(); // +5 to skip "\n---"
        
        // Parse YAML
        var yaml = new YamlStream();
        yaml.Load(new StringReader(yamlContent));
        
        if (yaml.Documents.Count == 0)
            throw new InvalidOperationException("No YAML document found");
        
        var root = (YamlMappingNode)yaml.Documents[0].RootNode;
        
        // Extract deck definition
        var deckDefinition = new DeckDefinition
        {
            DeckName = GetYamlValue(root, "deck_name"),
            Source = GetYamlValue(root, "source"),
            Separator = GetYamlValue(root, "separator"),
            Templates = ParseTemplates(root)
        };
        
        // Extract flash cards from code blocks
        var flashCards = ParseFlashCards(markdownBody, deckDefinition);
        
        return (deckDefinition, flashCards);
    }
    
    private string GetYamlValue(YamlMappingNode root, string key)
    {
        var keyNode = root.Children.Keys.FirstOrDefault(k => 
            k is YamlScalarNode && ((YamlScalarNode)k).Value == key);
        
        if (keyNode != null)
        {
            var valueNode = root.Children[keyNode];
            if (valueNode is YamlScalarNode scalarNode)
                return scalarNode.Value;
        }
        
        return string.Empty;
    }
    
    private List<TemplateDefinition> ParseTemplates(YamlMappingNode root)
    {
        var templates = new List<TemplateDefinition>();
        
        var templatesKey = root.Children.Keys.FirstOrDefault(k =>
            k is YamlScalarNode && ((YamlScalarNode)k).Value == "templates");
        
        if (templatesKey == null)
            return templates;
        
        var templatesNode = root.Children[templatesKey] as YamlSequenceNode;
        if (templatesNode == null)
            return templates;
        
        foreach (var item in templatesNode.Children)
        {
            if (item is YamlMappingNode templateMap)
            {
                var template = new TemplateDefinition
                {
                    Name = GetYamlValue(templateMap, "name"),
                    Fields = ParseFields(templateMap),
                    Usage = GetYamlValue(templateMap, "usage"),
                    HtmlQuestionFormat = GetYamlValue(templateMap, "html_question_format"),
                    HtmlAnswerFormat = GetYamlValue(templateMap, "html_answer_format"),
                    CssFormat = GetYamlValue(templateMap, "css_format")
                };
                templates.Add(template);
            }
        }
        
        return templates;
    }
    
    private List<string> ParseFields(YamlMappingNode templateMap)
    {
        var fields = new List<string>();
        
        var fieldsKey = templateMap.Children.Keys.FirstOrDefault(k =>
            k is YamlScalarNode && ((YamlScalarNode)k).Value == "fields");
        
        if (fieldsKey == null)
            return fields;
        
        var fieldsNode = templateMap.Children[fieldsKey] as YamlSequenceNode;
        if (fieldsNode == null)
            return fields;
        
        foreach (var field in fieldsNode.Children)
        {
            if (field is YamlScalarNode scalarNode)
                fields.Add(scalarNode.Value);
        }
        
        return fields;
    }
    
    private List<FlashCardNote> ParseFlashCards(string markdownBody, DeckDefinition deckDefinition)
    {
        var flashCards = new List<FlashCardNote>();
        var templateMap = deckDefinition.Templates.ToDictionary(t => t.Name);
        
        var headerPattern = @"^(#{1,6})\s+(.+)$";
        var codeBlockStartPattern = @"^```([A-Za-z0-9_]+)\s*$";
        
        var lines = markdownBody.Split('\n');
        var currentHeadersByLevel = new Dictionary<int, string>();
        var currentCodeBlock = new List<string>();
        string currentTemplate = null;
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            // Check if this is a header
            var headerMatch = Regex.Match(line, headerPattern);
            if (headerMatch.Success)
            {
                int level = headerMatch.Groups[1].Value.Length;
                string headerText = headerMatch.Groups[2].Value;
                
                // Remove headers deeper than or equal to current level
                var keysToRemove = currentHeadersByLevel.Keys.Where(k => k >= level).ToList();
                foreach (var key in keysToRemove)
                {
                    currentHeadersByLevel.Remove(key);
                }
                
                // Add this header
                currentHeadersByLevel[level] = headerText;
                continue;
            }
            
            // Check if this is a code block start
            var blockStartMatch = Regex.Match(line, codeBlockStartPattern);
            if (blockStartMatch.Success && currentTemplate == null)
            {
                currentTemplate = blockStartMatch.Groups[1].Value;
                currentCodeBlock.Clear();
                continue;
            }
            
            // Check if this is a code block end
            if (line.TrimStart().StartsWith("```") && currentTemplate != null)
            {
                // Process the code block
                if (templateMap.ContainsKey(currentTemplate))
                {
                    var template = templateMap[currentTemplate];
                    var fieldValues = new Dictionary<string, string>();
                    var content = string.Join("\n", currentCodeBlock);
                    
                    // Split content by separator
                    var values = content.Split(new[] { deckDefinition.Separator }, StringSplitOptions.None)
                        .Select(v => v.Trim())
                        .Select(v => v.Replace("\n", "<br>"))  // Convert newlines to HTML line breaks
                        .ToList();
                    
                    // Map values to fields
                    for (int j = 0; j < template.Fields.Count && j < values.Count; j++)
                    {
                        fieldValues[template.Fields[j]] = values[j];
                    }
                    
                    // Get tags from current header hierarchy, normalized
                    var tags = currentHeadersByLevel
                        .OrderBy(x => x.Key)  // Sort by level
                        .Select(x => NormalizeTag(x.Value))
                        .ToList();
                    
                    var note = new FlashCardNote
                    {
                        Template = template,
                        FieldValues = fieldValues,
                        Tags = tags
                    };
                    
                    flashCards.Add(note);
                }
                
                currentTemplate = null;
                currentCodeBlock.Clear();
                continue;
            }
            
            // If we're in a code block, accumulate the content
            if (currentTemplate != null)
            {
                currentCodeBlock.Add(line);
            }
        }
        
        return flashCards;
    }
    
    private string NormalizeTag(string tag)
    {
        // Remove accents using NFD decomposition
        string normalized = tag.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new StringBuilder();
        
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        
        string result = sb.ToString();
        
        // Convert to lowercase and replace spaces/special chars with underscores
        result = result.ToLower();
        result = Regex.Replace(result, @"[^a-z0-9]+", "_");
        result = Regex.Replace(result, @"^_+|_+$", ""); // Remove leading/trailing underscores
        
        return result;
    }
}