using System.Drawing;
using System.IO;

namespace Geisha.Framework.Rendering.Gdi
{
    public abstract class Renderer : IRenderer
    {
        protected readonly RenderingContext InternalRenderingContext;

        protected Renderer(IRenderingContextFactory renderingContextFactory)
        {
            InternalRenderingContext = (RenderingContext) renderingContextFactory.Create();
        }

        public IRenderingContext RenderingContext => InternalRenderingContext;

        public ITexture CreateTexture(Stream stream)
        {
            return new Texture(stream);
        }

        public void Clear()
        {
            using (var graphics = Graphics.FromImage(InternalRenderingContext.Bitmap))
            {
                graphics.Clear(System.Drawing.Color.White);
            }
        }
    }
}