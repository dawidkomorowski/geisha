using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation
{
    public sealed class SpriteAnimation
    {
        public SpriteAnimation(IEnumerable<SpriteAnimationFrame> frames)
        {
            Frames = frames.ToList().AsReadOnly();
        }

        public IReadOnlyList<SpriteAnimationFrame> Frames { get; }
    }

    public sealed class SpriteAnimationFrame
    {
        public SpriteAnimationFrame(Sprite sprite)
        {
            Sprite = sprite;
        }

        public Sprite Sprite { get; }
    }
}