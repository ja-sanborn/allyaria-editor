# Theming.md

Allyaria Editor provides a strongly typed theming API for colors, borders, and backgrounds via `AllyariaTheme`. This
replaces CSS-variable overrides and avoids a separate theming project. Themes are applied by the control at runtime and
update immediately when parameters change.

> Last updated: 2025-09-17

## 1. Theme Types

- System: Honors OS/browser preferences. If high contrast (forced colors) is active, uses `HighContrast`; otherwise uses
  `Dark` or `Light` based on `prefers-color-scheme`.
- Light: WCAG-compliant light preset.
- Dark: WCAG-compliant dark preset.
- HighContrast: Grayscale preset with maximum clarity and contrast.

## 2. Public API

- See Allyaria.Editor.Theming namespace:
    - `ThemeType` enum: System, Light, Dark, HighContrast.
    - `AllyariaTheme` class with properties for Transparent, Outlined, BorderColor, BackgroundImage, region
      backgrounds/foregrounds, and CaretColor.

## 3. Precedence Rules

- Background: Image > Color > Theme > Fallback.
    - If `BackgroundImage` is set (and `Transparent` is false), the container renders:
        - An overlay at 50% opacity (white for Light, black for Dark, theme base for HighContrast).
        - The background image with `cover/center/no-repeat`.
        - Region background colors (toolbar/content/status) are ignored.
    - If `Transparent` is true, no background is applied anywhere; parent background shows through.
- Foreground: Specified > Theme > Fallback.
- Caret: Specified > Theme > Fallback.
- Border: Specified > Theme > Fallback (`Outlined=false` removes border).

## 4. Accessibility

- WCAG 2.2 AA contrast maintained in built-in presets.
- Placeholder text uses 50% opacity of the content foreground color.
- Background images always include a 50% theme-appropriate overlay to preserve readability.
- ARIA labels/roles remain localized and unaffected by theming.

## 5. Runtime Behavior

- Theme changes at runtime update the UI immediately without a reload.
- System theme detection occurs on first render when `ThemeType.System` is selected. Forced colors (high contrast) take
  precedence over dark/light.

## 6. Out of Scope

- Custom per-user theme persistence.
- Animations or transitions between themes.
- Non-color theming (fonts, spacing, etc.).
