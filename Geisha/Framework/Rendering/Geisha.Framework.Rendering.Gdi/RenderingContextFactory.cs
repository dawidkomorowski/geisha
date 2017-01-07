using System.ComponentModel.Composition;
using System.Drawing;

namespace Geisha.Framework.Rendering.Gdi
{
    [Export(typeof(IRenderingContextFactory))]
    public class RenderingContextFactory : IRenderingContextFactory
    {
        public static Bitmap Bitmap { get; set; }

        public IRenderingContext Create()
        {
            return new RenderingContext(Bitmap);
        }
    }
}