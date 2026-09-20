using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing.Backend;

/// <summary>
///     Defines platform-specific operations for managing the game window.
/// </summary>
public interface IWindowingBackend
{
    /// <summary>
    ///     Gets or sets the window title.
    /// </summary>
    string WindowTitle { get; set; }

    /// <summary>
    ///     Gets or sets the size of the window's client area.
    /// </summary>
    Size WindowClientSize { get; set; }

    /// <summary>
    ///     Gets or sets the window display mode.
    /// </summary>
    DisplayMode DisplayMode { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user can resize the window.
    /// </summary>
    bool AllowWindowResizing { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the cursor is visible.
    /// </summary>
    bool CursorVisible { get; set; }

    /// <summary>
    ///     Runs the window update loop.
    /// </summary>
    /// <param name="updateCallback">The callback invoked for each iteration of the update loop.</param>
    /// <remarks>
    ///     The update loop continues while <paramref name="updateCallback" /> returns <see langword="true" /> and stops when
    ///     it returns <see langword="false" />.
    /// </remarks>
    void RunUpdateLoop(Func<bool> updateCallback);
}