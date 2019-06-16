using System;
using System.IO;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    /// <summary>
    ///     Defines interface of a rendering backend that implements texture loading and rendering.
    /// </summary>
    /// <remarks>
    ///     Coordinates system used by rendering methods is x-axis going right and y-axis going up with origin, that is
    ///     point (0,0), in the center of the render target - half width and half height.
    /// </remarks>
    public interface IRenderer2D : IDisposable
    {
        /// <summary>
        ///     Window that is a render target for the renderer. All rendering is presented in this window.
        /// </summary>
        IWindow Window { get; }

        /// <summary>
        ///     Creates <see cref="ITexture" /> object out of data given in a <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">Stream of data representing 2D image in supported format that is to be used as a texture.</param>
        /// <returns><see cref="ITexture" /> object that can be used in rendering.</returns>
        ITexture CreateTexture(Stream stream);

        /// <summary>
        ///     Prepares render target for rendering. It should be called in pair with <see cref="EndRendering" /> for each frame
        ///     to be rendered.
        /// </summary>
        void BeginRendering();

        /// <summary>
        ///     Ends rendering operations on render target and presents results to the user. It should be called in pair with
        ///     <see cref="BeginRendering" /> for each frame to be rendered.
        /// </summary>
        /// <param name="waitForVSync">If true, completed frame waits for vertical synchronization in order to be presented.</param>
        /// <remarks>
        ///     This method can be invoked with <paramref name="waitForVSync" /> set to <c>true</c> to wait for vertical
        ///     synchronization before presenting completed frame. The wait is synchronous and makes the calling code to wait until
        ///     frame is presented.
        /// </remarks>
        void EndRendering(bool waitForVSync);

        /// <summary>
        ///     Clears whole render target with single color.
        /// </summary>
        /// <param name="color"><see cref="Color" /> to be used for all pixels in the render target.</param>
        void Clear(Color color);

        /// <summary>
        ///     Renders given <see cref="Sprite" /> transformed with provided transformation.
        /// </summary>
        /// <param name="sprite"><see cref="Sprite" /> to be rendered on the render target.</param>
        /// <param name="transform">Transformation applied to the <see cref="Sprite" />.</param>
        void RenderSprite(Sprite sprite, Matrix3x3 transform);

        /// <summary>
        ///     Renders given text using specified <see cref="FontSize" /> and <see cref="Color" /> transformed with provided
        ///     transformation.
        /// </summary>
        /// <param name="text">Text to be rendered on the render target.</param>
        /// <param name="fontSize">Font size of the rendered text.</param>
        /// <param name="color">Color of the rendered text.</param>
        /// <param name="transform">Transformation applied to the text.</param>
        void RenderText(string text, FontSize fontSize, Color color, Matrix3x3 transform);

        // TODO Add xml docs.
        void RenderRectangle(Vector2 dimension, Color color, Matrix3x3 transform);
    }
}