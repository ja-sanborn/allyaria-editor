namespace Allyaria.Editor.Abstractions;

/// <summary>Defines built-in theme options.</summary>
public enum ThemeType
{
    /// <summary>Uses the system preference (forced-colors/high contrast first, then dark/light).</summary>
    System = 0,

    /// <summary>Light theme preset with WCAG-compliant contrast.</summary>
    Light = 1,

    /// <summary>Dark theme preset with WCAG-compliant contrast.</summary>
    Dark = 2,

    /// <summary>High-contrast preset (grayscale) for accessibility.</summary>
    HighContrast = 3
}
