using System.IO;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend
{
    // TODO Consider providing methods working in two coordinate systems, one in x-right, y-up, origin in center, second x-right, y-down, origin in left upper corner?
    /// <summary>
    ///     Defines interface of a 2D rendering context that implements 2D graphical resources loading and rendering.
    /// </summary>
    /// <remarks>
    ///     Coordinates system used by rendering methods is x-axis going right and y-axis going up with origin, that is
    ///     point (0,0), in the center of the render target - half width and half height.
    /// </remarks>
    public interface IRenderingContext2D
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
        ///     Creates new instance of <see cref="ITexture" /> object out of data given in a <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">Stream of data representing 2D image in supported format that is to be used as a texture.</param>
        /// <returns><see cref="ITexture" /> object that can be used in rendering.</returns>
        ITexture CreateTexture(Stream stream);

        /// <summary>
        ///     Creates new instance of <see cref="ITextLayout" /> object with specified properties.
        /// </summary>
        /// <param name="text">Text content of layout object.</param>
        /// <param name="fontFamilyName">Initial font family name of layout object.</param>
        /// <param name="fontSize">Initial font size of layout object.</param>
        /// <param name="maxWidth">Maximum width of layout box.</param>
        /// <param name="maxHeight">Maximum height of layout box.</param>
        /// <returns>
        ///     <see cref="ITextLayout" /> object that can be used for calculating text metrics and rendering of formatted
        ///     text.
        /// </returns>
        ITextLayout CreateTextLayout(string text, string fontFamilyName, FontSize fontSize, double maxWidth, double maxHeight);

        /// <summary>
        ///     Captures screen shot of currently rendered image as a PNG.
        /// </summary>
        /// <param name="stream">Output stream for PNG image bytes of captured screen shot.</param>
        public void CaptureScreenShotAsPng(Stream stream);

        /// <summary>
        ///     Initiates drawing on render target in this context. It should be called in pair with <see cref="EndDraw" />
        ///     for each frame to be rendered.
        /// </summary>
        /// <seealso cref="EndDraw" />
        void BeginDraw();

        /// <summary>
        ///     Ends drawing operations on the render target in this context. It should be called in pair with
        ///     <see cref="BeginDraw" /> for each frame to be rendered.
        /// </summary>
        /// <seealso cref="BeginDraw" />
        void EndDraw();

        /// <summary>
        ///     Clears whole render target with single color.
        /// </summary>
        /// <param name="color"><see cref="Color" /> to be used for all pixels in the render target.</param>
        void Clear(Color color);

        /// <summary>
        ///     Draws specified <see cref="Sprite" />.
        /// </summary>
        /// <param name="sprite"><see cref="Sprite" /> to be drawn on the render target.</param>
        /// <param name="transform">Transformation applied to the <see cref="Sprite" />.</param>
        /// <param name="opacity">
        ///     Opacity of drawn sprite. Valid range is from 0.0 meaning fully transparent to 1.0 meaning fully opaque. Default
        ///     value is <c>1.0</c>.
        /// </param>
        void DrawSprite(Sprite sprite, in Matrix3x3 transform, double opacity = 1d);

        // TODO Add documentation.
        void DrawSpriteBatch(SpriteBatch spriteBatch);

        /// <summary>
        ///     Draws specified text with origin at top left corner of layout box with text aligned to the left.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="fontFamilyName">Name of font family to be used for drawing text.</param>
        /// <param name="fontSize">Font size of the text to draw.</param>
        /// <param name="color">Color of the text to draw.</param>
        /// <param name="transform">Transformation applied to the text.</param>
        void DrawText(string text, string fontFamilyName, FontSize fontSize, Color color, in Matrix3x3 transform);

        /// <summary>
        ///     Draws specified <see cref="ITextLayout" />.
        /// </summary>
        /// <param name="textLayout">Text layout to draw.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="pivot">Pivot point which defines origin for transformations. It is defined relative to layout box.</param>
        /// <param name="transform">Transformation applied to the text layout.</param>
        /// <param name="clipToLayoutBox">
        ///     Specifies whether rendered text should be clipped to the layout rectangle. If <c>true</c>, clipping is enabled.
        /// </param>
        void DrawTextLayout(ITextLayout textLayout, Color color, in Vector2 pivot, in Matrix3x3 transform, bool clipToLayoutBox = false);

        /// <summary>
        ///     Draws specified <paramref name="rectangle" />.
        /// </summary>
        /// <param name="rectangle">Rectangle to draw.</param>
        /// <param name="color">Color of drawn rectangle.</param>
        /// <param name="fillInterior">Specifies whether to fill interior of rectangle. If true, interior is filled in.</param>
        /// <param name="transform">Transformation applied to the rectangle.</param>
        void DrawRectangle(in AxisAlignedRectangle rectangle, Color color, bool fillInterior, in Matrix3x3 transform);

        /// <summary>
        ///     Draws specified <paramref name="ellipse" />.
        /// </summary>
        /// <param name="ellipse">Ellipse to draw.</param>
        /// <param name="color">Color of drawn ellipse.</param>
        /// <param name="fillInterior">Specifies whether to fill interior of ellipse. If true, interior is filled in.</param>
        /// <param name="transform">Transformation applied to the ellipse.</param>
        void DrawEllipse(in Ellipse ellipse, Color color, bool fillInterior, in Matrix3x3 transform);

        /// <summary>
        ///     Enables clipping of rendered image to specified rectangle.
        /// </summary>
        /// <param name="clippingRectangle">
        ///     Defines area of screen in pixels to be rendered while everything outside this area is
        ///     clipped.
        /// </param>
        /// <seealso cref="ClearClipping" />
        void SetClippingRectangle(in AxisAlignedRectangle clippingRectangle);

        /// <summary>
        ///     Clears defined clipping rectangle and disables clipping.
        /// </summary>
        /// <seealso cref="SetClippingRectangle" />
        void ClearClipping();
    }
}