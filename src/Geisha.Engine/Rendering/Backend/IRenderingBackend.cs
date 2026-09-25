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

    // TODO: Add documentation.
    bool VSyncEnabled { get; set; }

    /// <summary>
    ///     Presents a rendered image to the user.
    /// </summary>
    /// <param name="waitForVSync">If true, completed frame waits for vertical synchronization in order to be presented.</param>
    /// <remarks>
    ///     This method can be invoked with <paramref name="waitForVSync" /> set to <c>true</c> to wait for vertical
    ///     synchronization before presenting completed frame. The wait is synchronous and makes the calling code to wait until
    ///     frame is presented.
    /// </remarks>
    void Present(bool waitForVSync);

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