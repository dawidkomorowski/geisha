using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing;

/// <summary>
///     Configuration of engine windowing subsystem.
/// </summary>
public sealed record WindowingConfiguration
{
    /// <summary>
    ///     Specifies whether the user can resize the game window. Default is <c>false</c>.
    /// </summary>
    public bool AllowWindowResizing { get; init; } = false;

    /// <summary>
    ///     Specifies whether the cursor is visible. Default is <c>true</c>.
    /// </summary>
    public bool CursorVisible { get; init; } = true;

    /// <summary>
    ///     Specifies the display mode of the game window. Default is <c>Windowed</c>.
    /// </summary>
    public DisplayMode DisplayMode { get; init; } = DisplayMode.Windowed;

    /// <summary>
    ///     Specifies the initial size of the game window's client area. Default is <c>1280 x 720</c>.
    /// </summary>
    public Size WindowClientSize { get; init; } = new(1280, 720);
}