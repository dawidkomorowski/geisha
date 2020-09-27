using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation
{
    public sealed class SpriteAnimation
    {
        public SpriteAnimation(IEnumerable<SpriteAnimationFrame> frames, TimeSpan duration)
        {
            if (duration.Ticks <= 0) throw new ArgumentOutOfRangeException(nameof(duration), "Value must be greater than zero.");

            Duration = duration;
            Frames = frames.ToList().AsReadOnly();

            if (Frames.Count == 0) throw new ArgumentException($"{nameof(SpriteAnimation)} must consist of at least one frame.", nameof(frames));
        }

        public IReadOnlyList<SpriteAnimationFrame> Frames { get; }
        public TimeSpan Duration { get; }
    }

    public sealed class SpriteAnimationFrame
    {
        public SpriteAnimationFrame(Sprite sprite, double duration)
        {
            if (duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration), "Value must be greater than zero.");

            Sprite = sprite;
            Duration = duration;
        }

        public Sprite Sprite { get; }
        public double Duration { get; }
    }
}