using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    public class TextRenderer : RendererBase
    {
        public string Text { get; set; }
        public int FontSize { get; set; } = 16;
        public Color Color { get; set; }
    }
}