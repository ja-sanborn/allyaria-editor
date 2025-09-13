# SCSS Coding Standards (Blazor-focused)

**Purpose**
These standards define how we write styles in this Blazor project. We prefer **SCSS** (Sass) as the authoring language because it is more maintainable and modern than raw CSS. SCSS files are compiled to CSS before being served.
Use styles **only where necessary**; prefer Blazor component structure and CSS isolation. Keep styles **scoped, minimal, and maintainable**.

> *Last updated: 2025-09-12*

## 1. Universal Formatting

* Encoding: UTF-8
* Line endings: LF
* Final newline: required
* Trim trailing whitespace: yes
* Indentation: 4 spaces (no tabs)

> Rationale: Matches common cross-platform tooling defaults, ensures clean diffs, and consistent formatting across editors/IDEs.

## 2. Blazor-first Principles

* Use **SCSS isolation** (`Component.razor.scss`) to scope styles per component.
* Avoid global styles except for:
    * design tokens (variables, mixins, functions)
    * base resets (normalize, typography defaults)
    * utilities (visually-hidden, clearfix, etc.)
* Never override Blazor-rendered DOM globally unless necessary (interop with third-party widgets).

## 3. File Organization

* **Component styles** live in `Component.razor.scss`.
* **Global styles** under `wwwroot/scss/` (compiled to `wwwroot/css/`).
* Structure globals by responsibility:
    * `_variables.scss` → design tokens
    * `_mixins.scss` → reusable patterns
    * `_utilities.scss` → helper classes
    * `base.scss` → resets and typography
    * `theme.scss` → colors, dark/light support

Use Sass partials (`_file.scss`) and import them into a main `app.scss`.

## 4. Naming & Class Conventions

* Use **kebab-case** class names: `.product-card`, `.page-header`.
* Use **BEM** when helpful for clarity: `.card`, `.card__title`, `.card--compact`.
* Avoid IDs for styling; use classes.
* Component styles: prefix with component name when possible to avoid leakage.

## 5. Nesting & Specificity

* Limit nesting to **3 levels deep**.
* Use nesting for logical hierarchy, not to replicate DOM depth.
* Keep specificity low: prefer single-class selectors.
* Avoid `!important` except for narrow third-party overrides with justification.

**Example:**

```scss
.card {
    padding: $space-3;
    border-radius: $radius-md;

    &__title {
        font-size: 1.25rem;
        font-weight: 600;
    }

    &--compact {
        padding: $space-2;
    }
}
```

## 6. Variables & Design Tokens

* Define variables in `_variables.scss`.
* Use them consistently for colors, spacing, radii, typography.
* Avoid raw hex values scattered in component files.

**Example:**

```scss
// _variables.scss
$color-bg: #ffffff;
$color-fg: #1b1b1b;
$radius-md: 12px;

$space-2: 8px;
$space-3: 12px;
```

## 7. Theming & Dark Mode

* Use variables for theming; override in `theme.scss`.
* Support `prefers-color-scheme: dark` and `prefers-reduced-motion: reduce`.

```scss
:root {
    --color-bg: #{$color-bg};
    --color-fg: #{$color-fg};
}

@media (prefers-color-scheme: dark) {
    :root {
        --color-bg: #0f1115;
        --color-fg: #e7e9ee;
    }
}
```

## 8. Layout & Spacing

* Use **Flexbox** for 1D layouts, **Grid** for 2D layouts.
* Prefer spacing tokens (`$space-*`) for padding/margins.
* Avoid arbitrary pixel values.

```scss
.toolbar {
    display: flex;
    gap: $space-2;
    align-items: center;
}
```

## 9. Typography

* Define type scale in variables.
* Use relative units (`rem`) for font sizes.
* Line-height between 1.3 and 1.6.

```scss
$font-sm: 0.875rem;
$font-md: 1rem;
$font-lg: 1.25rem;

.title {
    font-size: $font-lg;
    line-height: 1.4;
}
```

## 10. States & Accessibility

* Style `:focus-visible` with a clear outline.
* Maintain WCAG-compliant contrast ratios.
* Ensure hover/active/focus states are consistent.

```scss
.button {
    border-radius: $radius-md;

    &:focus-visible {
        outline: 2px solid currentColor;
        outline-offset: 2px;
    }
}
```

## 11. Motion

* Use transitions sparingly, with short durations.
* Respect user preferences (`prefers-reduced-motion`).
* Prefer CSS transitions over JavaScript animations.

```scss
.modal {
    transition: transform 160ms ease, opacity 160ms ease;
}

@media (prefers-reduced-motion: reduce) {
    .modal {
        transition: none;
    }
}
```

## 12. Utilities

Keep utilities small and global. Examples:

```scss
.visually-hidden {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0 0 0 0);
    white-space: nowrap;
    border: 0;
}
```

## 13. Performance

* Compile SCSS to minified CSS for production.
* Scope rules to reduce unused CSS.
* Favor logical properties (`margin-inline`, `padding-block`) for RTL/LTR support.

## 14. Deviations

* If you must use global CSS or `!important`, document the reason in comments.
* Keep overrides as narrow as possible and refactor when feasible.
