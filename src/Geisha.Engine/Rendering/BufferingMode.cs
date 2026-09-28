namespace Geisha.Engine.Rendering;

/// <summary>
///     Specifies the number of buffers used for rendering and presentation.
/// </summary>
public enum BufferingMode
{
    /// <summary>
    ///     Uses two buffers for rendering and presentation.
    /// </summary>
    DoubleBuffering,

    /// <summary>
    ///     Uses three buffers for rendering and presentation.
    /// </summary>
    TripleBuffering
}