namespace Geisha.Engine.Rendering;

/// <summary>
///     Specifies the number of buffers used for rendering and presentation.
/// </summary>
public enum BufferingMode
{
    /// <summary>
    ///     Uses two buffers for rendering and presentation.
    /// </summary>
    /// <remarks>
    ///     This is the default mode. It generally uses fewer presentation resources than
    ///     <see cref="TripleBuffering" />. Depending on the rendering backend, it may queue fewer frames and therefore
    ///     provide less tolerance for brief frame-time spikes. It may reduce presentation latency.
    /// </remarks>
    DoubleBuffering,

    /// <summary>
    ///     Uses three buffers for rendering and presentation.
    /// </summary>
    /// <remarks>
    ///     It generally uses more presentation resources than <see cref="DoubleBuffering" />. Depending on the
    ///     rendering backend, it may allow more frames to be queued, which can reduce presentation stalls during brief
    ///     frame-time spikes. It may increase presentation latency and does not guarantee consistent frame pacing.
    /// </remarks>
    TripleBuffering
}