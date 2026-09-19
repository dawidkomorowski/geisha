namespace Geisha.Engine.Windowing;

/// <summary>
///     Specifies how the game window is displayed.
/// </summary>
public enum DisplayMode
{
    /// <summary>
    ///     Displays the game in a window.
    /// </summary>
    Windowed,

    /// <summary>
    ///     Displays the game in fullscreen mode.
    /// </summary>
    /// <remarks>
    ///     Fullscreen mode uses a borderless fullscreen window.
    /// </remarks>
    Fullscreen
}