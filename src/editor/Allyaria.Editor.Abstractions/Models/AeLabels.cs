namespace Allyaria.Editor.Abstractions.Models;

/// <summary>Defines ARIA label overrides and labeled-by ID sources for the editor and its regions.</summary>
/// <param name="Container">
/// Optional override for the editor container's accessible name. When null or whitespace, a default resource is used.
/// </param>
/// <param name="Toolbar">
/// Optional override for the toolbar region's accessible name. When null or whitespace, a default resource is used.
/// </param>
/// <param name="Content">
/// Optional override for the content region's accessible name. When null or whitespace, a default resource is used.
/// </param>
/// <param name="Status">
/// Optional override for the status region's accessible name. When null or whitespace, a default resource is used.
/// </param>
/// <param name="ContainerLabelledById">
/// Space-separated element IDs providing the accessible name for the container via <c>aria-labelledby</c>. If the IDs are
/// invalid or resolve to empty texts, the component falls back to an <c>aria-label</c>.
/// </param>
/// <param name="ToolbarLabelledById">
/// Space-separated element IDs providing the accessible name for the toolbar via <c>aria-labelledby</c>. If the IDs are
/// invalid or resolve to empty texts, the component falls back to an <c>aria-label</c>.
/// </param>
/// <param name="ContentLabelledById">
/// Space-separated element IDs providing the accessible name for the content region via <c>aria-labelledby</c>. If the IDs
/// are invalid or resolve to empty texts, the component falls back to an <c>aria-label</c>.
/// </param>
/// <param name="StatusLabelledById">
/// Space-separated element IDs providing the accessible name for the status region via <c>aria-labelledby</c>. If the IDs
/// are invalid or resolve to empty texts, the component falls back to an <c>aria-label</c>.
/// </param>
public readonly record struct AeLabels(
    string? Container = null,
    string? Toolbar = null,
    string? Content = null,
    string? Status = null,
    string? ContainerLabelledById = null,
    string? ToolbarLabelledById = null,
    string? ContentLabelledById = null,
    string? StatusLabelledById = null
);
