using Allyaria.Editor.Abstractions.Interfaces;
using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Helpers;
using Allyaria.Editor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Allyaria.Editor.Components;

/// <summary>
/// Represents the editor's content region, including the contenteditable area and its placeholder. This component owns
/// content ARIA semantics, input/focus events, placeholder visibility/announcement, and JS interop for reading inner HTML
/// and validating <c>aria-labelledby</c> IDs.
/// </summary>
public partial class AllyariaContent : ComponentBase
{
    /// <summary>
    /// Holds the resolved, sanitized space-separated ID list for the content region's <c>aria-labelledby</c> attribute.
    /// </summary>
    private string _contentLabelledByResolved = string.Empty;

    /// <summary>Reference to the contenteditable element for focus management and JS interop.</summary>
    private ElementReference _contentRef;

    /// <summary>Gets or sets the cascading editor context (computed theme/style tokens, labels, and so on).</summary>
    [CascadingParameter]
    public AeContext AeContext { get; set; }

    /// <summary>Gets or sets ARIA label overrides and labelled-by IDs for the content region.</summary>
    [Parameter]
    public AeLabels AeLabels { get; set; }

    /// <summary>Gets or sets a value indicating whether the content area should receive focus on the first render.</summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>
    /// Gets the ID of the placeholder for <c>aria-describedby</c> when the placeholder is visible; otherwise, <c>null</c>.
    /// </summary>
    private string? ContentDescribedBy
        => ShowPlaceholder
            ? "ae-placeholder"
            : null;

    /// <summary>Gets or sets the content region's inline style string (for example, color and caret-color).</summary>
    [Parameter]
    public string ContentStyle { get; set; } = string.Empty;

    /// <summary>Gets or sets the content wrapper's inline style string (for example, background-color).</summary>
    [Parameter]
    public string ContentWrapperStyle { get; set; } = string.Empty;

    /// <summary>Gets or sets the JS interop abstraction used by the component.</summary>
    [Parameter]
    public IEditorJsInterop JsInterop { get; set; } = null!;

    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>Gets or sets the callback invoked when the content region loses focus.</summary>
    [Parameter]
    public EventCallback OnBlur { get; set; }

    /// <summary>Gets or sets the callback invoked when the content region receives focus.</summary>
    [Parameter]
    public EventCallback OnFocus { get; set; }

    /// <summary>
    /// Gets or sets placeholder text shown when <see cref="Text" /> is empty; announced via <c>aria-describedby</c>.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>Gets or sets the placeholder inline style string.</summary>
    [Parameter]
    public string PlaceholderStyle { get; set; } = string.Empty;

    /// <summary>Gets a value indicating whether the placeholder should be shown.</summary>
    private bool ShowPlaceholder => !string.IsNullOrEmpty(Placeholder) && string.IsNullOrEmpty(Text);

    /// <summary>Gets or sets the current HTML/text content. Two-way binding is supported via <c>bind-Text</c>.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the callback invoked when <see cref="Text" /> changes due to user input.</summary>
    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    /// <summary>
    /// Builds ARIA attributes for the content region using precedence rules: 1) non-empty <c>aria-labelledby</c>, 2)
    /// non-whitespace override via <c>aria-label</c>, 3) fallback via <c>aria-label</c>.
    /// </summary>
    /// <returns>An attribute dictionary containing either <c>aria-labelledby</c> or <c>aria-label</c>.</returns>
    private IReadOnlyDictionary<string, object> GetContentAriaAttributes()
        => EditorUtils.BuildAriaAttributes(
            _contentLabelledByResolved, AeLabels.Content, EditorResources.EditorContentLabel
        );

    /// <summary>Handles the blur event for the content region and forwards it to <see cref="OnBlur" /> when set.</summary>
    /// <param name="_">The focus event arguments (unused).</param>
    private async Task HandleBlurAsync(FocusEventArgs _)
    {
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync();
        }
    }

    /// <summary>Handles the focus event for the content region and forwards it to <see cref="OnFocus" /> when set.</summary>
    /// <param name="_">The focus event arguments (unused).</param>
    private async Task HandleFocusAsync(FocusEventArgs _)
    {
        if (OnFocus.HasDelegate)
        {
            await OnFocus.InvokeAsync();
        }
    }

    /// <summary>Executes post-render logic, including <c>aria-labelledby</c> sanitization and optional autofocus.</summary>
    /// <param name="firstRender">True if this is the first render; otherwise, false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load component-scoped JS (ignore failures in non-browser/test environments).
            try
            {
                await JsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/Allyaria.Editor/Components/AllyariaContent.razor.js"
                );
            }
            catch
            {
                // No-op
            }

            _contentLabelledByResolved = await SanitizeLabelledByAsync(AeLabels.ContentLabelledById);

            if (AutoFocus)
            {
                try
                {
                    await _contentRef.FocusAsync();
                }
                catch
                {
                    // Ignore focus errors in non-browser/test environments.
                }
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles input in the contenteditable region, reads updated HTML via JS interop, and raises <see cref="TextChanged" />.
    /// </summary>
    /// <param name="_">The change event arguments (unused).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnInputAsync(ChangeEventArgs _)
    {
        string newHtml;

        try
        {
            newHtml = await JsInterop.GetInnerHtmlAsync(_contentRef);
        }
        catch
        {
            newHtml = Text; // Best effort if JS is unavailable.
        }

        if (!string.Equals(newHtml, Text, StringComparison.Ordinal))
        {
            Text = newHtml;

            if (TextChanged.HasDelegate)
            {
                await TextChanged.InvokeAsync(Text);
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Sanitizes a space-separated list of element IDs by filtering to those that exist and have non-empty text content.
    /// </summary>
    /// <param name="ids">The space-separated IDs to validate.</param>
    /// <returns>A space-separated list of valid IDs, or an empty string if none are valid.</returns>
    private async Task<string> SanitizeLabelledByAsync(string? ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
        {
            return string.Empty;
        }

        try
        {
            return await JsInterop.SanitizeLabelledByAsync(ids);
        }
        catch
        {
            return string.Empty;
        }
    }
}
