namespace Allyaria.Editor.Helpers;

/// <summary>
/// Utility helpers for the Allyaria editor, including ARIA attribute construction, value fallback logic, and small CSS
/// string builders.
/// </summary>
internal static class EditorUtils
{
    /// <summary>
    /// Builds ARIA attributes using precedence rules for labeling:
    /// <list type="number">
    ///     <item>
    ///         <description>Use non-empty <c>aria-labelledby</c> when <paramref name="labelledByResolved" /> is provided.</description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Otherwise, use a non-whitespace override via <c>aria-label</c> from <paramref name="overrideLabel" />.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>Otherwise, fall back to <paramref name="fallback" /> via <c>aria-label</c>.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="labelledByResolved">A sanitized, space-separated list of element IDs (maybe empty).</param>
    /// <param name="overrideLabel">An optional override label to apply via <c>aria-label</c> when provided and not whitespace.</param>
    /// <param name="fallback">
    /// The fallback label used when neither <paramref name="labelledByResolved" /> nor <paramref name="overrideLabel" />
    /// apply.
    /// </param>
    /// <returns>
    /// A dictionary containing either <c>aria-labelledby</c> or <c>aria-label</c>, according to the precedence rules.
    /// </returns>
    public static IReadOnlyDictionary<string, object> BuildAriaAttributes(string labelledByResolved,
        string? overrideLabel,
        string fallback)
    {
        if (!string.IsNullOrWhiteSpace(labelledByResolved))
        {
            return new Dictionary<string, object>
            {
                ["aria-labelledby"] = labelledByResolved
            };
        }

        var label = DefaultOrOverride(overrideLabel, fallback);

        return new Dictionary<string, object>
        {
            ["aria-label"] = label
        };
    }

    /// <summary>
    /// Returns the trimmed <paramref name="value" /> when it is not null/whitespace; otherwise returns the specified
    /// <paramref name="fallback" />.
    /// </summary>
    /// <param name="value">The candidate values it to prefer, if present and non-whitespace.</param>
    /// <param name="fallback">The fallback value when <paramref name="value" /> is null or whitespace.</param>
    /// <returns>The trimmed, non-empty <paramref name="value" />; otherwise <paramref name="fallback" />.</returns>
    public static string DefaultOrOverride(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim();

    /// <summary>
    /// Builds a single CSS declaration string in the form "<c>name: value</c>"; when a non-empty value is provided; otherwise
    /// returns an empty string.
    /// </summary>
    /// <param name="name">The CSS property name (for example, <c>background-color</c>).</param>
    /// <param name="value">The CSS value. When null/whitespace, no output is produced.</param>
    /// <returns>
    /// A CSS declaration ending with a semicolon, or an empty string when <paramref name="value" /> is null/whitespace.
    /// </returns>
    public static string Style(string name, string? value = null)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : $"{name}: {value};";
}
