﻿using System.IO;
using Geisha.Common.Math;

namespace Geisha.Engine.Rendering.Backend
{
    // TODO Consider providing methods working in two coordinate systems, one in x-right, y-up, origin in center, second x-right, y-down, origin in left upper corner.
    /// <summary>
    ///     Defines interface of a 2D rendering service that implements 2D graphical resources loading and rendering.
    /// </summary>
    /// <remarks>
    ///     Coordinates system used by rendering methods is x-axis going right and y-axis going up with origin, that is
    ///     point (0,0), in the center of the render target - half width and half height.
    /// </remarks>
    public interface IRenderer2D
    {
        /// <summary>
        ///     Width of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        int ScreenWidth { get; }

        /// <summary>
        ///     Height of the screen (full screen) or client area in the window (excluding window frame) in pixels.
        /// </summary>
        int ScreenHeight { get; }

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
        /// <seealso cref="EndRendering" />
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
        /// <seealso cref="BeginRendering" />
        void EndRendering(bool waitForVSync);

        /// <summary>
        ///     Clears whole render target with single color.
        /// </summary>
        /// <param name="color"><see cref="Color" /> to be used for all pixels in the render target.</param>
        void Clear(Color color);

        /// <summary>
        ///     Renders given <see cref="Sprite" /> transformed using provided transformation.
        /// </summary>
        /// <param name="sprite"><see cref="Sprite" /> to be rendered on the render target.</param>
        /// <param name="transform">Transformation applied to the <see cref="Sprite" />.</param>
        void RenderSprite(Sprite sprite, Matrix3x3 transform);

        /// <summary>
        ///     Renders given text using specified <see cref="FontSize" /> and <see cref="Color" /> transformed using provided
        ///     transformation.
        /// </summary>
        /// <param name="text">Text to be rendered on the render target.</param>
        /// <param name="fontSize">Font size of the rendered text.</param>
        /// <param name="color">Color of the rendered text.</param>
        /// <param name="transform">Transformation applied to the text.</param>
        void RenderText(string text, FontSize fontSize, Color color, Matrix3x3 transform);

        /// <summary>
        ///     Renders given <paramref name="rectangle" /> with specified <paramref name="color" /> transformed using provided
        ///     transformation.
        /// </summary>
        /// <param name="rectangle">Rectangle to render.</param>
        /// <param name="color">Color of rendered rectangle.</param>
        /// <param name="fillInterior">Specifies whether to fill interior of rectangle. If true, interior is filled in.</param>
        /// <param name="transform">Transformation applied to the rectangle.</param>
        void RenderRectangle(Rectangle rectangle, Color color, bool fillInterior, Matrix3x3 transform);

        /// <summary>
        ///     Renders given <paramref name="ellipse" /> with specified <paramref name="color" /> transformed using provided
        ///     transformation.
        /// </summary>
        /// <param name="ellipse">Ellipse to render.</param>
        /// <param name="color">Color of rendered ellipse.</param>
        /// <param name="fillInterior">Specifies whether to fill interior of ellipse. If true, interior is filled in.</param>
        /// <param name="transform">Transformation applied to the ellipse.</param>
        void RenderEllipse(Ellipse ellipse, Color color, bool fillInterior, Matrix3x3 transform);

        /// <summary>
        ///     Enables clipping of rendered image to specified rectangle.
        /// </summary>
        /// <param name="clippingRectangle">
        ///     Defines area of screen in pixels to be rendered while everything outside this area is
        ///     clipped.
        /// </param>
        /// <seealso cref="ClearClipping" />
        void SetClippingRectangle(Rectangle clippingRectangle);

        /// <summary>
        ///     Clears defined clipping rectangle and disables clipping.
        /// </summary>
        /// <seealso cref="SetClippingRectangle" />
        void ClearClipping();
    }
}