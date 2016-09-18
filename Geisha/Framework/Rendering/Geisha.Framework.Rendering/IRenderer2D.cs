using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer2D : IRenderer
    {
        void Render(ITexture texture, int x, int y);
    }
}
