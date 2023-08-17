namespace Geisha.Engine.Rendering.Backend
{
    /// <summary>
    ///     Defines interface of rendering backend used by Geisha Engine.
    /// </summary>
    /// <remarks>
    ///     Rendering backend provides API for loading graphical resources (like textures) and rendering them to the screen.
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
        ///     Presents a rendered image to the user.
        /// </summary>
        /// <param name="waitForVSync">If true, completed frame waits for vertical synchronization in order to be presented.</param>
        /// <remarks>
        ///     This method can be invoked with <paramref name="waitForVSync" /> set to <c>true</c> to wait for vertical
        ///     synchronization before presenting completed frame. The wait is synchronous and makes the calling code to wait until
        ///     frame is presented.
        /// </remarks>
        void Present(bool waitForVSync);
    }
}