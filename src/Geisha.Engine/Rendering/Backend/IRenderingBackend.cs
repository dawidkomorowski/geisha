using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend;

/// <summary>
///     Defines interface of rendering backend used by Geisha Engine.
/// </summary>
/// <remarks>
///     Rendering backend provides API for loading graphical resources (like textures) rendering them to a render target
///     and presenting rendered images.
/// </remarks>
public interface IRenderingBackend
{
    /// <summary>
    ///     2D rendering context provided by the rendering backend.
    /// </summary>
    IRenderingContext2D Context2D { get; }

    /// <summary>
    ///     Rendering statistics of last frame provided by the rendering backend.
    /// </summary>
    RenderingStatistics Statistics { get; }

    /// <summary>
    ///     Information about the rendering backend and the graphics adapter it uses.
    /// </summary>
    RenderingBackendInfo Info { get; }

    /// <summary>
    ///     Gets or sets whether rendered frames are synchronized with the display's vertical refresh during
    ///     <see cref="Present" />.
    /// </summary>
    /// <remarks>
    ///     This value can be changed at runtime. Set it to <c>true</c> to wait for vertical synchronization while
    ///     presenting frames, or to <c>false</c> to present without waiting for vertical synchronization.
    /// </remarks>
    bool VSyncEnabled { get; set; }

    /// <summary>
    ///     Presents a rendered image to the user.
    /// </summary>
    /// <remarks>
    ///     Set <see cref="VSyncEnabled" /> to control whether presentation waits for vertical synchronization.
    /// </remarks>
    void Present();

    /// <summary>
    ///     Resizes the buffers used for rendering to the specified size.
    /// </summary>
    /// <param name="size">The new buffer size. Both dimensions must be greater than zero.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     Thrown when either dimension of <paramref name="size" /> is less than or equal to zero.
    /// </exception>
    /// <remarks>
    ///     Call this method after the render target size changes and before rendering at the new size. For a window-backed
    ///     render target, <paramref name="size" /> should match the window client-area size.
    /// </remarks>
    void ResizeBuffers(Size size);
}