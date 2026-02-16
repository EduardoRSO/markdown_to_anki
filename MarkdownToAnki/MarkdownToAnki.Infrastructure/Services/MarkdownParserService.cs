using System.Text.RegularExpressions;
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
        
        // Find all code blocks: ```TemplateName\n...\n```
        var codeBlockPattern = @"```([A-Za-z0-9_]+)\s*\n(.*?)\n```";
        var matches = Regex.Matches(markdownBody, codeBlockPattern, RegexOptions.Singleline);
        
        // Extract current headers for tags
        var currentTags = new List<string>();
        var headerPattern = @"^(#{1,6})\s+(.+)$";
        
        var lines = markdownBody.Split('\n');
        int blockIndex = 0;
        
        foreach (var line in lines)
        {
            var headerMatch = Regex.Match(line, headerPattern);
            if (headerMatch.Success)
            {
                int level = headerMatch.Groups[1].Value.Length;
                string headerText = headerMatch.Groups[2].Value;
                
                // Keep only headers of higher level (trim deeper ones)
                // while (currentTags.Count >= level)
                //     currentTags.RemoveAt(currentTags.Count - 1);
                
                currentTags.Add(headerText);
            }
        }
        
        foreach (Match match in matches)
        {
            string templateName = match.Groups[1].Value;
            string content = match.Groups[2].Value;
            
            if (!templateMap.ContainsKey(templateName))
                continue;
            
            var template = templateMap[templateName];
            var fieldValues = new Dictionary<string, string>();
            
            // Split content by separator
            var values = content.Split(new[] { deckDefinition.Separator }, StringSplitOptions.None)
                .Select(v => v.Trim())
                .Select(v => v.Replace("\n", "<br>"))  // Convert newlines to HTML line breaks
                .ToList();
            
            // Map values to fields
            for (int i = 0; i < template.Fields.Count && i < values.Count; i++)
            {
                fieldValues[template.Fields[i]] = values[i];
            }
            
            var note = new FlashCardNote
            {
                Template = template,
                FieldValues = fieldValues,
                Tags = new List<string>(currentTags)
            };
            
            flashCards.Add(note);
        }
        
        return flashCards;
    }
}