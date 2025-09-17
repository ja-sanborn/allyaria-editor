# Accessibility Standards (Blazor-focused: C#, Razor, JavaScript, SCSS)

Make every UI usable with screen readers, keyboard, and assistive tech. These standards align with WCAG 2.2 AA and apply
across our stack: C#, Razor, JavaScript, and SCSS. Favor semantic HTML, robust keyboard support, localized content, and
reduced visual/interaction friction.

> *Last updated: 2025-09-12*

## 1. Core Principles

* **Perceivable:** Provide text alternatives, proper structure, and sufficient contrast.
* **Operable:** Everything works via **keyboard only**; clear focus order and visible focus.
* **Understandable:** Consistent patterns, labels match visible text, helpful errors.
* **Robust:** Semantic HTML first; ARIA only if necessary; test with AT (NVDA/JAWS/VoiceOver).

## 2. Razor (Markup) Standards

### 2.1 Semantic structure & landmarks

* Use native elements first: `<button>`, `<a>`, `<label>`, `<input>`, `<table>`, `<ul>`, `<nav>`, `<main>`, `<header>`,
  `<footer>`, `<section>`, `<aside>`.
* One `<h1>` per page route; follow heading order (no skipping levels).
* Identify landmarks: `<main>`, `<nav>`, `<header>`, `<footer>`, `<aside>`.

```razor
<main id="main">
    <h1>@PageTitle</h1>
    <nav aria-label="Breadcrumb">…</nav>
    <section aria-labelledby="results-heading">
        <h2 id="results-heading">Results</h2>
        …
    </section>
</main>
```

### 2.2 Forms & labels

* Every input must have a **label**. Use `<label for>` or wrap label around input.
* Group related controls with `<fieldset>` + `<legend>`.
* Provide **help text** via `aria-describedby` if not inline.

```razor
<label for="email">Email</label>
<InputText id="email" @bind-Value="Model.Email" aria-describedby="email-help" />
<div id="email-help" class="help">We’ll never share your email.</div>
```

### 2.3 Errors & validation

* Associate errors with controls via `aria-describedby` and ensure they’re **programmatically determinable**.
* Use summary region with `role="alert"` (or `aria-live="assertive"`) for form-wide errors.

```razor
<div role="alert" class="validation-summary" aria-live="assertive">
    @if (EditContext.GetValidationMessages().Any()) { <ul>…</ul> }
</div>
```

### 2.4 Interactive controls

* Prefer native controls. If you must build custom widgets, follow WAI-ARIA Authoring Practices (APG) patterns and
  roles, manage focus, and support keyboard interaction.

## 3. C# (Components & Logic)

### 3.1 Focus management

* After route change, set focus to the main heading or a sentinel element.
* On dialogs/menus, **move focus into** the container on open and **return focus** to the opener on close.

```csharp
// Example: after content updates
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await _jsModule!.InvokeVoidAsync("focusFirstHeading");
    }
}
```

### 3.2 Announcements (live regions)

* For async updates, announce changes via live regions rather than moving focus.

```razor
<div aria-live="polite" class="sr-only" @ref="_liveRegionRef"></div>
```

```csharp
await JS.InvokeVoidAsync("announce", "Loaded 10 more results.");
```

### 3.3 Validation & messages

* Return localized, concise errors. Don’t rely on color alone to indicate state; set boolean states and expose them to
  markup (e.g., `aria-invalid="true"`).

## 4. JavaScript (Interop) Standards

> JS should be minimal. Use only when necessary for focus, measurements, or third-party widgets.

### 4.1 Keyboard & focus

* Never trap focus unintentionally. When adding key handlers, **don’t override** native behavior without accessible
  equivalents.
* Expose **init**/**dispose** functions and honor cancellation/disposal (for Blazor component teardown).

```js
// focus-helpers.js
export function focusFirstHeading() {
    const target = document.querySelector('main h1, main h2, [role="heading"]');
    target?.focus?.();
}

export function announce(message) {
    let region = document.getElementById('live-region');
    if (!region) {
        region = Object.assign(document.createElement('div'), { id: 'live-region' });
        region.setAttribute('aria-live', 'polite');
        region.className = 'sr-only';
        document.body.appendChild(region);
    }
    region.textContent = '';
    setTimeout(() => { region.textContent = message; }, 10);
}
```

### 4.2 ARIA & roles

* Only add ARIA when semantic HTML cannot express the role/state. Keep roles/states in **sync** with component state (
  expanded, selected, busy, modal).

### 4.3 Motion

* Respect `prefers-reduced-motion`. Avoid JS-driven parallax or large animations; if necessary, provide a user setting
  and honor OS preference.

## 5. SCSS (Styles) Standards

### 5.1 Focus visibility

* Use `:focus-visible` for keyboard focus. Never remove outlines without providing a clear replacement.

```scss
:focus-visible {
    outline: 2px solid currentColor;
    outline-offset: 2px;
}
```

### 5.2 Contrast & color

* Maintain WCAG 2.2 AA contrast: text 4.5:1 (3:1 for large text ≥ 18.66px regular / 14px bold).
* Don’t convey information by color alone; add icons, text, or patterns.

```scss
// Error + icon + text (not color alone)
.input--error { border-color: var(--color-danger); }
.input--error::after { content: "⚠"; margin-inline-start: .5rem; }
```

### 5.3 Motion & reduced motion

* Keep durations short. Respect user preference.

```scss
* {
    transition: color 120ms ease, background-color 120ms ease;
}
@media (prefers-reduced-motion: reduce) {
    * { transition: none !important; animation: none !important; }
}
```

### 5.4 Hit targets & spacing

* Ensure clickable areas meet minimum size (\~44×44 CSS px) via padding.
* Use logical properties (`margin-inline`, `padding-block`) for better RTL/LTR support.

---

## 6. Images, Media, Icons

* **Images:** Always include `alt`. If decorative, `alt=""`.
* **SVG icons:** Provide `role="img"` and `<title>` or use `aria-hidden="true"` if purely decorative.

```razor
<!-- Informative SVG -->
<svg role="img" aria-labelledby="title-ok" width="16" height="16">
  <title id="title-ok">Success</title>
  …
</svg>

<!-- Decorative SVG -->
<svg aria-hidden="true" focusable="false" …>…</svg>
```

* **Video/Audio:** Provide captions, transcripts, and controls. Avoid autoplay.

## 7. Tables & Data

* Use `<table>` only for **tabular data** (not layout).
* Provide headers with `<th scope="col|row">`.
* For complex tables, associate headers with `headers`/`id` and consider `<caption>`.

```razor
<table>
  <caption class="sr-only">Quarterly sales</caption>
  <thead>
    <tr><th scope="col">Region</th><th scope="col">Q1</th></tr>
  </thead>
  <tbody>
    <tr><th scope="row">EMEA</th><td>…</td></tr>
  </tbody>
</table>
```

## 8. Dialogs, Menus, and Overlays

* Use proper roles and manage focus:

    * **Dialog:** `role="dialog"`/`aria-modal="true"`, labelled by a heading.
    * **Menu:** arrow-key navigation, `aria-activedescendant` or roving `tabindex`.
* Trap focus **only while open**; return focus to the trigger on close.
* Prevent background scroll and ensure escape key closes the dialog.

## 9. Localization & Language

* Provide localized strings for **all** user-visible text and errors.
* Set page or component language via `lang`. For mixed-language content, annotate spans with `lang="…"`.
* Provide **direction** where necessary (`dir="rtl"`), and prefer logical CSS properties.

## 10. Timeouts & Auto-updates

* Avoid unexpected timeouts. If auto-refreshing content, do not steal focus; announce via live region if meaningful.
* Provide user controls to pause/stop auto-rotating carousels or animations.

## 11. Keyboard Interaction Patterns (Quick Reference)

* **Buttons:** `<button>`; Space/Enter activate.
* **Links:** `<a href>`; Enter activates. Use button if no navigation occurs.
* **Checkboxes/Radio:** Arrow keys (radio groups), Space toggles.
* **Menu/Combo:** Arrow keys to move, Enter/Space to select, Escape closes.
* **Dialog:** Trap focus inside; Escape closes; Return focus to opener.

## 12. Testing & Tooling

* **Manual checks:**
    * Keyboard-only nav through the UI (Tab/Shift+Tab/Arrow).
    * Screen reader sanity passes (NVDA/JAWS on Windows, VoiceOver on macOS).
    * Zoom to 200%+ and re-check layout/overflow.
* **Automated checks (integrate in CI):**
    * axe-core / Playwright axe scans (component and page level).
    * ESLint plugin jsx-a11y if using JSX/TSX (rare here).
    * Unit tests for focus behavior and ARIA attributes on custom widgets.
* **Color/contrast:**
    * Validate token palettes with contrast checkers.
    * Snapshot themes in light/dark with common backgrounds.

## 13. Common Anti-Patterns to Avoid

* Clickable `<div>`/`<span>` without role/button semantics and key handling.
* Removing focus outlines globally.
* Using ARIA where semantic HTML suffices.
* Color-only state indication (error/success).
* Modals without focus management or escape handling.
* Infinite scroll without programmatic announcement or landmarks.

## 14. Example: Accessible Dialog Pattern (Razor + C# + JS + SCSS)

```razor
<!-- Razor -->
<button @onclick="OpenAsync" aria-haspopup="dialog" aria-controls="dlg1">Open dialog</button>

<div id="dlg1" role="dialog" aria-modal="true" aria-labelledby="dlg1-title" hidden>
  <h2 id="dlg1-title" tabindex="-1">Settings</h2>
  …
  <button @onclick="CloseAsync">Close</button>
</div>
```

```csharp
// Code-behind (simplified)
private ElementReference _dialogEl;
private IJSObjectReference? _js;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
        _js = await JS.InvokeAsync<IJSObjectReference>("import", "/js/dialog.js");
}

protected Task OpenAsync() => _js!.InvokeVoidAsync("openDialog", _dialogEl).AsTask();
protected Task CloseAsync() => _js!.InvokeVoidAsync("closeDialog", _dialogEl).AsTask();
```

```js
// dialog.js (excerpt)
const active = new WeakMap();

export function openDialog(el) {
    const opener = document.activeElement;
    el.hidden = false;
    active.set(el, opener);
    el.querySelector('[tabindex="-1"], h1, h2, h3, button, [href], input, select, textarea')?.focus();
    document.addEventListener('keydown', esc, true);
    function esc(e){ if(e.key==='Escape') closeDialog(el); }
    el.__esc = esc;
}

export function closeDialog(el) {
    el.hidden = true;
    document.removeEventListener('keydown', el.__esc, true);
    const opener = active.get(el);
    opener?.focus?.();
    active.delete(el);
}
```

```scss
/* SCSS focus & reduced motion */
[role="dialog"] {
    outline: none;
}
[role="dialog"]:focus-visible {
    outline: 2px solid currentColor;
    outline-offset: 2px;
}

@media (prefers-reduced-motion: reduce) {
    [role="dialog"] { transition: none; animation: none; }
}
```

## 15. Governance

* **Definition of Done** includes keyboard nav, focus visibility, screen reader pass, and contrast checks.
* New components must include an accessibility section in PR description: semantics, keyboard behavior, focus
  management, and testing notes.
* Report and track a11y issues with severity; do not block ship on non-critical cosmetic issues, but never waive
  keyboard/critical blockers.

### Quick Checklist (per component)

* [ ] Headings and landmarks used correctly
* [ ] Keyboard-only operation (Tab/Shift+Tab/Arrow/Escape)
* [ ] Visible focus indicators
* [ ] Labels and help text; errors associated with inputs
* [ ] Sufficient color contrast
* [ ] No color-only meaning
* [ ] Live announcements for async updates (if applicable)
* [ ] Motion respects reduced-motion
* [ ] Localization-ready strings
* [ ] Tests: manual + automated axe scan (where applicable)

---

> Adhering to these standards ensures our Blazor UI is inclusive, resilient, and consistent for all users.
