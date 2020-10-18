using System;
using System.Diagnostics;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Systems
{
    [TestFixture]
    public class AnimationSystemTests
    {
        private AnimationSystem _animationSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _animationSystem = new AnimationSystem();
        }

        [Test]
        public void ProcessAnimations_ShouldNotAdvancePositionOfSpriteAnimationComponent_WhenThereIsNoCurrentAnimation()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            spriteAnimationComponent.AddAnimation("anim", CreateAnimation(TimeSpan.FromMilliseconds(20)));

            var scene = builder.Build();

            // Assume
            Assume.That(spriteAnimationComponent.Position, Is.Zero);

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(10)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.Zero);
        }

        [Test]
        public void ProcessAnimations_ShouldNotAdvancePositionOfSpriteAnimationComponent_WhenCurrentAnimationIsNotPlaying()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            spriteAnimationComponent.AddAnimation("anim", CreateAnimation(TimeSpan.FromMilliseconds(20)));

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Stop();

            // Assume
            Assume.That(spriteAnimationComponent.Position, Is.Zero);

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(10)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.Zero);
        }

        // deltaTime relation to animationDuration
        [TestCase(10, 0, 1.0, 0.0, 0.0)]
        [TestCase(20, 10, 1.0, 0.0, 0.5)]
        [TestCase(30, 30, 1.0, 0.0, 1.0)]
        // initialPosition is not 0.0
        [TestCase(20, 10, 1.0, 0.3, 0.8)]
        // playbackSpeed is not 1.0
        [TestCase(100, 20, 0.5, 0.0, 0.1)]
        [TestCase(100, 20, 2.0, 0.0, 0.4)]
        // position reaches the end
        [TestCase(100, 50, 1.0, 0.8, 1.0)]
        public void ProcessAnimations_ShouldAdvancePositionOfSpriteAnimationComponent(int animationDuration, int deltaTime, double playbackSpeed,
            double initialPosition, double expectedPosition)
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            spriteAnimationComponent.AddAnimation("anim", CreateAnimation(TimeSpan.FromMilliseconds(animationDuration)));

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.PlaybackSpeed = playbackSpeed;
            spriteAnimationComponent.Position = initialPosition;

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(deltaTime)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.EqualTo(expectedPosition));
        }

        [Test]
        public void ProcessAnimations_ShouldAdvancePositionOfSpriteAnimationComponentToTheEndAndStopAnimationAndNotifyWithEvent()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100));
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Position = 0.8;

            object? eventSender = null;
            SpriteAnimationCompletedEventArgs? eventArgs = null;
            spriteAnimationComponent.AnimationCompleted += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(50)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.EqualTo(1.0));
            Assert.That(spriteAnimationComponent.IsPlaying, Is.False);
            Assert.That(eventSender, Is.Not.Null, "Event sender is null.");
            Assert.That(eventSender, Is.EqualTo(spriteAnimationComponent));
            Assert.That(eventArgs, Is.Not.Null, "Event args are null.");
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.AnimationName, Is.EqualTo("anim"));
            Assert.That(eventArgs.Animation, Is.EqualTo(spriteAnimation));
        }

        private sealed class AnimationSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public SpriteAnimationComponent AddSpriteAnimationComponent()
            {
                var component = new SpriteAnimationComponent();

                var entity = new Entity();
                entity.AddComponent(component);
                _scene.AddEntity(entity);

                return component;
            }

            public Scene Build() => _scene;
        }

        private static SpriteAnimation CreateAnimation(TimeSpan duration)
        {
            var texture1 = Substitute.For<ITexture>();
            var sprite1 = new Sprite(texture1);
            var frame1 = new SpriteAnimationFrame(sprite1, 1);

            var texture2 = Substitute.For<ITexture>();
            var sprite2 = new Sprite(texture2);
            var frame2 = new SpriteAnimationFrame(sprite2, 1);

            var frames = new[] {frame1, frame2};
            return new SpriteAnimation(frames, duration);
        }
    }
}