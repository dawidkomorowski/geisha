using System;
using System.IO;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    // TODO Add documentation!
    public interface IRenderer2D : IDisposable
    {
        IRenderingContext RenderingContext { get; }
        ITexture CreateTexture(Stream stream);
        void BeginDraw();
        void EndDraw();
        void Clear(Color color);
        void RenderSprite(Sprite sprite, Matrix3 transform);
        void RenderText(string text, int fontSize, Color color, Matrix3 transform);
    }
}