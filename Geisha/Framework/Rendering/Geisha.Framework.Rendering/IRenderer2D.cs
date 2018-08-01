using System.IO;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    // TODO Add documentation!
    public interface IRenderer2D
    {
        IRenderingContext RenderingContext { get; }
        ITexture CreateTexture(Stream stream);
        void Clear(Color color);
        void RenderSprite(Sprite sprite, Matrix3 transform);
        void RenderText(string text, int fontSize, Color color, Matrix3 transform);
    }
}