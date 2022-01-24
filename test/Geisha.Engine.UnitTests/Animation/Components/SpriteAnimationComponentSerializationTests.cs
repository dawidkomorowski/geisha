using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Components
{
    [TestFixture]
    public class SpriteAnimationComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new SpriteAnimationComponentFactory();

        [Test]
        public void SerializeAndDeserialize_WhenCurrentAnimationIsNotNull()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();

            var component = new SpriteAnimationComponent();
            component.AddAnimation("animation", animation);
            component.PlayAnimation("animation");
            component.Position = 0.7;
            component.PlaybackSpeed = 1.3;
            component.PlayInLoop = true;

            AssetStore.GetAssetId(animation).Returns(animationAssetId);
            AssetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Assert.That(actual.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Not.Null);
            Assert.That(actual.CurrentAnimation!.Value.Name, Is.EqualTo("animation"));
            Assert.That(actual.CurrentAnimation.Value.Animation, Is.EqualTo(animation));
            Assert.That(actual.IsPlaying, Is.True);
            Assert.That(actual.Position, Is.EqualTo(component.Position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(component.PlaybackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(component.PlayInLoop));
        }

        [Test]
        public void SerializeAndDeserialize_WhenCurrentAnimationIsNull()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();

            var component = new SpriteAnimationComponent();
            component.AddAnimation("animation", animation);
            component.Position = 0.7;
            component.PlaybackSpeed = 1.3;
            component.PlayInLoop = true;

            AssetStore.GetAssetId(animation).Returns(animationAssetId);
            AssetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Assert.That(actual.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Null);
            Assert.That(actual.IsPlaying, Is.False);
            Assert.That(actual.Position, Is.EqualTo(component.Position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(component.PlaybackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(component.PlayInLoop));
        }

        private static SpriteAnimation CreateAnimation()
        {
            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(texture, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
            var frames = new[] {new SpriteAnimationFrame(sprite, 1)};
            return new SpriteAnimation(frames, TimeSpan.FromSeconds(1));
        }
    }
}