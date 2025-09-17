# CSS Coding Standards (Blazor-focused)

Purpose
These standards define how we write styles in this Blazor project using plain CSS. Prefer Blazor component structure and
CSS isolation. Keep styles scoped, minimal, and maintainable. The Allyaria Editor theming is configured via the
AllyariaTheme API rather than CSS variables or overrides.

> Last updated: 2025-09-17

1. Universal Formatting

- Encoding: UTF-8
- Line endings: LF
- Final newline: required
- Trim trailing whitespace: yes
- Indentation: 4 spaces (no tabs)

2. Blazor-first Principles

- Use CSS isolation (`Component.razor.css`) to scope styles per component.
- Avoid global styles except for:
    - base resets (normalize, typography defaults)
    - minimal utilities (visually-hidden, etc.)
- Never inject `<style>` in Razor; reference `.razor.css`.

3. File Organization

- Component styles live in `Component.razor.css`.
- Global styles under `wwwroot/css/` (if any). Keep them small and purposeful.

4. Naming & Class Conventions

- kebab-case classes: `.product-card`, `.page-header`.
- Use BEM when helpful: `.card`, `.card__title`, `.card--compact`.
- Avoid IDs for styling; use classes.

5. Specificity & Overrides

- Keep selectors shallow; avoid `!important` (only narrow third-party overrides when necessary).
- Prefer inline styles set by components for dynamic theming (see Theming.md).

6. Layout & Spacing

- Use Flexbox for 1D layouts, Grid for 2D layouts.
- Prefer spacing tokens via CSS custom properties or shared variables where static. Dynamic colors come from
  AllyariaTheme.

7. States & Accessibility

- Style `:focus-visible` with a clear outline.
- Maintain WCAG 2.2 AA contrast.
- Do not convey information by color alone.

8. Motion

- Keep transitions small and respect `prefers-reduced-motion`.

9. RTL & Logical Properties

- Prefer logical properties (`margin-inline`, `padding-block`) for bidi support.

10. Theming

- Colors, borders, and background images for Allyaria Editor are controlled by the `AllyariaTheme` object.
- Do not rely on CSS overrides for theme; the component applies inline styles with strict precedence.
