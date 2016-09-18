using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer
    {
        ITexture CreateTexture(Stream stream);
        void Clear();
    }
}
