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
            Render((Texture) texture, new Vector2(x, y));
        }

        public void Render(ITexture texture, Vector2 position)
        {
            Render((Texture) texture, position);
        }

        private void Render(Texture texture, Vector2 position)
        {
            using (var graphics = Graphics.FromImage(_renderingContext.Bitmap))
            {
                graphics.DrawImage(texture.Bitmap, (float) position.X, (float) position.Y);
            }
        }
    }
}