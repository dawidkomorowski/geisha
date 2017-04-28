using System.Drawing;
using System.IO;

namespace Geisha.Framework.Rendering.Gdi
{
    public abstract class Renderer : IRenderer
    {
        protected readonly RenderingContext RenderingContext;

        protected Renderer(IRenderingContextFactory renderingContextFactory)
        {
            RenderingContext = (RenderingContext) renderingContextFactory.Create();
        }

        public ITexture CreateTexture(Stream stream)
        {
            return new Texture(stream);
        }

        public void Clear()
        {
            using (var graphics = Graphics.FromImage(RenderingContext.Bitmap))
            {
                graphics.Clear(System.Drawing.Color.White);
            }
        }
    }
}