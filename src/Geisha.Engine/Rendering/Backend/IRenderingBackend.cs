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
    }
}