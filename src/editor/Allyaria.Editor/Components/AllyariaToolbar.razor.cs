using Allyaria.Editor.Abstractions.Interfaces;
using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Abstractions.Types;
using Allyaria.Editor.Helpers;
using Allyaria.Editor.Resources;
using Microsoft.AspNetCore.Components;

namespace Allyaria.Editor.Components;

/// <summary>
/// Represents the toolbar/status host for the editor. This component renders either the primary toolbar region or the
/// status region (depending on <see cref="Region" />), owns ARIA semantics for that region, and resolves any
/// <c>aria-labelledby</c> references via JS interop.
/// </summary>
public partial class AllyariaToolbar : ComponentBase
{
    /// <summary>
    /// Holds the resolved, sanitized space-separated ID list for the status region's <c>aria-labelledby</c> attribute.
    /// </summary>
    private string _statusLabelledByResolved = string.Empty;

    /// <summary>
    /// Holds the resolved, sanitized space-separated ID list for the toolbar region's <c>aria-labelledby</c> attribute.
    /// </summary>
    private string _toolbarLabelledByResolved = string.Empty;

    /// <summary>Gets or sets the cascading editor context (computed theme/style tokens, labels, and so on).</summary>
    [CascadingParameter]
    public AeContext AeContext { get; set; }

    /// <summary>Gets or sets ARIA label overrides and labelled-by IDs for this component's regions.</summary>
    [Parameter]
    public AeLabels AeLabels { get; set; }

    /// <summary>
    /// Gets or sets the JS interop abstraction used by the component to sanitize <c>aria-labelledby</c> IDs.
    /// </summary>
    [Parameter]
    public IEditorJsInterop JsInterop { get; set; } = null!;

    /// <summary>
    /// Gets or sets which region this instance represents: <see cref="ToolbarRegion.Toolbar" /> or
    /// <see cref="ToolbarRegion.Status" />. Defaults to <see cref="ToolbarRegion.Toolbar" />.
    /// </summary>
    [Parameter]
    public ToolbarRegion Region { get; set; } = ToolbarRegion.Toolbar;

    /// <summary>
    /// Gets or sets the inline style string for the status region (for example, background/foreground colors).
    /// </summary>
    [Parameter]
    public string StatusStyle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inline style string for the toolbar region (for example, background/foreground colors).
    /// </summary>
    [Parameter]
    public string ToolbarStyle { get; set; } = string.Empty;

    /// <summary>
    /// Builds ARIA attributes for the status region using precedence rules: 1) non-empty <c>aria-labelledby</c>, 2)
    /// non-whitespace override via <c>aria-label</c>, 3) fallback via <c>aria-label</c>.
    /// </summary>
    /// <returns>An attribute dictionary containing either <c>aria-labelledby</c> or <c>aria-label</c>.</returns>
    internal IReadOnlyDictionary<string, object> GetAriaAttributesForStatus()
        => EditorUtils.BuildAriaAttributes(
            _statusLabelledByResolved, AeLabels.Status, EditorResources.EditorStatusLabel
        );

    /// <summary>
    /// Builds ARIA attributes for the toolbar region using precedence rules: 1) non-empty <c>aria-labelledby</c>, 2)
    /// non-whitespace override via <c>aria-label</c>, 3) fallback via <c>aria-label</c>.
    /// </summary>
    /// <returns>An attribute dictionary containing either <c>aria-labelledby</c> or <c>aria-label</c>.</returns>
    internal IReadOnlyDictionary<string, object> GetAriaAttributesForToolbar()
        => EditorUtils.BuildAriaAttributes(
            _toolbarLabelledByResolved, AeLabels.Toolbar, EditorResources.EditorToolbarLabel
        );

    /// <summary>Executes post-render logic, resolving <c>aria-labelledby</c> references for the active region.</summary>
    /// <param name="firstRender">True if this is the first render; otherwise, false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Resolve labelled-by per region.
            if (Region == ToolbarRegion.Toolbar)
            {
                _toolbarLabelledByResolved = await SanitizeLabelledByAsync(AeLabels.ToolbarLabelledById);
            }
            else
            {
                _statusLabelledByResolved = await SanitizeLabelledByAsync(AeLabels.StatusLabelledById);
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
