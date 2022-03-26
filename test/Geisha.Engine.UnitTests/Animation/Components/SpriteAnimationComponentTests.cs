using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Components
{
    [TestFixture]
    public class SpriteAnimationComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [TestCase(0.0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Position_ShouldBeSet_WhenValueIsBetweenZeroAndOne(double position)
        {
            // Arrange
            var component = Entity.CreateComponent<SpriteAnimationComponent>();

            // Act
            component.Position = position;

            // Assert
            Assert.That(component.Position, Is.EqualTo(position));
        }

        [TestCase(-0.1)]
        [TestCase(1.1)]
        public void Position_ShouldThrow_WhenValueIsOutOfRangeBetweenZeroAndOne(double position)
        {
            // Arrange
            var component = Entity.CreateComponent<SpriteAnimationComponent>();

            // Act
            // Assert
            Assert.That(() => { component.Position = position; }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void AddAnimation_ShouldAddAnimationToAnimations()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();

            // Assume
            Assume.That(component.Animations, Has.Count.Zero);

            // Act
            component.AddAnimation(name, animation);

            // Assert
            Assert.That(component.Animations, Has.Count.EqualTo(1));
            Assert.That(component.Animations, Contains.Key(name));
            Assert.That(component.Animations[name], Is.EqualTo(animation));
        }

        [Test]
        public void RemoveAnimation_ShouldRemoveAnimationFromAnimations()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);

            // Assume
            Assume.That(component.Animations, Has.Count.EqualTo(1));

            // Act
            component.RemoveAnimation(name);

            // Assert
            Assert.That(component.Animations, Has.Count.Zero);
        }

        [Test]
        public void RemoveAnimation_ShouldThrow_WhenAnimationToRemoveIsCurrentAnimation()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);
            component.PlayAnimation(name);

            // Act
            // Assert
            Assert.That(() => { component.RemoveAnimation(name); }, Throws.InvalidOperationException);
        }

        [Test]
        public void PlayAnimation_ShouldSetCurrentAnimation_WhenItIsNotSet()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);

            // Assume
            Assume.That(component.CurrentAnimation.HasValue, Is.False);

            // Act
            component.PlayAnimation(name);

            // Assert
            Assert.That(component.CurrentAnimation.HasValue, Is.True);
            Debug.Assert(component.CurrentAnimation != null, "component.CurrentAnimation != null");
            Assert.That(component.CurrentAnimation.Value.Name, Is.EqualTo(name));
            Assert.That(component.CurrentAnimation.Value.Animation, Is.EqualTo(animation));
        }

        [Test]
        public void PlayAnimation_ShouldChangeCurrentAnimation_WhenOtherAnimationWasSet()
        {
            // Arrange
            const string name1 = "animation 1";
            var animation1 = CreateAnimation();

            const string name2 = "animation 2";
            var animation2 = CreateAnimation();

            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name1, animation1);
            component.AddAnimation(name2, animation2);
            component.PlayAnimation(name1);

            // Assume
            Assume.That(component.CurrentAnimation.HasValue, Is.True);

            // Act
            component.PlayAnimation(name2);

            // Assert
            Assert.That(component.CurrentAnimation.HasValue, Is.True);
            Debug.Assert(component.CurrentAnimation != null, "component.CurrentAnimation != null");
            Assert.That(component.CurrentAnimation.Value.Name, Is.EqualTo(name2));
            Assert.That(component.CurrentAnimation.Value.Animation, Is.EqualTo(animation2));
        }

        [Test]
        public void PlayAnimation_ShouldSetIsPlayingToTrueAndPositionToZero()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);
            component.Position = 0.5;

            // Assume
            Assume.That(component.IsPlaying, Is.False);

            // Act
            component.PlayAnimation(name);

            // Assert
            Assert.That(component.IsPlaying, Is.True);
            Assert.That(component.Position, Is.Zero);
        }

        [Test]
        public void Resume_ShouldThrow_WhenThereIsNoCurrentAnimation()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);

            // Act
            // Assert
            Assert.That(() => component.Resume(), Throws.InvalidOperationException);
        }

        [Test]
        public void Resume_ShouldSetIsPlayingToTrueAndKeepCurrentPosition()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);
            component.PlayAnimation(name);
            component.Stop();
            component.Position = 0.5;

            // Assume
            Assume.That(component.IsPlaying, Is.False);

            // Act
            component.Resume();

            // Assert
            Assert.That(component.IsPlaying, Is.True);
            Assert.That(component.Position, Is.EqualTo(0.5));
        }

        [Test]
        public void Pause_ShouldThrow_WhenThereIsNoCurrentAnimation()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);

            // Act
            // Assert
            Assert.That(() => component.Pause(), Throws.InvalidOperationException);
        }

        [Test]
        public void Pause_ShouldSetIsPlayingToFalseAndKeepCurrentPosition()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);
            component.PlayAnimation(name);
            component.Position = 0.5;

            // Assume
            Assume.That(component.IsPlaying, Is.True);

            // Act
            component.Pause();

            // Assert
            Assert.That(component.IsPlaying, Is.False);
            Assert.That(component.Position, Is.EqualTo(0.5));
        }

        [Test]
        public void Stop_ShouldThrow_WhenThereIsNoCurrentAnimation()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);

            // Act
            // Assert
            Assert.That(() => component.Stop(), Throws.InvalidOperationException);
        }

        [Test]
        public void Stop_ShouldSetIsPlayingToFalseAndPositionToZero()
        {
            // Arrange
            const string name = "animation";
            var animation = CreateAnimation();
            var component = Entity.CreateComponent<SpriteAnimationComponent>();
            component.AddAnimation(name, animation);
            component.PlayAnimation(name);
            component.Position = 0.5;

            // Assume
            Assume.That(component.IsPlaying, Is.True);

            // Act
            component.Stop();

            // Assert
            Assert.That(component.IsPlaying, Is.False);
            Assert.That(component.Position, Is.Zero);
        }

        private static SpriteAnimation CreateAnimation()
        {
            var texture1 = Substitute.For<ITexture>();
            var sprite1 = new Sprite(texture1, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
            var frame1 = new SpriteAnimationFrame(sprite1, 1);

            var texture2 = Substitute.For<ITexture>();
            var sprite2 = new Sprite(texture2, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
            var frame2 = new SpriteAnimationFrame(sprite2, 1);

            var frames = new[] { frame1, frame2 };
            return new SpriteAnimation(frames, TimeSpan.FromSeconds(1));
        }
    }
}