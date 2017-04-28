using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using Geisha.Common.Geometry;

namespace Geisha.Framework.Rendering.Gdi
{
    [Export(typeof(IRenderer2D))]
    public class Renderer2D : Renderer, IRenderer2D
    {
        [ImportingConstructor]
        public Renderer2D(IRenderingContextFactory renderingContextFactory) : base(renderingContextFactory)
        {
        }

        public void RenderSprite(Sprite sprite, Matrix3 transform)
        {
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                // This is necessary as GDI renders from upper left corner with Y axis towards bottom of the screen
                var finalTransform = AdjustCoordinatesSystem(transform);
                var matrix = new Matrix((float) finalTransform.M11, (float) finalTransform.M12, (float) finalTransform.M21,
                    (float) finalTransform.M22, (float) finalTransform.M13, (float) finalTransform.M23);

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
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                // This is necessary as GDI renders from upper left corner with Y axis towards bottom of the screen
                var finalTransform = AdjustCoordinatesSystem(transform);
                var matrix = new Matrix((float) finalTransform.M11, (float) finalTransform.M12, (float) finalTransform.M21,
                    (float) finalTransform.M22, (float) finalTransform.M13, (float) finalTransform.M23);

                using (var font = new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel))
                {
                    graphics.MultiplyTransform(matrix);
                    graphics.DrawString(text, font, new SolidBrush(System.Drawing.Color.FromArgb(color.ToArgb())), 0, 0);
                    graphics.ResetTransform();
                }
            }
        }

        private Matrix3 AdjustCoordinatesSystem(Matrix3 transform)
        {
            var flipYAxisAndMoveToCenterOfScreen =
                Matrix3.Translation(new Vector2((double) RenderingContext.Bitmap.Width / 2,
                    (double) RenderingContext.Bitmap.Height / 2)) * Matrix3.Scale(new Vector2(1, -1));

            return flipYAxisAndMoveToCenterOfScreen * transform * Matrix3.Scale(new Vector2(1, -1));
        }
    }
}