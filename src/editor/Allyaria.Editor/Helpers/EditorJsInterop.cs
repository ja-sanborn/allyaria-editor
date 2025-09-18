using Allyaria.Editor.Abstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Allyaria.Editor.Helpers;

/// <summary>
/// Default JavaScript interop implementation for the Allyaria editor. Wraps calls into the host page's JS to detect system
/// theme, read inner HTML from the content element, and sanitize <c>aria-labelledby</c> references.
/// </summary>
internal sealed class EditorJsInterop : IEditorJsInterop
{
    /// <summary>The JavaScript runtime used to perform interop calls.</summary>
    private readonly IJSRuntime _js;

    /// <summary>Initializes a new instance of the <see cref="EditorJsInterop" /> class.</summary>
    /// <param name="js">The JavaScript runtime to use for interop calls.</param>
    public EditorJsInterop(IJSRuntime js) => _js = js;

    /// <summary>
    /// Detects the user's system theme by invoking the <c>Allyaria_Editor_detectSystemTheme</c> JavaScript function.
    /// </summary>
    /// <returns>
    /// A task that resolves to a string token representing the detected theme, typically <c>"hc"</c> (high contrast),
    /// <c>"dark"</c>, or a value mapping to light.
    /// </returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    public async Task<string> DetectSystemThemeAsync()
        => await _js.InvokeAsync<string>("Allyaria_Editor_detectSystemTheme");

    /// <summary>
    /// Gets the current <c>innerHTML</c> from the specified content element by invoking the
    /// <c>Allyaria_Editor_getInnerHtml</c> JavaScript function.
    /// </summary>
    /// <param name="element">The contenteditable element whose HTML should be read.</param>
    /// <returns>A task that resolves to the element's current <c>innerHTML</c> string.</returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    public async Task<string> GetInnerHtmlAsync(ElementReference element)
        => await _js.InvokeAsync<string>("Allyaria_Editor_getInnerHtml", element);

    /// <summary>
    /// Sanitizes a space-separated list of IDs by invoking the <c>Allyaria_Editor_sanitizeLabelledBy</c> JavaScript function.
    /// The sanitizer returns only those IDs that exist in the DOM and have non-empty text content.
    /// </summary>
    /// <param name="ids">The space-separated list of candidate IDs.</param>
    /// <returns>A task that resolves to a space-separated list of valid IDs, or an empty string if none are valid.</returns>
    /// <remarks>Any JavaScript interop failures will propagate as exceptions to the caller.</remarks>
    public async Task<string> SanitizeLabelledByAsync(string ids)
        => await _js.InvokeAsync<string>("Allyaria_Editor_sanitizeLabelledBy", ids);
}
