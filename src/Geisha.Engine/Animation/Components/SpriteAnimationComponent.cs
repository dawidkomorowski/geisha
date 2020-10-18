using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Animation.Components
{
    public sealed class SpriteAnimationComponent : IComponent
    {
        private readonly Dictionary<string, SpriteAnimation> _animations = new Dictionary<string, SpriteAnimation>();
        private double _position;

        public IReadOnlyDictionary<string, SpriteAnimation> Animations => _animations;
        public (string Name, SpriteAnimation Animation)? CurrentAnimation { get; private set; }
        public bool IsPlaying { get; private set; }
        public double PlaybackSpeed { get; set; } = 1.0;

        public double Position
        {
            get => _position;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Position must be between 0.0 and 1.0.");

                _position = value;
            }
        }

        public event EventHandler<SpriteAnimationCompletedEventArgs>? AnimationCompleted;

        public void AddAnimation(string name, SpriteAnimation animation)
        {
            _animations.Add(name, animation);
        }

        public void RemoveAnimation(string name)
        {
            if (CurrentAnimation.HasValue && CurrentAnimation.Value.Name == name)
                throw new InvalidOperationException("Cannot remove current animation.");

            _animations.Remove(name);
        }

        public void PlayAnimation(string name)
        {
            CurrentAnimation = (name, _animations[name]);
            IsPlaying = true;
            Position = 0;
        }

        public void Resume()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = true;
        }

        public void Pause()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = false;
        }

        public void Stop()
        {
            ThrowIfThereIsNoCurrentAnimation();
            IsPlaying = false;
            Position = 0;
        }

        internal void OnAnimationCompleted(SpriteAnimationCompletedEventArgs e)
        {
            AnimationCompleted?.Invoke(this, e);
        }

        private void ThrowIfThereIsNoCurrentAnimation()
        {
            if (CurrentAnimation.HasValue == false)
                throw new InvalidOperationException("Cannot resume when there is no current animation.");
        }
    }

    public sealed class SpriteAnimationCompletedEventArgs : EventArgs
    {
        public SpriteAnimationCompletedEventArgs(string animationName, SpriteAnimation animation)
        {
            AnimationName = animationName;
            Animation = animation;
        }

        public string AnimationName { get; }
        public SpriteAnimation Animation { get; }
    }
}