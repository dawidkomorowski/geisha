using System;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Provides access to application window.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        ///     Width of client area in the window (excluding window frame).
        /// </summary>
        int ClientAreaWidth { get; }

        /// <summary>
        ///     Height of client area in the window (excluding window frame).
        /// </summary>
        int ClientAreaHeight { get; }

        /// <summary>
        ///     Native handle to the window.
        /// </summary>
        IntPtr Handle { get; }
    }
}