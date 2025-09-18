using Microsoft.AspNetCore.Components;

namespace Allyaria.Editor.Abstractions.Interfaces;

/// <summary>
/// Abstraction for JavaScript interop used by the Allyaria editor components. Provides methods to detect the system theme,
/// read content HTML, and sanitize <c>aria-labelledby</c> references.
/// </summary>
public interface IEditorJsInterop
{
    /// <summary>Detects the user's system theme via JavaScript.</summary>
    /// <returns>
    /// A task that resolves to a theme token string (for example, <c>"hc"</c> for high contrast, <c>"dark"</c>, or a value
    /// that maps to light).
    /// </returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    Task<string> DetectSystemThemeAsync();

    /// <summary>Gets the current <c>innerHTML</c> from the specified content element via JavaScript.</summary>
    /// <param name="element">The contenteditable element to read.</param>
    /// <returns>A task that resolves to the element's current <c>innerHTML</c> string.</returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    Task<string> GetInnerHtmlAsync(ElementReference element);

    /// <summary>
    /// Sanitizes a space-separated list of IDs by filtering to those that exist in the DOM and have non-empty text content,
    /// via JavaScript.
    /// </summary>
    /// <param name="ids">The space-separated list of candidate IDs.</param>
    /// <returns>A task that resolves to a space-separated list of valid IDs, or an empty string if none are valid.</returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    Task<string> SanitizeLabelledByAsync(string ids);
}
