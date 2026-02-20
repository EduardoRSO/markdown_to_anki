---
deck_name: "Edge Cases Test"
source: "Test"
separator: "||"
media_root: "./media"
templates:
  - name: "Special"
    anki_model_type: "standard"
    media_files: []
    fields: [Field1, Field2, Field3]
    html_question_format: "{{Field1}}"
    html_answer_format: "{{Field2}}<br>{{Field3}}"
    css_format: ""
---

# Special Characters & Accents

```Special
Diacrítics: àáâäãåèéêë
||
The quick brown fox
||
Multiple lines
are supported
```

# Empty Content Test

```Special
Question with empty answer
||

||
Third field
```

# Multiline Content

```Special
Multi-line question
with multiple sentences
||
Multi-line answer
spanning several
lines of text
||
And explanation
```
