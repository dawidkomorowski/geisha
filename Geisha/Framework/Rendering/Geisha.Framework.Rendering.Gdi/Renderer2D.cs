using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering.Gdi
{
    // TODO introduce batch rendering? I.e. SpriteBatch?
    [Export(typeof(IRenderer2D))]
    public class Renderer2D : Renderer, IRenderer2D
    {
        [ImportingConstructor]
        public Renderer2D(IRenderingContextFactory renderingContextFactory) : base(renderingContextFactory)
        {
        }

        public void RenderSprite(Sprite sprite, Matrix3 transform)
        {
            using (var graphics = Graphics.FromImage(InternalRenderingContext.Bitmap))
            {
                var matrix = CreateMatrixWithAdjustedCoordinatesSystem(transform);

                var spriteRectangle = sprite.Rectangle;
                var location = sprite.SourceUV;
                var size = sprite.SourceDimension;

                var image = ((Texture) sprite.SourceTexture).Bitmap;
                var srcRect = new RectangleF((float) location.X, (float) location.Y, (float) size.X, (float) size.Y);

                graphics.MultiplyTransform(matrix);
                graphics.DrawImage(image, (float) spriteRectangle.LowerLeft.X, (float) spriteRectangle.LowerLeft.Y, srcRect, GraphicsUnit.Pixel);
                graphics.ResetTransform();
            }
        }

        public void RenderText(string text, int fontSize, Color color, Matrix3 transform)
        {
            using (var graphics = Graphics.FromImage(InternalRenderingContext.Bitmap))
            {
                var matrix = CreateMatrixWithAdjustedCoordinatesSystem(transform);

                using (var font = new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel))
                {
                    graphics.MultiplyTransform(matrix);
                    graphics.DrawString(text, font, new SolidBrush(System.Drawing.Color.FromArgb(color.ToArgb())), 0, 0);
                    graphics.ResetTransform();
                }
            }
        }

        /// <summary>
        ///     Converts given <see cref="Matrix3" /> transform to GDI+ <see cref="Matrix" /> adjusting coordinates system.
        /// </summary>
        /// <remarks>
        ///     GDI+ renders from upper left corner with Y axis towards bottom of the screen while it is required to have
        ///     origin in center of screen with Y axis towards top of the screen.
        /// </remarks>
        /// <param name="transform">Raw transform to be used for rendering.</param>
        /// <returns></returns>
        private Matrix CreateMatrixWithAdjustedCoordinatesSystem(Matrix3 transform)
        {
            var xTranslation = (double) InternalRenderingContext.Bitmap.Width / 2;
            var yTranslation = (double) -InternalRenderingContext.Bitmap.Height / 2;
            var flipYAxisAndMoveToCenterOfScreen = Matrix3.Translation(new Vector2(xTranslation, yTranslation));
            transform = flipYAxisAndMoveToCenterOfScreen * transform;

            return new Matrix((float) transform.M11, (float) transform.M12, (float) transform.M21,
                (float) transform.M22, (float) transform.M13, (float) -transform.M23);
        }
    }
}