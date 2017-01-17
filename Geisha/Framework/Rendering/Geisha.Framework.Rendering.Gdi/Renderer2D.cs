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

        public void Render(ITexture texture, int x, int y)
        {
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                graphics.DrawImage(((Texture) texture).Bitmap, x, y);
            }
        }

        public void Render(Sprite sprite, Matrix3 transform)
        {
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                var targetRectangle = sprite.Rectangle.Transform(transform);
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
    }
}