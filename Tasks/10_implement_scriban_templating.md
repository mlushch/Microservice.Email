# Task 10: Implement Scriban Email Templating

## Description
Setup Scriban template engine for dynamic email content generation.

## Requirements
- Install and configure Scriban NuGet package
- Create template loader that reads templates from MinIO storage
- Implement template rendering method that:
  - Accepts template name
  - Accepts template properties (Dictionary)
  - Renders template with provided properties
  - Returns rendered HTML string
- Add template validation (ensure templates are valid Scriban)
- Add caching for frequently used templates
- Create sample email templates (HTML format)
- Add error handling for template not found or invalid syntax

## Acceptance Criteria
- Scriban is configured
- Templates can be loaded from storage
- Template rendering with properties works
- Template validation is implemented
- Caching improves performance
- Sample templates are provided
