---
deck_name: "Complex Hierarchy Deck"
source: "Test Source"
separator: "---"
media_root: "./media"
templates:
  - name: "Concept"
    anki_model_type: "standard"
    media_files: []
    fields: [Term, Definition, Example]
    html_question_format: "<b>{{Term}}</b>"
    html_answer_format: "{{Definition}}<br><i>{{Example}}</i>"
    css_format: ".card { }"
  - name: "Question"
    anki_model_type: "standard"
    media_files: []
    fields: [Question, Answer, Explanation]
    html_question_format: "{{Question}}"
    html_answer_format: "{{Answer}}<br><small>{{Explanation}}</small>"
    css_format: ".card { }"
---

# Science

## Physics

### Classical Mechanics

```Concept
Newton's First Law
---
An object at rest stays at rest unless acted upon by a force
---
A ball on a table doesn't move unless pushed
```

```Question
What is the SI unit of force?
---
Newton (N)
---
Named after Isaac Newton
```

### Thermodynamics

```Concept
Entropy
---
A measure of disorder in a system
---
Heat naturally flows from hot to cold
```

## Chemistry

### Atomic Theory

```Question
How many electrons does Carbon have?
---
6
---
Carbon has atomic number 6
```
