namespace Geisha.Engine.Rendering.Backend
{
    /// <summary>
    ///     Defines interface of rendering backend used by the engine.
    /// </summary>
    /// <remarks>
    ///     Rendering backend provides API for loading graphical resources (like textures) and rendering them to the screen.
    /// </remarks>
    public interface IRenderingBackend
    {
        /// <summary>
        ///     2D rendering service provided by the rendering backend.
        /// </summary>
        IRenderer2D Renderer2D { get; }
    }
}