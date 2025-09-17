# C# Coding Standards

These standards codify our team’s C# practices. They follow Microsoft’s recommended guidelines for .NET and add
project-specific rules for consistency, documentation, reliability, and usability of our library code.

> *Last updated: 2025-09-12*

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

* Unit test names follow: **`Method_Scenario_Expected`**
  Example: `Create_WhenNameMissing_ThrowsArgumentException`
* Tests should be deterministic, isolated, and explicit about Arrange/Act/Assert sections.

## 16. Deviations

Where Microsoft guidance differs from these standards (e.g., `System`-first usings), this document’s rules take
precedence for our codebase. Propose changes via PR for team discussion.
