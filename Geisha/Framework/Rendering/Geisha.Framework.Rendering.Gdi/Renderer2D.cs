using System.ComponentModel.Composition;
using System.Drawing;
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

        public void Render(Sprite sprite, Matrix3 transform)
        {
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                // This is necessary as GDI renders from upper left corner with Y asis towards bottom of the screen
                var finalTransform = AdjustCoordinatesSystem(transform);

                var targetRectangle = sprite.Rectangle.Transform(finalTransform);
                var location = sprite.SourceUV;
                var size = sprite.SourceDimension;

                var image = ((Texture) sprite.SourceTexture).Bitmap;
                var destPoints = new[]
                {
                    new PointF((float) targetRectangle.UpperLeft.X, (float) targetRectangle.UpperLeft.Y),
                    new PointF((float) targetRectangle.UpperRight.X, (float) targetRectangle.UpperRight.Y),
                    new PointF((float) targetRectangle.LowerLeft.X, (float) targetRectangle.LowerLeft.Y)
                };
                var srcRect = new RectangleF((float) location.X, (float) location.Y, (float) size.X, (float) size.Y);

                graphics.DrawImage(image, destPoints, srcRect, GraphicsUnit.Pixel);
            }
        }

        private Matrix3 AdjustCoordinatesSystem(Matrix3 transform)
        {
            var flipYAxisAndMoveToCenterOfScreen =
                Matrix3.Translation(new Vector2((double) RenderingContext.Bitmap.Width / 2,
                    (double) RenderingContext.Bitmap.Height / 2)) * Matrix3.Scale(new Vector2(1, -1));

            return flipYAxisAndMoveToCenterOfScreen * transform;
        }
    }
}