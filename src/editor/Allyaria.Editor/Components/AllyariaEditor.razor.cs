using Allyaria.Editor.Abstractions;
using Allyaria.Editor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Allyaria.Editor.Components;

/// <summary>
/// Core editor container that renders toolbar, content, and status regions with sizing, accessibility, localization, and
/// theming hooks.
/// </summary>
public partial class AllyariaEditor : ComponentBase
{
    /// <summary>The sanitized space-separated ID list for the container's <c>aria-labelledby</c> attribute.</summary>
    private string _containerLabelledByResolved = string.Empty;

    /// <summary>The computed container inline style string.</summary>
    private string _containerStyle = string.Empty;

    /// <summary>The sanitized space-separated ID list for the content's <c>aria-labelledby</c> attribute.</summary>
    private string _contentLabelledByResolved = string.Empty;

    /// <summary>A reference to the content element for focus management and interop.</summary>
    private ElementReference _contentRef;

    /// <summary>The computed content inline style string.</summary>
    private string _contentStyle = string.Empty;

    /// <summary>The computed content wrapper inline style string.</summary>
    private string _contentWrapperStyle = string.Empty;

    /// <summary>The computed placeholder inline style string.</summary>
    private string _placeholderStyle = string.Empty;

    /// <summary>The sanitized space-separated ID list for the status region's <c>aria-labelledby</c> attribute.</summary>
    private string _statusLabelledByResolved = string.Empty;

    /// <summary>The computed status inline style string.</summary>
    private string _statusStyle = string.Empty;

    /// <summary>
    /// The theme detected from the host system when <see cref="AllyariaTheme.Theme" /> is <see cref="ThemeType.System" />.
    /// </summary>
    private ThemeType? _systemThemeDetected;

    /// <summary>The sanitized space-separated ID list for the toolbar's <c>aria-labelledby</c> attribute.</summary>
    private string _toolbarLabelledByResolved = string.Empty;

    /// <summary>The computed toolbar inline style string.</summary>
    private string _toolbarStyle = string.Empty;

    /// <summary>Gets or sets a value indicating whether the content area should receive focus on the first render.</summary>
    [Parameter]
    public bool AutoFocus { get; set; }

    /// <summary>Computes the container style including width and height plus theming.</summary>
    private string ContainerStyle => _containerStyle;

    /// <summary>Gets the ID of the placeholder for <c>aria-describedby</c> when the placeholder is visible.</summary>
    private string? ContentDescribedBy
        => ShowPlaceholder
            ? "ae-placeholder"
            : null;

    /// <summary>Computes the content region style.</summary>
    private string ContentStyle => _contentStyle;

    /// <summary>Computes the content wrapper style.</summary>
    private string ContentWrapperStyle => _contentWrapperStyle;

    /// <summary>Gets or sets the height in pixels; 0 means 100% height of the parent. Default is 300.</summary>
    [Parameter]
    public int Height { get; set; } = 300;

    /// <summary>Gets or sets the JavaScript interop service used by the component.</summary>
    [Inject]
    internal IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>Gets or sets ARIA label overrides and labeled-by IDs.</summary>
    [Parameter]
    public AllyariaLabels Labels { get; set; }

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

    /// <summary>Computes the placeholder style.</summary>
    private string PlaceholderStyle => _placeholderStyle;

    /// <summary>Gets a value indicating whether the placeholder should be shown.</summary>
    private bool ShowPlaceholder => !string.IsNullOrEmpty(Placeholder) && string.IsNullOrEmpty(Text);

    /// <summary>Computes the status region style.</summary>
    private string StatusStyle => _statusStyle;

    /// <summary>Gets or sets the bound HTML/text content. Two-way binding is supported via <c>bind-Text</c>.</summary>
    [Parameter]
    public string Text { get; set; } = string.Empty;

    /// <summary>Occurs when <see cref="Text" /> changes due to user input.</summary>
    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    /// <summary>Exposes runtime theme configuration. Defaults to System (automatic) behavior.</summary>
    [Parameter]
    public AllyariaTheme Theme { get; set; }

    /// <summary>Computes the toolbar region style.</summary>
    private string ToolbarStyle => _toolbarStyle;

    /// <summary>Gets or sets the width in pixels; 0 means 100% width of the parent. Default is 400.</summary>
    [Parameter]
    public int Width { get; set; } = 400;

    /// <summary>
    /// Builds ARIA attributes using the precedence rules: 1) non-empty <c>aria-labelledby</c>, 2) non-whitespace override via
    /// <c>aria-label</c>, 3) fallback via <c>aria-label</c>.
    /// </summary>
    /// <param name="labelledByResolved">A sanitized list of labeled-by IDs (can be empty).</param>
    /// <param name="overrideLabel">An optional override label value.</param>
    /// <param name="fallback">The fallback label when neither labeled-by nor override applies.</param>
    /// <returns>An attribute dictionary with either <c>aria-labelledby</c> or <c>aria-label</c>.</returns>
    private static IReadOnlyDictionary<string, object> BuildAriaAttributes(string labelledByResolved,
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

    /// <summary>Returns default theme values for the specified <see cref="ThemeType" />.</summary>
    /// <param name="theme">The theme type to resolve.</param>
    /// <returns>An <see cref="AllyariaTheme" /> carrying default colors and overlay.</returns>
    private static AllyariaTheme GetDefaults(ThemeType theme)
        => theme switch
        {
            ThemeType.Dark => new AllyariaTheme(
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
            ThemeType.HighContrast => new AllyariaTheme(
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
            _ => new AllyariaTheme( // Light
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
    private async Task HandleBlurAsync(FocusEventArgs _)
    {
        if (OnBlur.HasDelegate)
        {
            await OnBlur.InvokeAsync();
        }
    }

    /// <summary>Handles the focus event for the content region.</summary>
    /// <param name="_">The focus event arguments (unused).</param>
    private async Task HandleFocusAsync(FocusEventArgs _)
    {
        if (OnFocus.HasDelegate)
        {
            await OnFocus.InvokeAsync();
        }
    }

    /// <summary>
    /// Executes post-render logic including labelled-by sanitization, system theme detection, and optional autofocus.
    /// </summary>
    /// <param name="firstRender">True if this is the first render; otherwise, false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ValidateLabelledByAsync();

            // Detect system theme if requested.
            if (Theme.Theme == ThemeType.System)
            {
                try
                {
                    var result = await JsRuntime.InvokeAsync<string>("Allyaria_Editor_detectSystemTheme");

                    _systemThemeDetected = result switch
                    {
                        "hc" => ThemeType.HighContrast,
                        "dark" => ThemeType.Dark,
                        _ => ThemeType.Light
                    };
                }
                catch
                {
                    _systemThemeDetected = ThemeType.Light;
                }

                RecomputeStyles(_systemThemeDetected!.Value);
                StateHasChanged();
            }

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
        }
    }

    /// <summary>Handles the content input event and updates the bound <see cref="Text" /> value.</summary>
    /// <param name="_">The change event arguments (unused).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnInputAsync(ChangeEventArgs _)
    {
        string newHtml;

        try
        {
            newHtml = await JsRuntime.InvokeAsync<string>("Allyaria_Editor_getInnerHtml", _contentRef);
        }
        catch
        {
            newHtml = Text; // Best effort in environments without JS.
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

    /// <summary>Computes the theme and styles whenever parameters change.</summary>
    protected override void OnParametersSet()
    {
        var effective = Theme.Theme == ThemeType.System && _systemThemeDetected.HasValue
            ? _systemThemeDetected.Value
            : Theme.Theme;

        RecomputeStyles(effective);
        base.OnParametersSet();
    }

    /// <summary>Recomputes all region styles based on the effective theme and current parameters.</summary>
    /// <param name="effectiveTheme">The resolved theme used for defaults.</param>
    private void RecomputeStyles(ThemeType effectiveTheme)
    {
        var defaults = GetDefaults(effectiveTheme);

        // Foreground/caret.
        var toolbarFg = DefaultOrOverride(Theme.ToolbarForeground, defaults.ToolbarForeground!);
        var contentFg = DefaultOrOverride(Theme.ContentForeground, defaults.ContentForeground!);
        var statusFg = DefaultOrOverride(Theme.StatusForeground, defaults.StatusForeground!);
        var caret = DefaultOrOverride(Theme.CaretColor, defaults.CaretColor!);

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
        _toolbarStyle = $"{Style("background-color", toolbarBg)}{Style("color", toolbarFg)}";
        _contentWrapperStyle = $"{Style("background-color", contentBg)}";
        _contentStyle = $"{Style("color", contentFg)}{Style("caret-color", caret)}";
        _placeholderStyle = $"{Style("color", contentFg)}opacity: 0.5;";
        _statusStyle = $"{Style("background-color", statusBg)}{Style("color", statusFg)}";
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
            // Returns space-separated existing ids that have non-empty text; empty string if none.
            return await JsRuntime.InvokeAsync<string>("Allyaria_Editor_sanitizeLabelledBy", ids);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>Builds a style declaration when a non-empty value is provided.</summary>
    /// <param name="name">The CSS property name (for example, <c>background-color</c>).</param>
    /// <param name="value">The CSS value. If null/whitespace, return an empty string.</param>
    /// <returns>A CSS declaration ending with a semicolon, or an empty string.</returns>
    private static string Style(string name, string? value = null)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : $"{name}: {value};";

    /// <summary>Validates the <c>aria-labelledby</c> IDs for all regions and updates internal state.</summary>
    private async Task ValidateLabelledByAsync()
    {
        _containerLabelledByResolved = await SanitizeLabelledByAsync(Labels.ContainerLabelledById);
        _toolbarLabelledByResolved = await SanitizeLabelledByAsync(Labels.ToolbarLabelledById);
        _contentLabelledByResolved = await SanitizeLabelledByAsync(Labels.ContentLabelledById);
        _statusLabelledByResolved = await SanitizeLabelledByAsync(Labels.StatusLabelledById);
        StateHasChanged();
    }
}
