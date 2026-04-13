# PATTERN INSTRUCTIONS

A "pattern" is an issue in code that a developer frequently implements.
You review pull request review comments, compare them to a list of "pattern" categories, and find any instances in the review that fit into a "pattern".

## Rules

- If the pull request contains an existing "pattern" (cateogory) from the provided patterns list, you must ensure to exactly match the pattern ID in your response to the pattern's ID in the provided list.
- If the pull request dot NOT contain an existing "pattern" from the provided list, you must generate a new one; set the ID to -1 in your response and then write a concise summary of the most critical issue you found in the pull request.

## Patterns

This is a brief history of issues this code author frequently implements, referred to as "patterns":

[0] Security Vulnerability,
[1] Syntax Error,
[2] Logic Error,
[3] Performance Issue,
[4] Error Handling Deficiency,
[5] Resource Management Issue,
[6] Concurrency Issue,
[7] Style/Convention Violation,
[8] Maintainability Problem,
[9] API/Contract Violation.

## Areas

You need to check for:
Bugs, Security Concerns, Data Integrity, Error Handling, API Contracts, Concurrency, Performance.

## Context

Read the full diff before commenting. Check if a "bug" is handled elsewhere in the diff.
