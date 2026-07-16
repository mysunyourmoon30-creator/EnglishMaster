# Content Quality Module

## Purpose

The Content Quality module provides simple rule-based QA checks for EnglishMaster content. It helps editors and reviewers find missing or incomplete content before publishing.

## Approach

The first version is rule-based only. It does not use AI, advanced NLP, paid services, or external quality engines. Checks inspect existing domain data and create persisted findings.

## Supported Content Types

- Word
- Pronunciation
- GrammarTopic
- GrammarRule
- GrammarExample
- Lesson
- Course
- Book
- Quiz
- Publishing

## Severity Values

- `Info`
- `Warning`
- `Error`
- `Critical`

## Check Status Values

- `Passed`
- `Warning`
- `Failed`
- `NotChecked`

## Score Behavior

Scores are bounded from 0 to 100. Findings apply simple penalties by severity. No findings produce a passing score. Warnings reduce the score but keep the check in `Warning`; errors or critical findings make the check `Failed`.

## Example Checks

- Word: missing IPA creates a warning; missing required meaning creates an error.
- Pronunciation: missing audio creates a warning.
- Grammar: topics should have rules; rules should have examples.
- Lesson: lessons should have sections and related words or grammar rules.
- Course: courses should have lessons.
- Book: books should have chapters.
- Quiz: quizzes should have questions; choice-based questions should have a correct answer.
- Publishing: publish jobs should reference valid source content.

## Why AI QA Is Not Included

AI-based QA requires model selection, prompt design, privacy review, cost controls, human review policies, and repeatability rules. The v0.2.0 system intentionally starts with transparent deterministic checks.
