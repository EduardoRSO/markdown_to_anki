namespace MarkdownToAnki.Tests.Fixtures;

/// <summary>
/// Provides predefined markdown test data as constants.
/// Each test can define its own markdown inline and optionally use these.
/// </summary>
public static class MarkdownFixtures
{
    public static string SimpleDeckYaml => """
deck_name: "Simple Test Deck"
source: "Test Source"
separator: "---"
templates:
  - name: "Basic"
    fields: [Question, Answer]
    html_question_format: "<div>{{Question}}</div>"
    html_answer_format: "<div>{{Answer}}</div>"
    css_format: ".card { }"
""";

    public static string MultiTemplateDeckYaml => """
deck_name: "Multi Template"
source: "Test"
separator: "---"
templates:
  - name: "Concept"
    fields: [Term, Definition]
    html_question_format: "{{Term}}"
    html_answer_format: "{{Definition}}"
    css_format: ""
  - name: "Question"
    fields: [Question, Answer, Explanation]
    html_question_format: "{{Question}}"
    html_answer_format: "{{Answer}}<br>{{Explanation}}"
    css_format: ""
""";

    public static string SingleHeaderMarkdown => """
# Topic 1

```Basic
What is 2+2?
---
4
```
""";

    public static string NestedHeaderMarkdown => """
# Mathematics
## Arithmetic
### Addition

```Basic
What is 1+1?
---
2
```
""";

    public static string NoCardMarkdown => """
# Topic 1
## Subtopic A

No code blocks here.
""";

    public static string MultiCardMarkdown => """
# Languages

```Basic
Hello in English
---
Hello
```

```Basic
Hello in Spanish
---
Hola
```
""";

    public static string SpecialCharactersMarkdown => """
# Spëcïál Çhãrãçtërš
## Àccënts & Dîacritîcs

```Basic
Café in French
---
A coffee shop
```
""";
}
