# Razor Coding Standards (Blazor-focused)

These standards define how we write Razor component files (`.razor`) in this project. Razor files should contain *
*markup and lightweight binding only**. All C# logic belongs in a **code-behind** partial class (`.razor.cs`). **No
inline `<script>` or `<style>`** in Razor files—JavaScript and styles must be **referenced** (via JS modules and
SCSS/CSS files), not embedded.

> *Last updated: 2025-09-12*

## 1. File Layout & Separation of Concerns

* One component per pair:
    * `Component.razor` → markup, parameters, minimal directives.
    * `Component.razor.cs` → partial class with all C# code (lifecycle, methods, state, services).
* **Do not** use `@code`/`@functions` blocks inside `.razor`—move to code-behind.
* **Do not** include `<script>` or `<style>` tags in `.razor`. Reference JS modules and compiled CSS instead.
* Prefer **CSS isolation** with `Component.razor.scss` compiled to `Component.razor.css`. (Author in SCSS if available.)

## 2. Formatting (from editor settings)

* Encoding: **UTF-8**
* Line endings: **LF**
* Final newline: **required**
* Trim trailing whitespace: **yes**
* Indentation: **4 spaces** (no tabs)

> Rationale: Matches common cross-platform tooling defaults, ensures clean diffs, and consistent formatting across
> editors/IDEs.

## 3. Parameters, State, and Naming

* **Parameters**:
    * Use `[Parameter]` for incoming values; `[CascadingParameter]` when appropriate.
    * Two-way binding uses the `@bind-Value` pattern (`Value` + `ValueChanged` + `ValueExpression` for forms).
    * Public parameters are PascalCase; avoid mutable public fields.
* **Private fields**: `_camelCase` (underscore prefix).
* **State**: keep minimal UI state in the component; complex logic belongs in services or state containers.

## 4. Lifecycle & Async Patterns

* Prefer `OnInitializedAsync`/`OnParametersSetAsync` for async work; **never block** with `.Result`/`.Wait()`.
* **Suffix `Async`** on all async methods and **propagate `CancellationToken`** where possible (library-wide rule).
* Don’t call `StateHasChanged()` excessively; batch state updates.
* Cancel or dispose **long-lived resources** in `Dispose`/`DisposeAsync` (e.g., timers, subscriptions, CTS).

## 5. Events, Callbacks, and Binding

* Use `EventCallback`/`EventCallback<T>` for parameters that represent events; don’t expose `Action`/`Func` from
  components.
* Avoid large inline lambdas in Razor that capture significant closure state; prefer small method groups in code-behind.
* Keep bindings explicit and minimal; prefer `@bind-Value:event="oninput"` only when real-time updates are required.

## 6. Dependency Injection & Services

* Inject services in the **code-behind** with `[Inject]`.
* Keep components thin; move business rules to services.
* Prefer **typed HttpClient**, `IStringLocalizer`, and other framework services via DI.

## 7. Routing, Layouts, and Fragments

* One route per page component using `@page`. Non-routable components should **not** declare routes.
* Use `Layout` only in top-level page components; child components inherit layout.
* For templated content, prefer `RenderFragment`/`RenderFragment<T>`. Keep fragments small and stateless.

## 8. JavaScript Interop (Reference Only—no inline scripts)

* Do not embed scripts in `.razor`. Use **ES modules** under `wwwroot/js/` and import via
  `IJSRuntime/IJSObjectReference`.
* Keep JS minimal and focused (see JS standards). Always provide a **dispose** path for event listeners/observers.
* Pass `ElementReference` from Razor to JS instead of global queries.

## 9. Styles (Reference Only—no inline styles)

* No `<style>` tags in `.razor`. Author styles in `Component.razor.scss` (preferred) or `Component.razor.css`.
* Use **CSS isolation** to scope styles to the component.
* Keep selectors shallow; avoid `!important`. For third-party overrides, document and keep as narrow as possible.

## 10. Accessibility & UX

* Use semantic HTML and ARIA roles only when necessary. Prefer native semantics.
* Ensure focus management for dialogs/menus; use JS interop helpers sparingly to enforce focus traps or scroll lock.
* Maintain contrast and keyboard navigation; use `:focus-visible` styles in component CSS.

## 11. Localization

* All user-visible strings should be localizable via `IStringLocalizer` or equivalent.
* Avoid concatenating localized strings inside Razor; prefer formatted resources in code-behind.

## 12. Error Handling

* Surface critical/fatal errors via exceptions in code-behind; render friendly, localized messages in Razor.
* Don’t throw from markup; do it in code-behind and set state accordingly.

## 13. Testing

* Prefer **bUnit** or similar for component tests.
* Keep components deterministic; inject fakes/mocks for services.
* Avoid timing flakiness—await renders and events explicitly.

## 15. Deviations

If a guideline must be violated (interop constraints, performance, third-party markup), document the rationale in
comments near the code and in the PR description, and keep the deviation as small and localized as possible.
