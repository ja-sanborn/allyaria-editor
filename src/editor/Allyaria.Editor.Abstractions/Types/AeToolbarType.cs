namespace Allyaria.Editor.Abstractions.Types;

/// <summary>
/// Defines the toolbar region type used by editor components to distinguish between the primary toolbar area and the
/// status area.
/// </summary>
public enum AeToolbarType
{
    /// <summary>The primary toolbar region intended for command buttons, menus, and tool controls.</summary>
    Toolbar,

    /// <summary>The status region intended for contextual information, indicators, and readouts.</summary>
    Status
}
