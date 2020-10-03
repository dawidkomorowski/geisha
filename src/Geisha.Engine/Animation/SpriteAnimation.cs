using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation
{
    /// <summary>
    ///     Sprite animation is sequence of sprites with time duration that compose 2D animated image.
    /// </summary>
    public sealed class SpriteAnimation
    {
        /// <summary>
        ///     Creates new instance of <see cref="SpriteAnimation" /> composed of provided frames and of specified time duration.
        /// </summary>
        /// <param name="frames">Subsequent frames of animation in order of specification. There must be at least one frame.</param>
        /// <param name="duration">Total duration of animation. Duration must be greater than zero.</param>
        public SpriteAnimation(IReadOnlyCollection<SpriteAnimationFrame> frames, TimeSpan duration)
        {
            if (frames.Count == 0) throw new ArgumentException($"{nameof(SpriteAnimation)} must consist of at least one frame.", nameof(frames));
            if (duration.Ticks <= 0) throw new ArgumentOutOfRangeException(nameof(duration), "Value must be greater than zero.");

            Duration = duration;
            Frames = frames.ToList().AsReadOnly();
        }

        /// <summary>
        ///     List of subsequent animation frames.
        /// </summary>
        /// <remarks>Animation is played according to order of frames in this list.</remarks>
        public IReadOnlyList<SpriteAnimationFrame> Frames { get; }

        /// <summary>
        ///     Total duration of animation.
        /// </summary>
        /// <remarks>
        ///     It is reference duration for playback speed of 1.0. If animation is played at different speed total time of
        ///     playback will differ accordingly.
        /// </remarks>
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