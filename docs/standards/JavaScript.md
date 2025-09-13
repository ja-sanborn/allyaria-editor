# JavaScript Coding Standards (Blazor-focused)

JavaScript should be avoided unless absolutely necessary, and kept as minimal as possible to achieve the required functionality. Prefer C# / Razor and .NET libraries first. Use JavaScript only when platform/browser APIs are not available in .NET or when integrating a third-party widget that requires JS.

> *Last updated: 2025-09-12*

## 1. Scope & Principles

* Use JS **only** for:
    * Access to browser APIs not covered by .NET/Blazor.
    * Lightweight DOM interactions that cannot be expressed via Blazor.
    * Integrations with third-party scripts/components.
* Keep each JS routine focused, deterministic, and side-effect minimal.
* Prefer small, self-contained ES modules callable via Blazor JS interop.
* Clean up all resources you create (events, timers, observers).

## 2. Minimal Formatting Defaults (from our editor settings)

* Encoding: UTF-8
* Line endings: LF
* Final newline: required
* Trim trailing whitespace: yes
* Indentation: 4 spaces (no tabs)

> Rationale: Matches common cross-platform tooling defaults, ensures clean diffs, and consistent formatting across editors/IDEs.

## 3. Language Level & Modules

* Use modern ECMAScript and **ES modules** (`import`/`export`).
* One responsibility per module; keep files short.
* File names: kebab-case (e.g., `scroll-lock.js`).
* Avoid globals; export functions explicitly.

## 4. Naming Conventions

* Functions/variables: `camelCase` (e.g., `attachScrollLock`).
* Classes (rare): `PascalCase`.
* Constants: use `const` with `camelCase` names; reserve UPPER\_SNAKE\_CASE only for true compile-time constants.

## 5. Interop Contract (Blazor ⇄ JS)

* Export **small, pure** functions; pass/return plain JSON-serializable values.
* Prefer `ElementReference` from .NET instead of querying the DOM globally.
* Always provide a way to **dispose** or **cancel** long-lived work (events, observers, timers).
* Surface failures via rejected promises; don’t swallow errors.

Example (module pattern with init/dispose)

```js
// wwwroot/js/focus-trap.js
const traps = new WeakMap();

export function initFocusTrap(element) {
    if (!element) throw new Error('Element is required');
    const handler = (e) => {
        if (!element.contains(document.activeElement)) {
            element.focus();
        }
    };
    document.addEventListener('focusin', handler);
    traps.set(element, handler);
    return true;
}

export function disposeFocusTrap(element) {
    const handler = traps.get(element);
    if (handler) {
        document.removeEventListener('focusin', handler);
        traps.delete(element);
    }
    return true;
}
```

## 6. Syntax & Style (keep it simple)

* Use semicolons consistently.
* Prefer `const`, then `let`; never `var`.
* Use strict equality (`===` / `!==`).
* Use optional chaining (`?.`) and nullish coalescing (`??`) where it improves clarity.
* Early-return to avoid deep nesting.

## 7. DOM Operations (minimal and safe)

* Operate only within the element(s) passed in from .NET.
* Avoid manipulating Blazor-owned markup in ways that fight the renderer.
* Always unregister listeners and observers in a `dispose` function.
* Avoid layout thrashing; batch reads/writes if needed.

Example (event lifecycle)

```js
// wwwroot/js/click-outside.js
const handlers = new WeakMap();

export function attach(el) {
    const onDocClick = (e) => {
        if (el && !el.contains(e.target)) {
            el.dispatchEvent(new CustomEvent('blazor:click-outside', { bubbles: true }));
        }
    };
    document.addEventListener('click', onDocClick, true);
    handlers.set(el, onDocClick);
}

export function detach(el) {
    const h = handlers.get(el);
    if (h) {
        document.removeEventListener('click', h, true);
        handlers.delete(el);
    }
}
```

## 8. Async, Cancellation, and Long-Running Work

* Prefer `async`/`await`. Always handle rejections (return a rejected promise on error).
* For browser APIs that support it, accept an `AbortSignal` and honor cancellation.
* Do not leave “floating” promises or intervals running after component disposal.

Example (abortable fetch helper)

```js
export async function fetchJson(url, { signal } = {}) {
    const res = await fetch(url, { signal });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    return res.json();
}
```

## 9. Errors, Logging, and Diagnostics

* Throw `Error` (or subclasses) with actionable messages; avoid leaking sensitive data.
* Keep logging out of library JS; surface information to .NET if needed.
* Fail fast for invalid inputs; validate at the boundary.

## 10. Security

* Never inject unsanitized HTML (avoid `innerHTML` with untrusted data).
* Avoid `eval`, `new Function`, and dynamic script construction.
* Prefer attribute/property assignment over string concatenation for DOM updates.
* Respect CSP; do not disable it for convenience.

## 11. Testing (only where it adds value)

* For tiny interop utilities, rely on component-level tests in .NET where practical.
* If needed, write small, deterministic unit tests (e.g., Jest) focused on pure functions.

Example (guard function)

```js
export function requireNonEmpty(value, name = 'value') {
    if (typeof value !== 'string' || value.trim() === '') {
        throw new Error(`${name} must be a non-empty string`);
    }
    return value;
}
```

## 12. Packaging & Placement

* Place JS modules under `wwwroot/js/` and reference them via `<script type="module">` or import maps.
* No bundler/transpiler unless absolutely required by a dependency.
* Version cache-busting via static file options or query strings managed on the .NET side.

## 13. Deviations

If a rule must be violated (interop constraints, performance, or third-party requirements), document the rationale in comments near the code and in the PR description, and keep the deviation as narrow as possible.
~~~~
