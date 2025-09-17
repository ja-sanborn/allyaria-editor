using Allyaria.Editor.Abstractions.Interfaces;
using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Abstractions.Types;
using Allyaria.Editor.Helpers;
using Allyaria.Editor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Allyaria.Editor.Components;

/// <summary>
/// Core editor container that composes toolbars, content, and status regions; computes sizing, accessibility,
/// localization, and theming hooks, and cascades an <see cref="AeContext" /> to children.
/// </summary>
public partial class AllyariaEditor : ComponentBase
{
    /// <summary>The effective editor context (theme + resolved theme type) that is cascaded to child components.</summary>
    private AeContext _aeContext;

    /// <summary>The sanitized space-separated ID list for the container's <c>aria-labelledby</c> attribute.</summary>
    private string _containerLabelledByResolved = string.Empty;

    /// <summary>The computed inline style string for the container element.</summary>
    private string _containerStyle = string.Empty;

    /// <summary>The computed inline style string for the content element.</summary>
    private string _contentStyle = string.Empty;

    /// <summary>The computed inline style string for the content wrapper element.</summary>
    private string _contentWrapperStyle = string.Empty;

    /// <summary>
    /// Optional override for the JS-interop implementation; lazily initialized to the default if not supplied.
    /// </summary>
    private IEditorJsInterop? _interop;

    /// <summary>The computed inline style string for the placeholder element.</summary>
    private string _placeholderStyle = string.Empty;

    /// <summary>The computed inline style string for the status element.</summary>
    private string _statusStyle = string.Empty;

    /// <summary>
    /// The system theme that was detected (only when the public theme is <see cref="AeThemeType.System" />).
    /// </summary>
    private AeThemeType? _systemThemeDetected;

    /// <summary>The computed inline style string for the toolbar element.</summary>
    private string _toolbarStyle = string.Empty;

    /// <summary>Gets or sets ARIA label overrides and labeled-by IDs.</summary>
    [Parameter]
    public AeLabels AeLabels { get; set; }

    /// <summary>Gets or sets a value indicating whether the content area should receive focus on the first render.</summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>Gets the computed container inline style (size, border, and any image overlay).</summary>
    private string ContainerStyle => _containerStyle;

    /// <summary>Gets the computed content inline style (foreground and caret color).</summary>
    private string ContentStyle => _contentStyle;

    /// <summary>Gets the computed content wrapper inline style (background color).</summary>
    private string ContentWrapperStyle => _contentWrapperStyle;

    /// <summary>Gets or sets the height in pixels; 0 means 100% height of the parent. Default is 300.</summary>
    [Parameter]
    public int Height { get; set; } = 300;

    /// <summary>
    /// Optional override for the JS interop implementation (useful for testing). If not supplied, a default implementation
    /// will be created using <see cref="JsRuntime" />.
    /// </summary>
    [Parameter]
    public IEditorJsInterop? JsInterop { get; set; }

    /// <summary>Gets or sets the JavaScript runtime from DI used by the default interop implementation.</summary>
    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = null!;

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

    /// <summary>Gets the computed placeholder inline style.</summary>
    private string PlaceholderStyle => _placeholderStyle;

    /// <summary>Gets the computed status inline style.</summary>
    private string StatusStyle => _statusStyle;

    /// <summary>Gets or sets the bound HTML/text content. Two-way binding is supported via <c>bind-Text</c>.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Occurs when <see cref="Text" /> changes due to user input.</summary>
    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    /// <summary>
    /// Exposes runtime theme configuration. Defaults to <see cref="AeThemeType.System" /> (automatic) behavior.
    /// </summary>
    [Parameter]
    public AeTheme Theme { get; set; }

    /// <summary>Gets the computed toolbar inline style.</summary>
    private string ToolbarStyle => _toolbarStyle;

    /// <summary>Gets or sets the width in pixels; 0 means 100% width of the parent. Default is 400.</summary>
    [Parameter]
    public int Width { get; set; } = 400;

    /// <summary>Ensures a JS interop implementation is available, creating the default one if none was provided.</summary>
    /// <returns>The interop instance to use.</returns>
    private IEditorJsInterop EnsureInterop() => _interop ??= JsInterop ?? new EditorJsInterop(JsRuntime);

    /// <summary>
    /// Builds ARIA attributes for the editor container using precedence rules: 1) non-empty <c>aria-labelledby</c>, 2)
    /// non-whitespace override via <c>aria-label</c>, 3) fallback via <c>aria-label</c>.
    /// </summary>
    /// <returns>An attribute dictionary containing either <c>aria-labelledby</c> or <c>aria-label</c>.</returns>
    private IReadOnlyDictionary<string, object> GetContainerAriaAttributes()
        => EditorUtils.BuildAriaAttributes(
            _containerLabelledByResolved, AeLabels.Container, EditorResources.EditorLabel
        );

    /// <summary>Returns default theme values for the specified theme type.</summary>
    /// <param name="themeType">The theme type to resolve.</param>
    /// <returns>An <see cref="AeTheme" /> carrying default colors and overlay.</returns>
    private static AeTheme GetDefaults(AeThemeType themeType)
        => themeType switch
        {
            AeThemeType.Dark => new AeTheme(
                ToolbarBackground: "#161b22",
                ContentBackground: "#0f1115",
                StatusBackground: "#161b22",
                ToolbarForeground: "#e7e9ee",
                ContentForeground: "#e7e9ee",
                StatusForeground: "#e7e9ee",
                BorderColor: "#30363d",
                CaretColor: "#e7e9ee",
                OverlayColor: "rgba(0,0,0,0.5)"
            ),
            AeThemeType.HighContrast => new AeTheme(
                ToolbarBackground: "#000000",
                ContentBackground: "#000000",
                StatusBackground: "#000000",
                ToolbarForeground: "#ffffff",
                ContentForeground: "#ffffff",
                StatusForeground: "#ffffff",
                BorderColor: "#ffffff",
                CaretColor: "#ffffff",
                OverlayColor: "rgba(255,255,255,0.5)"
            ),
            _ => new AeTheme( // Light
                ToolbarBackground: "#f6f8fa",
                ContentBackground: "#ffffff",
                StatusBackground: "#f6f8fa",
                ToolbarForeground: "#24292f",
                ContentForeground: "#24292f",
                StatusForeground: "#24292f",
                BorderColor: "#d0d7de",
                CaretColor: "#24292f",
                OverlayColor: "rgba(255,255,255,0.5)"
            )
        };

    /// <summary>
    /// Executes post-render logic including container <c>aria-labelledby</c> sanitization and optional system theme detection.
    /// </summary>
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
                    "import", "./_content/Allyaria.Editor/Components/AllyariaEditor.razor.js"
                );
            }
            catch
            {
                // No-op
            }

            // Validate only the container here; children validate their own labelled-by.
            _containerLabelledByResolved = await SanitizeLabelledByAsync(AeLabels.ContainerLabelledById);

            // Detect system theme if requested.
            if (Theme.ThemeType == AeThemeType.System)
            {
                try
                {
                    var result = await EnsureInterop().DetectSystemThemeAsync();

                    _systemThemeDetected = result switch
                    {
                        "hc" => AeThemeType.HighContrast,
                        "dark" => AeThemeType.Dark,
                        _ => AeThemeType.Light
                    };
                }
                catch
                {
                    _systemThemeDetected = AeThemeType.Light;
                }

                RecomputeStyles(_systemThemeDetected!.Value);
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Computes the effective theme, updates computed styles, and refreshes the cascaded <see cref="AeContext" />.
    /// </summary>
    protected override void OnParametersSet()
    {
        // Lazy-init interop (allow override by parameter for unit tests)
        _interop ??= JsInterop ?? new EditorJsInterop(JsRuntime);

        var effective = Theme.ThemeType == AeThemeType.System && _systemThemeDetected.HasValue
            ? _systemThemeDetected.Value
            : Theme.ThemeType;

        RecomputeStyles(effective);

        // Cascade minimal theming info (can be expanded in the future)
        _aeContext = new AeContext(Theme, effective);

        base.OnParametersSet();
    }

    /// <summary>Recomputes all region styles based on the effective theme and current parameters.</summary>
    /// <param name="effectiveTheme">The resolved theme used for defaults.</param>
    private void RecomputeStyles(AeThemeType effectiveTheme)
    {
        var defaults = GetDefaults(effectiveTheme);

        // Foreground/caret.
        var toolbarFg = EditorUtils.DefaultOrOverride(Theme.ToolbarForeground, defaults.ToolbarForeground!);
        var contentFg = EditorUtils.DefaultOrOverride(Theme.ContentForeground, defaults.ContentForeground!);
        var statusFg = EditorUtils.DefaultOrOverride(Theme.StatusForeground, defaults.StatusForeground!);
        var caret = EditorUtils.DefaultOrOverride(Theme.CaretColor, defaults.CaretColor!);

        // Background precedence: Image > Color > Theme > Fallback; Transparent clears and ignores image.
        var transparent = Theme.Transparent;
        var hasImage = !transparent && !string.IsNullOrWhiteSpace(Theme.BackgroundImage);

        var imageLayer = hasImage
            ? $"background-image: linear-gradient({defaults.OverlayColor}, {defaults.OverlayColor}), url(\"{Theme.BackgroundImage!.Trim()}\"); background-size: cover; background-position: center; background-repeat: no-repeat;"
            : string.Empty;

        var toolbarBg = transparent
            ? "transparent"
            : hasImage
                ? "transparent"
                : string.IsNullOrWhiteSpace(Theme.ToolbarBackground)
                    ? defaults.ToolbarBackground
                    : Theme.ToolbarBackground!.Trim();

        var contentBg = transparent
            ? "transparent"
            : hasImage
                ? "transparent"
                : string.IsNullOrWhiteSpace(Theme.ContentBackground)
                    ? defaults.ContentBackground
                    : Theme.ContentBackground!.Trim();

        var statusBg = transparent
            ? "transparent"
            : hasImage
                ? "transparent"
                : string.IsNullOrWhiteSpace(Theme.StatusBackground)
                    ? defaults.StatusBackground
                    : Theme.StatusBackground!.Trim();

        // Border: Specified > Theme > Fallback; if not outlined, no border.
        var outlined = Theme.Outlined;

        var borderColor = string.IsNullOrWhiteSpace(Theme.BorderColor)
            ? defaults.BorderColor
            : Theme.BorderColor!.Trim();

        var borderCss = outlined
            ? $"border: 1px solid {borderColor};"
            : "border: none;";

        // Size.
        var sizeCss =
            $"width: {(Width == 0 ? "100%" : $"{Width}px")}; height: {(Height == 0 ? "100%" : $"{Height}px")};";

        // Compose styles.
        _containerStyle = $"{sizeCss}{borderCss}{(hasImage ? imageLayer : string.Empty)}";
        _toolbarStyle = $"{EditorUtils.Style("background-color", toolbarBg)}{EditorUtils.Style("color", toolbarFg)}";
        _contentWrapperStyle = $"{EditorUtils.Style("background-color", contentBg)}";
        _contentStyle = $"{EditorUtils.Style("color", contentFg)}{EditorUtils.Style("caret-color", caret)}";
        _placeholderStyle = $"{EditorUtils.Style("color", contentFg)}opacity: 0.5;";
        _statusStyle = $"{EditorUtils.Style("background-color", statusBg)}{EditorUtils.Style("color", statusFg)}";
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
            return await EnsureInterop().SanitizeLabelledByAsync(ids);
        }
        catch
        {
            return string.Empty;
        }
    }
}
