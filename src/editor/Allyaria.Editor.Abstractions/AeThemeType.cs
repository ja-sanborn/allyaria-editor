namespace Allyaria.Editor.Abstractions;

/// <summary>Defines built-in aeTheme options.</summary>
public enum AeThemeType
{
    /// <summary>Uses the system preference (forced-colors/high contrast first, then dark/light).</summary>
    System = 0,

    /// <summary>Light aeTheme preset with WCAG-compliant contrast.</summary>
    Light = 1,

    /// <summary>Dark aeTheme preset with WCAG-compliant contrast.</summary>
    Dark = 2,

    /// <summary>High-contrast preset (grayscale) for accessibility.</summary>
    HighContrast = 3
}
