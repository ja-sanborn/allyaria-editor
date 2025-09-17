using System.Globalization;
using System.Resources;

namespace Allyaria.Editor.Resources;

/// <summary>Provides strongly typed access to localized editor resource strings with safe fallbacks.</summary>
internal static class EditorResources
{
    /// <summary>The resource manager used to resolve localized strings for the editor.</summary>
    private static readonly ResourceManager ResourceManager = new(
        "Allyaria.Editor.Resources.EditorResources", typeof(EditorResources).Assembly
    );

    /// <summary>Gets the localized label for the editor content region.</summary>
    public static string EditorContentLabel => Get(nameof(EditorContentLabel), "Editor content");

    /// <summary>Gets the localized label for the editor container.</summary>
    public static string EditorLabel => Get(nameof(EditorLabel), "Editor");

    /// <summary>Gets the localized label for the editor status region.</summary>
    public static string EditorStatusLabel => Get(nameof(EditorStatusLabel), "Editor status");

    /// <summary>Gets the localized label for the editor toolbar region.</summary>
    public static string EditorToolbarLabel => Get(nameof(EditorToolbarLabel), "Editor toolbar");

    /// <summary>
    /// Retrieves a localized string by name, returning a fallback value when the resource is missing or empty.
    /// </summary>
    /// <param name="name">The resource name to resolve.</param>
    /// <param name="fallback">The fallback value when the resource can't be resolved.</param>
    /// <returns>A non-empty localized string.</returns>
    private static string Get(string name, string fallback)
    {
        try
        {
            var value = ResourceManager.GetString(name, CultureInfo.CurrentUICulture);

            return string.IsNullOrWhiteSpace(value)
                ? fallback
                : value;
        }
        catch
        {
            return fallback;
        }
    }
}
