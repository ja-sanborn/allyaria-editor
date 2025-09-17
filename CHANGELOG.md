# Change Log

## [0.0.2] – 2025-09-17

Introduces a strongly typed theming API with light/dark/high-contrast support, runtime updates, and CSS-only standards.

### Added

- Public theming API: AllyariaTheme and ThemeType (Light, Dark, HighContrast, System).
- Runtime theme application with strict precedence for the background, foreground, caret, and border.
- System/high-contrast detection with automatic application when ThemeType.System is used.
- New documentation: Theming.md and CSS.md (CSS-only guidance).

### Removed

- SCSS.md standards and SCSS-based theming guidance.
- Theme-related projects and folders.

### Fixed

- Placeholder visibility and background-image overlay now ensure WCAG 2.2 AA contrast.

## [0.0.1] – 2025-09-16

Initial release.

### Added

- User Story: Core Editor Container (Initial Framework)
- Allyaria.Editor:
    - EditorContainer component providing Toolbar, Content (contenteditable, role="textbox"), and Status regions with
      stable IDs (ae-toolbar, ae-content, ae-status).
    - Sizing parameters: Width (default 400; 0 => 100%) and Height (default 300; 0 => 100%); CSS min size enforced to
      220×140.
    - Text binding (bind-Text) with TextChanged event; programmatic updates display immediately.
    - Placeholder parameter shown when empty and announced via aria-describedby.
    - Focus events OnFocus and OnBlur; AutoFocus option.
    - Accessibility & Localization: resx-backed defaults (en-US), AriaLabels overrides, aria-labelledby precedence with
      safe fallback ensuring non-empty accessible names; NeutralResourcesLanguage set to en-US; strongly-typed resource
      accessor.
    - Theming: light theme with CSS variables (--ae-bg, --ae-border-color, --ae-content-bg, --ae-text-color,
      --ae-radius, --ae-spacing).
    - JS interop helpers to read contenteditable innerHTML and sanitize aria-labelledby IDs.
- Allyaria.Editor.Abstractions:
    - AriaLabels record struct public API for ARIA labels and labelled-by IDs.
- Allyaria.Tests.Component:
    - bUnit tests for sizing, binding, events, AutoFocus behavior, role="textbox", ARIA precedence/fallback, placeholder
      visibility/announcement, and stable IDs.
- Allyaria.Tools.SampleSite:
    - Sample site with basic usage. 
