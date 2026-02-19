using MarkdownToAnki.Domain.Models;
using YamlDotNet.RepresentationModel;

namespace MarkdownToAnki.Infrastructure.Services;

/// <summary>
/// Parses YAML front-matter configuration.
/// Extracts deck metadata and template definitions.
/// </summary>
public class DeckConfigParser : IDeckConfigParser
{
    public DeckDefinition ParseDeckConfig(string yamlContent)
    {
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

        return deckDefinition;
    }

    private static string GetYamlValue(YamlMappingNode root, string key)
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
                var templateName = GetYamlValue(templateMap, "name");
                var template = new TemplateDefinition
                {
                    Name = templateName,
                    ModelType = ParseTemplateModelType(templateMap, templateName),
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

    private static TemplateModelType ParseTemplateModelType(YamlMappingNode templateMap, string templateName)
    {
        var rawModelType = GetYamlValue(templateMap, "anki_model_type").Trim();
        if (string.IsNullOrWhiteSpace(rawModelType))
        {
            throw new InvalidOperationException($"Template '{templateName}' must define 'anki_model_type' with value 'standard' or 'cloze'.");
        }

        return rawModelType.ToLowerInvariant() switch
        {
            "standard" => TemplateModelType.Standard,
            "cloze" => TemplateModelType.Cloze,
            _ => throw new InvalidOperationException($"Template '{templateName}' has invalid anki_model_type '{rawModelType}'. Use 'standard' or 'cloze'.")
        };
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
}
