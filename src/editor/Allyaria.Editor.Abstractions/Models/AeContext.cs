using Allyaria.Editor.Abstractions.Types;

namespace Allyaria.Editor.Abstractions.Models;

/// <summary>
/// Cascaded editor context that provides children with the currently effective theme configuration and the resolved theme
/// type after applying system detection and overrides.
/// </summary>
/// <param name="EffectiveTheme">
/// The effective theme instance containing the computed colors (foreground/background/border/caret) and overlay settings
/// that should be applied by child regions.
/// </param>
/// <param name="EffectiveThemeType">
/// The resolved theme type (for example, <see cref="AeThemeType.Light" />, <see cref="AeThemeType.Dark" />,
/// <see cref="AeThemeType.HighContrast" />) after considering user selection and any system-detected preference.
/// </param>
public readonly record struct AeContext(
    AeTheme EffectiveTheme,
    AeThemeType EffectiveThemeType
);
