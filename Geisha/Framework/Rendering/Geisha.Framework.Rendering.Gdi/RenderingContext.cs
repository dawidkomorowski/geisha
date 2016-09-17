using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public class RenderingContext
    {
        private readonly Bitmap _bitmap;

        public RenderingContext(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public Bitmap Bitmap => _bitmap;
    }
}
