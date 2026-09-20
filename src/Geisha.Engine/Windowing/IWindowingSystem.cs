namespace Geisha.Engine.Windowing;

/// <summary>
///     Provides access to the game window state.
/// </summary>
/// <remarks>
///     Changes to the window state are applied during the game loop.
/// </remarks>
public interface IWindowingSystem
{
    /// <summary>
    ///     Gets or sets a value indicating whether the cursor is visible.
    /// </summary>
    bool CursorVisible { get; set; }

    /// <summary>
    ///     Gets or sets the window display mode.
    /// </summary>
    DisplayMode DisplayMode { get; set; }
}