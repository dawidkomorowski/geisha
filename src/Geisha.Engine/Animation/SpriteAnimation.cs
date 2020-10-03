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

    /// <summary>
    ///     Single frame of <see cref="SpriteAnimation" />. It is composed of <see cref="Sprite" /> and relative duration.
    /// </summary>
    public sealed class SpriteAnimationFrame
    {
        /// <summary>
        ///     Creates new instance of <see cref="SpriteAnimationFrame" /> composed of provided <see cref="Rendering.Sprite" />
        ///     and of specified duration.
        /// </summary>
        /// <param name="sprite"><see cref="Rendering.Sprite" /> instance to be used as image for animation frame.</param>
        /// <param name="duration">
        ///     Duration of frame relative to other frames composing whole animation. Duration must be greater
        ///     than zero.
        /// </param>
        public SpriteAnimationFrame(Sprite sprite, double duration)
        {
            if (duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration), "Value must be greater than zero.");

            Sprite = sprite;
            Duration = duration;
        }

        /// <summary>
        ///     <see cref="Rendering.Sprite" /> instance defining 2D image of animation frame.
        /// </summary>
        public Sprite Sprite { get; }

        /// <summary>
        ///     Duration of frame relative to other frames composing whole animation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         For example if animation is composed of 4 frames all with duration of 1.0 then each frame will last the same
        ///         amount of time. For animation with total duration of 2 seconds each frame would last for 0.5 second.
        ///     </para>
        ///     <para>
        ///         If animation is composed of 4 frames where first is of duration 5.0 and rest is of duration 1.0 then first
        ///         frame would last 5 times longer in comparison to frames with duration of 1.0. As a result for animation with
        ///         total duration of 2 seconds first frame would last for 1.25 second and rest of frames would last for 0.25
        ///         second.
        ///     </para>
        /// </remarks>
        public double Duration { get; }
    }
}