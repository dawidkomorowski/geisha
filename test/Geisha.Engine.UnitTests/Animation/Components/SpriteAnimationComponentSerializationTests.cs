using System;
using System.Linq;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Components
{
    [TestFixture]
    public class SpriteAnimationComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize_WhenCurrentAnimationIsNotNull()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();
            const string animationName = "animation";
            const double position = 0.7;
            const double playbackSpeed = 1.3;
            const bool playInLoop = true;

            AssetStore.GetAssetId(animation).Returns(animationAssetId);
            AssetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = SerializeAndDeserialize<SpriteAnimationComponent>(component =>
            {
                component.AddAnimation(animationName, animation);
                component.PlayAnimation(animationName);
                component.Position = position;
                component.PlaybackSpeed = playbackSpeed;
                component.PlayInLoop = playInLoop;
            });

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Assert.That(actual.Animations.Single().Key, Is.EqualTo(animationName));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Not.Null);
            Assert.That(actual.CurrentAnimation!.Value.Name, Is.EqualTo(animationName));
            Assert.That(actual.CurrentAnimation.Value.Animation, Is.EqualTo(animation));
            Assert.That(actual.IsPlaying, Is.True);
            Assert.That(actual.Position, Is.EqualTo(position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(playbackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(playInLoop));
        }

        [Test]
        public void SerializeAndDeserialize_WhenCurrentAnimationIsNull()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();
            const string animationName = "animation";
            const double position = 0.7;
            const double playbackSpeed = 1.3;
            const bool playInLoop = true;

            AssetStore.GetAssetId(animation).Returns(animationAssetId);
            AssetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = SerializeAndDeserialize<SpriteAnimationComponent>(component =>
            {
                component.AddAnimation(animationName, animation);
                component.Position = position;
                component.PlaybackSpeed = playbackSpeed;
                component.PlayInLoop = playInLoop;
            });

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Assert.That(actual.Animations.Single().Key, Is.EqualTo(animationName));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Null);
            Assert.That(actual.IsPlaying, Is.False);
            Assert.That(actual.Position, Is.EqualTo(position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(playbackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(playInLoop));
        }

        private static SpriteAnimation CreateAnimation()
        {
            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(texture, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
            var frames = new[] { new SpriteAnimationFrame(sprite, 1) };
            return new SpriteAnimation(frames, TimeSpan.FromSeconds(1));
        }
    }
}