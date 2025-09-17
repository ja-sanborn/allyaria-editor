using Allyaria.Editor.Abstractions;
using Allyaria.Editor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Allyaria.Editor.Components;

/// <summary>
/// A core editor container that renders toolbar, content, and status regions with sizing, accessibility, localization, and
/// theming hooks.
/// </summary>
public partial class AllyariaEditor : ComponentBase
{
    /// <summary>The sanitized space-separated ID list for the container's <c>aria-labelledby</c> attribute.</summary>
    private string _containerLabelledByResolved = string.Empty;

    /// <summary>The sanitized space-separated ID list for the content's <c>aria-labelledby</c> attribute.</summary>
    private string _contentLabelledByResolved = string.Empty;

    /// <summary>A reference to the content element for focus management and interop.</summary>
    private ElementReference _contentRef;

    /// <summary>The sanitized space-separated ID list for the status region's <c>aria-labelledby</c> attribute.</summary>
    private string _statusLabelledByResolved = string.Empty;

    /// <summary>The sanitized space-separated ID list for the toolbar's <c>aria-labelledby</c> attribute.</summary>
    private string _toolbarLabelledByResolved = string.Empty;

    /// <summary>Gets or sets a value indicating whether the content area should receive focus on the first render.</summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>Gets the ID of the placeholder for <c>aria-describedby</c> when the placeholder is visible.</summary>
    private string? ContentDescribedBy
        => ShowPlaceholder
            ? "ae-placeholder"
            : null;

    /// <summary>Gets or sets the height in pixels; 0 means 100% height of the parent. Default is 300.</summary>
    [Parameter]
    public int Height { get; set; } = 300;

    /// <summary>Gets or sets the JavaScript interop service used by the component.</summary>
    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>Gets or sets ARIA label overrides and labeled-by IDs.</summary>
    [Parameter]
    public AriaLabels Labels { get; set; }

    /// <summary>Occurs when the content region loses focus.</summary>
    [Parameter]
    public EventCallback OnBlur { get; set; }

    /// <summary>Occurs when the content region receives focus.</summary>
    [Parameter]
    public EventCallback OnFocus { get; set; }

    /// <summary>
    /// Gets or sets placeholder text shown when <see cref="Text" /> is empty; announced via <c>aria-describedby</c>.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>Gets a value indicating whether the placeholder should be shown.</summary>
    private bool ShowPlaceholder => !string.IsNullOrEmpty(Placeholder) && string.IsNullOrEmpty(Text);

    /// <summary>Computes the width and height inline styles based on the component parameters.</summary>
    private string Style
        => $"width: {(Width == 0 ? "100%" : $"{Width}px")}; height: {(Height == 0 ? "100%" : $"{Height}px")};";

    /// <summary>Gets or sets the bound HTML/text content. Two-way binding is supported via <c>bind-Text</c>.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Occurs when <see cref="Text" /> changes due to user input.</summary>
    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    /// <summary>Gets or sets the width in pixels; 0 means 100% width of the parent. Default is 400.</summary>
    [Parameter]
    public int Width { get; set; } = 400;

    /// <summary>Builds the appropriate ARIA attributes following the precedence rules.</summary>
    /// <param name="labelledByResolved">A sanitized list of labeled-by IDs (can be empty).</param>
    /// <param name="overrideLabel">An optional override label value.</param>
    /// <param name="fallback">The fallback label when neither labeled-by nor override applies.</param>
    /// <returns>An attribute dictionary with either <c>aria-labeledby</c> or <c>aria-label</c>.</returns>
    private static IReadOnlyDictionary<string, object> BuildAriaAttributes(string labelledByResolved,
        string? overrideLabel,
        string fallback)
    {
        // Precedence:
        // 1) valid aria-labelledby (non-empty sanitized id list)
        // 2) non-whitespace override string via aria-label
        // 3) fallback to .resx default via aria-label
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

    /// <summary>Returns either the provided non-whitespace value or the specified fallback.</summary>
    /// <param name="value">The override value to prefer, if present and non-whitespace.</param>
    /// <param name="fallback">The fallback to use when <paramref name="value" /> is null or whitespace.</param>
    /// <returns>The chosen non-empty string.</returns>
    private static string DefaultOrOverride(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim();

    /// <summary>Builds ARIA attributes for the editor container region.</summary>
    /// <returns>An attribute dictionary to be applied to the container element.</returns>
    private IReadOnlyDictionary<string, object> GetContainerAriaAttributes()
        => BuildAriaAttributes(_containerLabelledByResolved, Labels.Container, EditorResources.EditorLabel);

    /// <summary>Builds ARIA attributes for the content region.</summary>
    /// <returns>An attribute dictionary to be applied to the content element.</returns>
    private IReadOnlyDictionary<string, object> GetContentAriaAttributes()
        => BuildAriaAttributes(_contentLabelledByResolved, Labels.Content, EditorResources.EditorContentLabel);

    /// <summary>Builds ARIA attributes for the status region.</summary>
    /// <returns>An attribute dictionary to be applied to the status element.</returns>
    private IReadOnlyDictionary<string, object> GetStatusAriaAttributes()
        => BuildAriaAttributes(_statusLabelledByResolved, Labels.Status, EditorResources.EditorStatusLabel);

    /// <summary>Builds ARIA attributes for the toolbar region.</summary>
    /// <returns>An attribute dictionary to be applied to the toolbar element.</returns>
    private IReadOnlyDictionary<string, object> GetToolbarAriaAttributes()
        => BuildAriaAttributes(_toolbarLabelledByResolved, Labels.Toolbar, EditorResources.EditorToolbarLabel);

    /// <summary>Handles the blur event for the content region.</summary>
    /// <param name="_">The focus event arguments (unused).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task HandleBlurAsync(FocusEventArgs _)
    {
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync();
        }
    }

    /// <summary>Handles the focus event for the content region.</summary>
    /// <param name="_">The focus event arguments (unused).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task HandleFocusAsync(FocusEventArgs _)
    {
        if (OnFocus.HasDelegate)
        {
            await OnFocus.InvokeAsync();
        }
    }

    /// <summary>Executes post-render logic, including labeled-by sanitization and optional autofocus behavior.</summary>
    /// <param name="firstRender">True if this is the first render; otherwise, false.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ValidateLabelledByAsync();

            if (AutoFocus)
            {
                try
                {
                    await _contentRef.FocusAsync();
                }
                catch
                {
                    // Ignore focus errors in non-browser/test environments
                }
            }
        }
    }

    /// <summary>Handles the content input event and updates the bound <see cref="Text" /> value.</summary>
    /// <param name="_">The change event arguments (unused).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task OnInputAsync(ChangeEventArgs _)
    {
        string newHtml;

        try
        {
            newHtml = await JsRuntime.InvokeAsync<string>("Allyaria_Editor_getInnerHtml", _contentRef);
        }
        catch
        {
            newHtml = Text; // best effort in environments without JS
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
            // Returns space-separated existing ids that have non-empty text; empty string if none
            return await JsRuntime.InvokeAsync<string>("Allyaria_Editor_sanitizeLabelledBy", ids);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>Validates the <c>aria-labelledby</c> IDs for all regions and updates internal state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ValidateLabelledByAsync()
    {
        _containerLabelledByResolved = await SanitizeLabelledByAsync(Labels.ContainerLabelledById);
        _toolbarLabelledByResolved = await SanitizeLabelledByAsync(Labels.ToolbarLabelledById);
        _contentLabelledByResolved = await SanitizeLabelledByAsync(Labels.ContentLabelledById);
        _statusLabelledByResolved = await SanitizeLabelledByAsync(Labels.StatusLabelledById);
        StateHasChanged();
    }
}
