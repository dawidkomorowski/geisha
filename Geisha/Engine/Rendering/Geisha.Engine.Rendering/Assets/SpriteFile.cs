using Geisha.Common.Math.Definition;

namespace Geisha.Engine.Rendering.Assets
{
    // TODO Add docs after adding them to Sprite
    public class SpriteFile
    {
        public string SourceTextureFilePath { get; set; }
        public Vector2Definition SourceUV { get; set; }
        public Vector2Definition SourceDimension { get; set; }
        public Vector2Definition SourceAnchor { get; set; }
        public double PixelsPerUnit { get; set; }
    }
}