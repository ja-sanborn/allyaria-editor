# C# Coding Standards

These standards codify our team’s C# practices. They follow Microsoft’s recommended guidelines for .NET and add
project-specific rules for consistency, documentation, reliability, and usability of our library code.

> *Last updated: 2025-09-17*

## 1. Universal Formatting

- **Encoding:** UTF-8
- **Line endings:** LF
- **Final newline:** Required
- **Trim trailing whitespace:** Yes (except in Markdown)
- **Indentation:** 4 spaces (no tabs)

> Rationale: Matches common cross-platform tooling defaults, ensures clean diffs, and consistent formatting across
> editors/IDEs.

## 2. Language Version, Nullable, & General Project Settings

- **C# version:** Modern language version (C# 12+ recommended) to support `file` and `required`.
- **Nullable reference types:** **Enabled** in all projects.
- **Implicit usings:** Team choice; prefer explicit usings in libraries for clarity.
- **XML documentation file:** **Generated** in all projects (see §9).

## 3. Namespaces & File Layout

* **File-scoped namespaces** are preferred.
* One **type** per file.
* Organize members: fields → constructors → properties → methods → operators → events → nested types.

**Example:**

```csharp
namespace AllyariaEditor.Widgets;

public sealed class Widget
{
    // fields → constructors → properties → methods...
}
```

## 4. Modifier Order

Use this exact order (aligned with Microsoft guidance and our preference):

* public
* private
* protected
* internal
* file
* new
* static
* abstract
* virtual
* sealed
* readonly
* override
* extern
* unsafe
* volatile
* async
* required

## 5. Braces, Blocks, and Expression Bodies

* **Braces are required** on all control statements—even single-line statements.
* **Expression-bodied members** are encouraged for concise accessors, constructors, methods, and properties (keep
  readable).

```csharp
// ✔️ Braces required
if (isReady)
{
    DoWork();
}

// ✔️ Expression-bodied (concise)
public override string ToString() => $"{Name} ({Id})";
```

## 6. Type Usage & `var`

* Prefer `var` **everywhere** (built-in types, when apparent, and elsewhere).
* Prefer **predefined types** (`int`, `string`, `bool`, etc.) in declarations and member access.

```csharp
var items = new List<string>();
int count = 0;
var name = string.Empty;
```

## 7. Qualification & Usings

* Do **not** require `this.` or type qualification for fields, properties, methods, or events.
* **System-first usings:** **Not used** (by team choice). Grouping of usings is optional; be consistent within a
  project.

## 8. Readability: Parentheses & Clarity

* **Arithmetic/relational:** Avoid redundant parentheses.
* **Other binary operators:** Use parentheses when they materially improve clarity.

## 9. XML Documentation Comments: **Required**

> **Policy:** XML documentation comments are **required for all public, protected, internal, and private** types and
> members: fields, properties, constructors, methods, events, operators, and delegates.

* Documentation must state purpose, behavior, contracts, side effects, and noteworthy exceptions.
* Use `<summary>`, `<param>`, `<returns>`, `<remarks>`, `<example>`, and `<exception>` appropriately.
* Keep docs succinct, correct, and maintained alongside code changes.

**Example:**

```csharp
/// <summary>
/// Represents a domain widget.
/// </summary>
public sealed class Widget
{
    /// <summary>
    /// Unique identifier for this widget.
    /// </summary>
    private readonly int _id;

    /// <summary>
    /// Creates a widget with an identifier.
    /// </summary>
    /// <param name="id">A positive identifier.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="id"/> is not positive.
    /// </exception>
    public Widget(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        _id = id;
    }

    /// <summary>Gets or sets the widget’s display name.</summary>
    public required string Name { get; init; }

    /// <summary>Builds a user-visible label.</summary>
    /// <returns>A human-readable label.</returns>
    public string BuildLabel() => $"{Name} (#{_id})";
}
```

## 10. Naming Conventions

* **PascalCase:** public/protected/internal types, methods, properties, events, constants.
* **\_camelCase:** **private fields** (required underscore prefix).
* **camelCase:** locals and parameters.
* **Async methods:** end with **`Async`** (see §11).
* **Interfaces:** `I` prefix (e.g., `IService`).
* **Type parameters:** `T` prefix (e.g., `TItem`).
* **Acronyms:** Two-letter acronyms stay uppercase (`IO`), longer acronyms are Pascalized (`XmlReader`, not
  `XMLReader`).

## 11. Asynchronous Code

* **All async methods** (public, protected, internal, private) must:
    * End with **`Async`**.
    * Accept a **`CancellationToken`** parameter (last parameter). Prefer a default of `default`.
* Always **honor and propagate** the cancellation token.
* Prefer `Task`/`Task<T>`; avoid `async void` except for event handlers.
* Name tasks/methods to reflect their asynchronous behavior and **avoid sync-over-async** patterns.

```csharp
public async Task<Result> FetchItemsAsync(int page, CancellationToken cancellationToken = default)
{
    // Pass the token down to all awaited calls
    using var response = await _http.GetAsync(BuildUrl(page), cancellationToken);
    // ...
}
```

> Library guidance: In modern .NET, `ConfigureAwait(false)` is typically unnecessary unless you explicitly need to avoid
> a captured context in legacy scenarios.

## 12. Exceptions & Error Handling

* **Exceptions are for fatal/critical situations** only (library policy).
    * For expected/recoverable outcomes, prefer result types (e.g., `OneOf`, `Result<T>`) or documented return
      contracts.
* Throw the **most specific** exception type; avoid leaking sensitive/internal details.
* Use **guard clauses** for parameter validation.
* **Localization is required** for all user-visible messages (see §13).
* Do not use exceptions for flow control; avoid large try/catch blocks that obscure logic.

```csharp
public static string RequireNonEmpty(string input, string paramName)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException(Localize("Error_EmptyValue"), paramName);
    return input;
}
```

## 13. Globalization & Localization

* All **user-facing strings must be localized** (resource-based).
* Use culture-aware formatting (`ToString(CultureInfo)`, `string.Format` with culture, or `FormattableString.Invariant`
  where appropriate).
* Avoid string concatenation for user messages; prefer localized **composite** resources.

## 14. Immutability & Initialization

* Prefer **readonly** fields and immutable types for thread-safety and clarity.
* Use `required` members for construction contracts where appropriate.
* Prefer object initializers; keep them concise and documented.

## 15. Testing Conventions

This section defines conventions for **xUnit**, **bUnit**, **AwesomeAssertions** (FluentAssertions fork), and *
*NSubstitute**.

### 15.1 General test guidance

* **Structure:** Arrange / Act / Assert (comment the sections).
* **Naming:** `Method_Scenario_Expected` (or `Behavior_Scenario_Expected` for component tests).

    * Example: `RecomputeStyles_WithBackgroundImage_UsesOverlayAndTransparentRegions`
* **Determinism:** No randomness, time sleeps, or environment dependencies.
* **Culture/time:** If relevant, set `CultureInfo.CurrentCulture/CurrentUICulture` within a test and restore in
  `finally`.
* **Parallelization:** Default parallel test execution is OK; use `[Collection]` only when you truly need shared state.
* **What goes where:**

    * **bUnit (component tests):** rendering, parameters, markup/DOM, events, interop wiring, CSS/output strings.
    * **xUnit (unit tests):** pure logic (helpers, theming math, mappers), small services without rendering.
    * Avoid duplicating the same assertion in both layers.

### 15.2 xUnit (plain unit tests)

* Use `[Fact]` for fixed inputs; `[Theory]` + `[InlineData]` for table-driven coverage.
* Prefer **small focused tests** over large end-to-end checks.
* Use **AwesomeAssertions** for expressive assertions (see §15.4).
* Examples (EditorUtils):

```csharp
[Fact]
public void DefaultOrOverride_WhenNullOrWhitespace_ReturnsFallback()
{
    EditorUtils.DefaultOrOverride("  ", "fallback").Should().Be("fallback");
}

[Fact]
public void Style_WhenValueProvided_BuildsCssDeclaration()
{
    EditorUtils.Style("color", "#fff").Should().Be("color: #fff;");
}
```

* For theming:

    * Verify `GetDefaults(AeThemeType.Dark)` returns expected anchor values (e.g., content background, border, overlay).
    * If style computation is extracted (recommended), unit test precedence:

        * Transparent clears backgrounds
        * Background image applies overlay + transparent regions
        * Explicit overrides beat defaults
        * Border on/off logic
        * Width/Height `0` → `100%`

### 15.3 bUnit (component tests)

* **Test base:** derive test classes from `TestContext`.
* **JS interop:** use bUnit’s `JSInterop`:

    * `JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult("")`
    * Avoid referencing concrete `JSRuntime`; rely on `IJSRuntime` provided by the test DI container.
* **Helpers:** prefer shared helpers (e.g., `JsInteropSetups`) to keep tests terse and consistent.
* **Rendering:** `RenderComponent<T>(ps => ps.Add(p => p.Param, value))`
* **Assertions:** prefer stable selectors (`#id`, `.class`), and assert **intent**, not incidental details.

    * For inline styles, assert key substrings rather than whole-string equality to avoid ordering brittleness.
* **Events:** trigger with `TriggerEventAsync` and assert resulting state/parameters (`TextChanged`, `OnFocus`, etc).
* **System theme detection:** set up the interop to return `"hc"`, `"dark"`, etc., and assert resulting styles.

```csharp
[Fact]
public void Placeholder_Shown_And_Announced_When_Text_Empty()
{
    JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult("");

    var cut = RenderComponent<AllyariaContent>(ps => ps
        .Add(p => p.Text, string.Empty)
        .Add(p => p.Placeholder, "Start typing...")
    );

    cut.Find("#ae-placeholder").TextContent.Trim().Should().Be("Start typing...");
    cut.Find("#ae-content").GetAttribute("aria-describedby").Should().Contain("ae-placeholder");
}
```

### 15.4 Assertions with AwesomeAssertions (FluentAssertions fork)

* Prefer **fluent assertions** for readability and helpful failure messages.
* Typical patterns:

    * Strings: `.Should().Be(...)`, `.Contain(...)`, `.NotBeNullOrEmpty()`, `.MatchRegex(...)`
    * Objects: `.BeEquivalentTo(...)` for structural comparison
    * Booleans: `.BeTrue()`, `.BeFalse()`
    * Collections: `.HaveCount(...)`, `.Contain(...)`, `.OnlyContain(...)`
* Examples:

```csharp
style.Should().Contain("background-color: #ffffff");
updated.Should().Be("Hello world");
elements.Should().HaveCount(3);
```

> Note: AwesomeAssertions mirrors FluentAssertions APIs; keep assertions **expressive** and **precise**.

### 15.5 Test doubles with NSubstitute

* Use **NSubstitute** for interface stubs/mocks when plain fakes are noisy.
* Typical flow:

    * `var interop = Substitute.For<IEditorJsInterop>();`
    * `interop.SanitizeLabelledByAsync("id").Returns("id");`
    * Pass the substitute into the component or service under test.
    * Verify via `Received()`/`DidNotReceive()`:

```csharp
interop.Received(1).SanitizeLabelledByAsync(Arg.Is<string>(s => s.Contains("heading")));
```

* Prefer substitutes for **behavioral verification**; otherwise, stub only what the test needs.

### 15.6 Test project layout

```
tests/
  Allyaria.Tests.Component/         // bUnit
    Content/...
    Editor/...
    Toolbar/...
    Helpers/JsInteropSetups.cs
  Allyaria.Tests.Unit/              // xUnit (pure logic)
    Helpers/EditorUtilsTests.cs
    Theming/ThemeDefaultsTests.cs
    Theming/StyleEngineTests.cs     // if style math is extracted
    Resources/EditorResourcesTests.cs
```

* Mirror `src/` folder names to keep navigation easy.
* Keep helper extensions (e.g., `GetRequiredJsRuntime`, interop setups) in a shared `Helpers` folder.

### 15.7 What to **not** test

* Don’t assert private implementation details (e.g., exact order of concatenated CSS when intent can be asserted).
* Don’t duplicate the same checks across bUnit and xUnit—choose the **most appropriate layer**.
* Don’t serialize/deserialize for the sake of testing unless the API contract demands it.

### 15.8 Build & CI

* All test projects must build and run in CI with `dotnet test`.
* Aim for **fast** unit tests; component tests are allowed to be slower but should still be concise.

## 16. Deviations

Where Microsoft guidance differs from these standards (e.g., `System`-first usings), this document’s rules take
precedence for our codebase. Propose changes via PR for team discussion.
