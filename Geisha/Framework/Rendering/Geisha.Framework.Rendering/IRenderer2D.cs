using System;
using System.IO;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    // TODO Add documentation!
    public interface IRenderer2D : IDisposable
    {
        IWindow Window { get; }
        ITexture CreateTexture(Stream stream);
        void BeginRendering();
        void EndRendering();
        void Clear(Color color);
        void RenderSprite(Sprite sprite, Matrix3 transform);
        void RenderText(string text, FontSize fontSize, Color color, Matrix3 transform);
    }
}