using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
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

        [TestCase(50)]
        [TestCase(150)] // Time step longer than whole animation
        public void
            ProcessAnimations_ShouldAdvancePositionOfSpriteAnimationComponentToTheEndAndContinuePlayingFromBeginningAndNotifyWithEvent_WhenAnimationPlayedInLoop(
                int deltaTime)
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100));
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Position = 0.8;
            spriteAnimationComponent.PlayInLoop = true;

            object? eventSender = null;
            SpriteAnimationCompletedEventArgs? eventArgs = null;
            spriteAnimationComponent.AnimationCompleted += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(deltaTime)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.EqualTo(0.3).Within(1e-15));
            Assert.That(spriteAnimationComponent.IsPlaying, Is.True);
            Assert.That(eventSender, Is.Not.Null, "Event sender is null.");
            Assert.That(eventSender, Is.EqualTo(spriteAnimationComponent));
            Assert.That(eventArgs, Is.Not.Null, "Event args are null.");
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.AnimationName, Is.EqualTo("anim"));
            Assert.That(eventArgs.Animation, Is.EqualTo(spriteAnimation));
        }

        [Test]
        public void ProcessAnimations_ShouldInvokeAnimationCompletedAfterAdvancingPosition_WhenAnimationEnded()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var spriteAnimationComponent = builder.AddSpriteAnimationComponent();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100));
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Position = 0.8;

            spriteAnimationComponent.AnimationCompleted += (sender, args) => { spriteAnimationComponent.PlayAnimation("anim"); };

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(50)));

            // Assert
            Assert.That(spriteAnimationComponent.Position, Is.EqualTo(0.0));
            Assert.That(spriteAnimationComponent.IsPlaying, Is.True);
        }

        [TestCase(0.0, new[] { 1.0, 1.0 }, 0)]
        [TestCase(0.4, new[] { 1.0, 1.0 }, 0)]
        [TestCase(0.5, new[] { 1.0, 1.0 }, 1)]
        [TestCase(1.0, new[] { 1.0, 1.0 }, 1)]
        [TestCase(0.0, new[] { 3.0, 1.0, 4.0 }, 0)]
        [TestCase(0.2, new[] { 3.0, 1.0, 4.0 }, 0)]
        [TestCase(0.4, new[] { 3.0, 1.0, 4.0 }, 1)]
        [TestCase(0.6, new[] { 3.0, 1.0, 4.0 }, 2)]
        [TestCase(0.8, new[] { 3.0, 1.0, 4.0 }, 2)]
        [TestCase(1.0, new[] { 3.0, 1.0, 4.0 }, 2)]
        public void ProcessAnimations_ShouldComputeCurrentAnimationFrameOfSpriteAnimationComponentAndSetItAsSpriteOfSpriteRendererComponent(double position,
            double[] framesDurations, int expectedAnimationFrame)
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var (spriteAnimationComponent, spriteRendererComponent) = builder.AddSpriteAnimationAndRendererComponents();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100), framesDurations);
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Position = position;

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(0)));

            // Assert
            Assert.That(spriteRendererComponent.Sprite, Is.EqualTo(spriteAnimation.Frames[expectedAnimationFrame].Sprite));
        }

        [Test]
        public void ProcessAnimations_ShouldNotSetSpriteOfSpriteRendererComponent_WhenThereIsNoCurrentAnimation()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var (spriteAnimationComponent, spriteRendererComponent) = builder.AddSpriteAnimationAndRendererComponents();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100), new[] { 1.0, 1.0, 1.0 });
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(0)));

            // Assert
            Assert.That(spriteRendererComponent.Sprite, Is.Null);
        }

        [Test]
        [Description("Issue #235")]
        public void ProcessAnimations_ShouldSetFirstAnimationFrame_WhenAnimationWasStopped()
        {
            // Arrange
            var builder = new AnimationSceneBuilder();
            var (spriteAnimationComponent, spriteRendererComponent) = builder.AddSpriteAnimationAndRendererComponents();
            var spriteAnimation = CreateAnimation(TimeSpan.FromMilliseconds(100), new[] { 1.0, 1.0, 1.0 });
            spriteAnimationComponent.AddAnimation("anim", spriteAnimation);

            var scene = builder.Build();

            spriteAnimationComponent.PlayAnimation("anim");
            spriteAnimationComponent.Position = 0.5;
            spriteAnimationComponent.Stop();

            // Act
            _animationSystem.ProcessAnimations(scene, new GameTime(TimeSpan.FromMilliseconds(0)));

            // Assert
            Assert.That(spriteRendererComponent.Sprite, Is.EqualTo(spriteAnimation.Frames.First().Sprite));
        }

        private sealed class AnimationSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public SpriteAnimationComponent AddSpriteAnimationComponent()
            {
                var component = new SpriteAnimationComponent();

                var entity = new Entity();
                entity.AddComponent(component);
                _scene.AddEntity(entity);

                return component;
            }

            public (SpriteAnimationComponent, SpriteRendererComponent) AddSpriteAnimationAndRendererComponents()
            {
                var spriteAnimationComponent = new SpriteAnimationComponent();
                var spriteRendererComponent = new SpriteRendererComponent();

                var entity = new Entity();
                entity.AddComponent(spriteAnimationComponent);
                entity.AddComponent(spriteRendererComponent);
                _scene.AddEntity(entity);

                return (spriteAnimationComponent, spriteRendererComponent);
            }

            public Scene Build() => _scene;
        }

        private static SpriteAnimation CreateAnimation(TimeSpan duration) => CreateAnimation(duration, new[] { 1.0, 1.0 });

        private static SpriteAnimation CreateAnimation(TimeSpan duration, IEnumerable<double> framesDurations)
        {
            var frames = framesDurations.Select(frameDuration =>
            {
                var texture = Substitute.For<ITexture>();
                var sprite = new Sprite(texture, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
                return new SpriteAnimationFrame(sprite, frameDuration);
            }).ToList();

            return new SpriteAnimation(frames, duration);
        }
    }
}