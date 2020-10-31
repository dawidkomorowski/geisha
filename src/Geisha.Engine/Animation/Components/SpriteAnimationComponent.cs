using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;

namespace Geisha.Engine.Animation.Components
{
    /// <summary>
    ///     <see cref="SpriteAnimationComponent" /> gives an <see cref="Entity" /> capability of playing sprite based
    ///     animations.
    /// </summary>
    /// <remarks>
    ///     <see cref="SpriteAnimationComponent" /> is responsible for processing animation and determining current animation
    ///     frame. For the frame to be presented <see cref="Rendering.Components.SpriteRendererComponent" /> needs to be
    ///     present on the same entity as the <see cref="SpriteAnimationComponent" />.
    /// </remarks>
    public sealed class SpriteAnimationComponent : IComponent
    {
        private readonly Dictionary<string, SpriteAnimation> _animations = new Dictionary<string, SpriteAnimation>();
        private double _position;

        /// <summary>
        ///     Dictionary of all animations added to the component. Keys are names of animations and values are
        ///     corresponding <see cref="SpriteAnimation" /> assets.
        /// </summary>
        public IReadOnlyDictionary<string, SpriteAnimation> Animations => _animations;

        /// <summary>
        ///     Currently playing animation. If no animation is playing the value is <c>null</c>.
        /// </summary>
        /// <remarks><see cref="CurrentAnimation" /> is set by <see cref="PlayAnimation" />.</remarks>
        public (string Name, SpriteAnimation Animation)? CurrentAnimation { get; private set; }

        /// <summary>
        ///     Indicates whether current animation is playing or not.
        /// </summary>
        /// <value><c>true</c> if animation is playing; <c>false</c> otherwise.</value>
        public bool IsPlaying { get; private set; }

        /// <summary>
        ///     Gets or sets playback speed of animation.
        /// </summary>
        /// <value>Default value is <c>1.0</c>.</value>
        public double PlaybackSpeed { get; set; } = 1.0;

        /// <summary>
        ///     Gets or sets value indicating if animation should be played in a loop.
        /// </summary>
        /// <value><c>true</c> if animation is played in a loop; <c>false</c> otherwise. The default is <c>false</c>.</value>
        /// <remarks>Animation playing in a loop is automatically replayed from the beginning when it reaches the end.</remarks>
        public bool PlayInLoop { get; set; }

        /// <summary>
        ///     Gets or sets position in current animation. Animation begins with value <c>0.0</c> and ends with value <c>1.0</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><c>value</c> is lower than <c>0.0</c> or greater than <c>1.0</c>.</exception>
        public double Position
        {
            get => _position;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Position must be between 0.0 and 1.0.");

                _position = value;
            }
        }

        /// <summary>
        ///     Occurs when current animation is completed.
        /// </summary>
        /// <remarks>
        ///     This event is raised for animations that reach the end and stop and for animations that reach the end and
        ///     continue playing in a loop.
        /// </remarks>
        public event EventHandler<SpriteAnimationCompletedEventArgs>? AnimationCompleted;

        /// <summary>
        ///     Adds <see cref="SpriteAnimation" /> with specified <paramref name="name" /> to
        ///     <see cref="SpriteAnimationComponent" />.
        /// </summary>
        /// <param name="name">Name of the animation to be used for referencing this particular animation.</param>
        /// <param name="animation"><see cref="SpriteAnimation" /> instance to be added to <see cref="SpriteAnimationComponent" />.</param>
        public void AddAnimation(string name, SpriteAnimation animation)
        {
            _animations.Add(name, animation);
        }

        /// <summary>
        ///     Removes animation with specified <paramref name="name" /> from <see cref="SpriteAnimationComponent" />.
        /// </summary>
        /// <param name="name">Name of animation to be removed from <see cref="SpriteAnimationComponent" />.</param>
        /// <exception cref="InvalidOperationException"><paramref name="name" /> specifies <see cref="CurrentAnimation" />.</exception>
        public void RemoveAnimation(string name)
        {
            if (CurrentAnimation.HasValue && CurrentAnimation.Value.Name == name)
                throw new InvalidOperationException("Cannot remove current animation.");

            _animations.Remove(name);
        }

        /// <summary>
        ///     Starts playing animation with specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">Name of animation to be played.</param>
        /// <remarks>This method resets <see cref="Position" /> to the beginning. This method sets <see cref="CurrentAnimation" />.</remarks>
        public void PlayAnimation(string name)
        {
            CurrentAnimation = (name, _animations[name]);
            IsPlaying = true;
            Position = 0;
        }

        /// <summary>
        ///     Resumes playing <see cref="CurrentAnimation" /> from current <see cref="Position" />.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="CurrentAnimation" /> is <c>null</c>.</exception>
        public void Resume()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = true;
        }

        /// <summary>
        ///     Pauses playing <see cref="CurrentAnimation" /> at current <see cref="Position" />.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="CurrentAnimation" /> is <c>null</c>.</exception>
        public void Pause()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = false;
        }

        /// <summary>
        ///     Stops playing <see cref="CurrentAnimation" /> and resets <see cref="Position" /> to the beginning.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="CurrentAnimation" /> is <c>null</c>.</exception>
        public void Stop()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = false;
            Position = 0;
        }

        internal void AdvanceAnimation(TimeSpan deltaTime)
        {
            if (!CurrentAnimation.HasValue || !IsPlaying) return;

            var currentAnimation = CurrentAnimation.Value.Animation;
            var positionDelta = deltaTime / currentAnimation.Duration;
            var finalPosition = Position + positionDelta * PlaybackSpeed;

            var reachedTheEnd = finalPosition >= 1.0;
            if (reachedTheEnd)
            {
                if (PlayInLoop)
                {
                    finalPosition %= 1.0;
                }
                else
                {
                    finalPosition = 1.0;
                    IsPlaying = false;
                }
            }

            Position = finalPosition;

            if (reachedTheEnd)
            {
                var currentAnimationName = CurrentAnimation.Value.Name;
                OnAnimationCompleted(new SpriteAnimationCompletedEventArgs(currentAnimationName, currentAnimation));
            }
        }

        internal Sprite? ComputeCurrentAnimationFrame()
        {
            if (CurrentAnimation.HasValue == false) return null;

            var animationFrames = CurrentAnimation.Value.Animation.Frames;

            var totalFramesDuration = animationFrames.Sum(animationFrame => animationFrame.Duration);
            var positionInTotalDuration = totalFramesDuration * Position;

            var currentFrameEndPosition = 0.0;
            foreach (var animationFrame in animationFrames)
            {
                currentFrameEndPosition += animationFrame.Duration;
                if (currentFrameEndPosition > positionInTotalDuration) return animationFrame.Sprite;
            }

            return animationFrames[^1].Sprite;
        }

        private void OnAnimationCompleted(SpriteAnimationCompletedEventArgs e)
        {
            AnimationCompleted?.Invoke(this, e);
        }

        private void ThrowIfThereIsNoCurrentAnimation()
        {
            if (CurrentAnimation.HasValue == false)
                throw new InvalidOperationException("Operation cannot be executed when there is no current animation.");
        }
    }

    /// <summary>
    ///     Provides data for <see cref="SpriteAnimationComponent.AnimationCompleted" /> event.
    /// </summary>
    public sealed class SpriteAnimationCompletedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes new instance of the <see cref="SpriteAnimationCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="animationName">Name of animation that has completed.</param>
        /// <param name="animation"><see cref="SpriteAnimation" /> that has completed.</param>
        public SpriteAnimationCompletedEventArgs(string animationName, SpriteAnimation animation)
        {
            AnimationName = animationName;
            Animation = animation;
        }

        /// <summary>
        ///     Name of animation that has completed.
        /// </summary>
        public string AnimationName { get; }

        /// <summary>
        ///     <see cref="SpriteAnimation" /> that has completed.
        /// </summary>
        public SpriteAnimation Animation { get; }
    }
}