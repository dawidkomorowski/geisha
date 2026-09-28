namespace Geisha.Engine.Rendering.Systems;

/// <summary>
///     Provides runtime control over rendering-system settings.
/// </summary>
public interface IRenderingSystem
{
    /// <summary>
    ///     Gets or sets whether rendered frames are synchronized with the display's vertical refresh.
    /// </summary>
    /// <remarks>
    ///     This value is forwarded to the rendering backend and can be changed at runtime. It controls the synchronization
    ///     behavior used when the rendering system presents a frame.
    /// </remarks>
    bool VSyncEnabled { get; set; }

    /// <summary>
    ///     Gets or sets the buffering mode used for rendering and presentation.
    /// </summary>
    /// <remarks>
    ///     This value is forwarded to the rendering backend and can be changed at runtime.
    /// </remarks>
    BufferingMode BufferingMode { get; set; }
}