namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Specifies API for capturing current state of input devices.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>
        ///     Specifies whether to lock the cursor position at center of the window/screen. If <c>true</c> the cursor is
        ///     successively moved to the center of the window/screen; otherwise it is unlocked and can move freely.
        /// </summary>
        /// <remarks>
        ///     It is useful to lock the cursor position (and often to hide it) when mouse is used as two-directional axis
        ///     input e.g. in FPS game to move the camera.
        /// </remarks>
        bool LockCursorPosition { get; set; }

        /// <summary>
        ///     Captures current state of input devices as instance of <see cref="HardwareInput" /> class.
        /// </summary>
        /// <returns><see cref="HardwareInput" /> instance that represents captured state of input devices.</returns>
        HardwareInput Capture();
    }
}