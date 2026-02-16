# Markdown to Anki

Convert Markdown documents into Anki flash card decks automatically.

## What It Does

**Markdown to Anki** is a command-line tool that transforms structured Markdown files into `.apkg` (Anki Package) files. It extracts flashcards from code blocks, organizes them by template type, and automatically applies hierarchical tags based on the document structure.

## Why

Creating flashcards manually is time-consuming and error-prone. This tool enables you to:

- **Write flashcards in Markdown** - Use your favorite text editor and version control system
- **Define multiple card types** - Support different formats (concepts, questions, cloze deletions, etc.)
- **Automatic tag hierarchy** - Tags are derived from your document's heading structure
- **Batch process** - Generate entire decks from a single Markdown file
- **Maintain consistency** - Reuse templates across all your cards

## Features

- **Multiple Template Types**: Define custom card templates (e.g., Conceito, Questão, Omissão)
- **Flexible Card Format**: Use any field structure with custom HTML/CSS styling
- **Hierarchical Tags**: Cards are automatically tagged based on parent headings (normalized to snake_case)
- **UTF-8 Support**: Full Unicode support with automatic accent normalization
- **Custom Separators**: Define how card fields are separated in Markdown

## Installation

1. Clone the repository
2. Navigate to the project directory
3. Build the project:

   ```bash
   dotnet build
   ```

## Usage

### Basic Usage

```bash
dotnet run <input.md>
```

This generates an `.apkg` file in the current directory with a timestamp.

### Specify Output Path

```bash
dotnet run <input.md> <output.apkg>
```

## Markdown Format

Your Markdown file should follow this structure:

```markdown
---
deck_name: "My Deck"
source: "My Study Material"
separator: "---"
templates:
  - name: "Conceito"
    fields: [Pergunta, Resposta, Contexto]
    html_question_format: "<div class='question'>{{Pergunta}}</div>"
    html_answer_format: "<div class='answer'>{{Resposta}}</div>"
    css_format: |
      .card { font-family: arial; font-size: 20px; }
---

# Matemática e Raciocínio Lógico

## Razão e proporção

```Conceito
What is a ratio?
---
A ratio is the relationship between two quantities.
---
Example: 3:5 or 3/5
```

```

### Flashcard Syntax

Flashcards are defined in code blocks with the template name:

```markdown
```[TemplateName]
Field 1
---
Field 2
---
Field 3
```

```

The `---` separator divides card fields. The number of fields should match the template definition.

### Automatic Tags

Tags are automatically generated from the heading hierarchy and normalized:

- **Normalization**: Removes accents, converts to lowercase, replaces spaces with underscores
- **Hierarchy**: Only includes headers in the path to the card

Example:
```markdown
# Matemática e Raciocínio Lógico
## Razão e proporção

```Conceito
...
```

```

This card receives tags: `matematica_e_raciocinio_logico`, `razao_e_proporcao`

## Project Structure

```

MarkdownToAnki/
├── MarkdownToAnki.ConsoleApp/       # CLI entry point
├── MarkdownToAnki.Domain/            # Domain models
└── MarkdownToAnki.Infrastructure/    # Core logic
    └── Services/
        ├── MarkdownParserService.cs  # Markdown parsing
        ├── AnkiGeneratorService.cs   # APKG generation
        └── AnkiTagService.cs         # Tag management

```

## Dependencies

- **AnkiNet**: Library for creating Anki packages
- **YamlDotNet**: YAML parsing for front-matter
- **System.Data.SQLite**: SQLite support for database operations

## Example

See `teoria.md` in the root directory for a complete example with multiple templates and hierarchies.

## License

MIT
