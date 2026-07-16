---
name: blazor-feature
description: Use for adding, changing, or reviewing Blazor UI features in EnglishMaster, including components, pages, forms, routing, validation, state handling, interaction flows, and integration with ASP.NET Core 9 application services.
---

# Blazor Feature

## Project Context

EnglishMaster uses ASP.NET Core 9 and Blazor. Treat Blazor as the presentation layer of the Clean Architecture and modular monolith design.

## Workflow

1. Inspect the existing `Components` structure, routes, layouts, shared components, styling, and render mode patterns.
2. Place new UI in the nearest existing feature or component folder. Match naming, file organization, and partial class conventions already present.
3. Keep components focused on presentation, user interaction, and view state. Call application services or handlers for business behavior.
4. Use existing form, validation, navigation, and notification patterns before adding new ones.
5. Model UI state explicitly: loading, empty, valid, invalid, saving, success, and failure states when the workflow needs them.
6. Make user actions resilient. Disable duplicate submits, surface validation errors near the related input, and handle service failures without losing entered data.
7. Keep UI text direct and domain-specific. Avoid explanatory text about how the app is built.
8. Preserve accessibility basics: labels for inputs, usable keyboard focus, meaningful button names, and status text for async results when applicable.

## Architecture Rules

- Do not query EF Core directly from `.razor` components.
- Do not put domain rules in event handlers.
- Do not create global state when feature-local state is enough.
- Do not introduce JavaScript interop unless Blazor or existing components cannot handle the interaction.
- Keep reusable components genuinely reusable; otherwise keep them feature-local.

## Verification

- Build the project after changes.
- Run focused tests if component or service behavior is covered.
- For visible UI changes, run the app and inspect the affected page at desktop and mobile widths when feasible.
- Check that text fits, actions are reachable, and no loading or error state traps the user.
