# Change Log

## [0.0.1] – 2025-09-16
Initial release.

### Added
- User Story: Core Editor Container (Initial Framework)
- Allyaria.Editor:
  - EditorContainer component providing Toolbar, Content (contenteditable, role="textbox"), and Status regions with stable IDs (ae-toolbar, ae-content, ae-status).
  - Sizing parameters: Width (default 400; 0 => 100%) and Height (default 300; 0 => 100%); CSS min size enforced to 220×140.
  - Text binding (bind-Text) with TextChanged event; programmatic updates display immediately.
  - Placeholder parameter shown when empty and announced via aria-describedby.
  - Focus events OnFocus and OnBlur; AutoFocus option.
  - Accessibility & Localization: resx-backed defaults (en-US), AriaLabels overrides, aria-labelledby precedence with safe fallback ensuring non-empty accessible names; NeutralResourcesLanguage set to en-US; strongly-typed resource accessor.
  - Theming: light theme with CSS variables (--ae-bg, --ae-border-color, --ae-content-bg, --ae-text-color, --ae-radius, --ae-spacing).
  - JS interop helpers to read contenteditable innerHTML and sanitize aria-labelledby IDs.
- Allyaria.Editor.Abstractions:
  - AriaLabels record struct public API for ARIA labels and labelled-by IDs.
- Allyaria.Tests.Component:
  - bUnit tests for sizing, binding, events, AutoFocus behavior, role="textbox", ARIA precedence/fallback, placeholder visibility/announcement, and stable IDs.
- Allyaria.Tools.SampleSite:
  - Sample site with basic usage. 
