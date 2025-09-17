namespace Allyaria.Editor.Abstractions;

/// <summary>
/// Represents runtime theme options applied to AllyariaEditor. Immutable by design; specify all values via the
/// constructor.
/// </summary>
/// <param name="Theme">The base theme type. <see cref="ThemeType.System" /> honors OS or browser preferences.</param>
/// <param name="Transparent">
/// Whether the editor background should be transparent. When true, no fill and no background image are applied.
/// </param>
/// <param name="Outlined">Whether the editor shows an outline or border.</param>
/// <param name="BorderColor">Explicit border color. When <c>null</c> or whitespace, theme defaults are used.</param>
/// <param name="BackgroundImage">
/// Background image URL for the editor container. When set and <paramref name="Transparent" /> is false, the control
/// applies a 50% overlay appropriate to the theme and ignores region background colors.
/// </param>
/// <param name="ToolbarBackground">Toolbar background color. When <c>null</c>, theme defaults apply.</param>
/// <param name="ToolbarForeground">Toolbar foreground (text/icon) color. When <c>null</c>, theme defaults apply.</param>
/// <param name="ContentBackground">Content region background color. When <c>null</c>, theme defaults apply.</param>
/// <param name="ContentForeground">Content region foreground (text) color. When <c>null</c>, theme defaults apply.</param>
/// <param name="StatusBackground">Status bar background color. When <c>null</c>, theme defaults apply.</param>
/// <param name="StatusForeground">Status bar foreground (text) color. When <c>null</c>, theme defaults apply.</param>
/// <param name="CaretColor">Caret color for the contenteditable region. When <c>null</c>, theme defaults apply.</param>
/// <param name="OverlayColor">The appropriate overlay color for the theme style.</param>
public readonly record struct AllyariaTheme(
    ThemeType Theme = ThemeType.System,
    bool Transparent = true,
    bool Outlined = true,
    string? BorderColor = null,
    string? BackgroundImage = null,
    string? ToolbarBackground = null,
    string? ToolbarForeground = null,
    string? ContentBackground = null,
    string? ContentForeground = null,
    string? StatusBackground = null,
    string? StatusForeground = null,
    string? CaretColor = null,
    string? OverlayColor = null
);
