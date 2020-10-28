using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Animation.Components.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Components.Serialization
{
    [TestFixture]
    public class SerializableSpriteAnimationComponentMapperTests
    {
        private IAssetStore _assetStore = null!;
        private SerializableSpriteAnimationComponentMapper _mapper = null!;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new SerializableSpriteAnimationComponentMapper(_assetStore);
        }

        [Test]
        public void MapToSerializable()
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

            _assetStore.GetAssetId(animation).Returns(animationAssetId);

            // Act
            var actual = (SerializableSpriteAnimationComponent) _mapper.MapToSerializable(component);

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Debug.Assert(actual.Animations != null, "actual.Animations != null");
            Assert.That(actual.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animationAssetId.Value));
            Assert.That(actual.CurrentAnimation, Is.Not.Null);
            Debug.Assert(actual.CurrentAnimation != null, "actual.CurrentAnimation != null");
            Assert.That(actual.CurrentAnimation.Value.Name, Is.EqualTo("animation"));
            Assert.That(actual.CurrentAnimation.Value.Animation, Is.EqualTo(animationAssetId.Value));
            Assert.That(actual.IsPlaying, Is.True);
            Assert.That(actual.Position, Is.EqualTo(component.Position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(component.PlaybackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(component.PlayInLoop));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();

            var serializableComponent = new SerializableSpriteAnimationComponent
            {
                Animations = new Dictionary<string, Guid> {{"animation", animationAssetId.Value}},
                CurrentAnimation = ("animation", animationAssetId.Value),
                IsPlaying = true,
                Position = 0.7,
                PlaybackSpeed = 1.3,
                PlayInLoop = true
            };

            _assetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = (SpriteAnimationComponent) _mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Debug.Assert(actual.Animations != null, "actual.Animations != null");
            Assert.That(actual.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Not.Null);
            Debug.Assert(actual.CurrentAnimation != null, "actual.CurrentAnimation != null");
            Assert.That(actual.CurrentAnimation.Value.Name, Is.EqualTo("animation"));
            Assert.That(actual.CurrentAnimation.Value.Animation, Is.EqualTo(animation));
            Assert.That(actual.IsPlaying, Is.True);
            Assert.That(actual.Position, Is.EqualTo(serializableComponent.Position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(serializableComponent.PlaybackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(serializableComponent.PlayInLoop));
        }

        [Test]
        public void MapFromSerializable_CurrentAnimation_IsNull()
        {
            // Arrange
            var animation = CreateAnimation();
            var animationAssetId = AssetId.CreateUnique();

            var serializableComponent = new SerializableSpriteAnimationComponent
            {
                Animations = new Dictionary<string, Guid> {{"animation", animationAssetId.Value}},
                CurrentAnimation = null,
                IsPlaying = false,
                Position = 0.7,
                PlaybackSpeed = 1.3,
                PlayInLoop = true
            };

            _assetStore.GetAsset<SpriteAnimation>(animationAssetId).Returns(animation);

            // Act
            var actual = (SpriteAnimationComponent) _mapper.MapFromSerializable(serializableComponent);

            // Assert
            Assert.That(actual.Animations, Has.Count.EqualTo(1));
            Debug.Assert(actual.Animations != null, "actual.Animations != null");
            Assert.That(actual.Animations.Single().Key, Is.EqualTo("animation"));
            Assert.That(actual.Animations.Single().Value, Is.EqualTo(animation));
            Assert.That(actual.CurrentAnimation, Is.Null);
            Assert.That(actual.IsPlaying, Is.False);
            Assert.That(actual.Position, Is.EqualTo(serializableComponent.Position));
            Assert.That(actual.PlaybackSpeed, Is.EqualTo(serializableComponent.PlaybackSpeed));
            Assert.That(actual.PlayInLoop, Is.EqualTo(serializableComponent.PlayInLoop));
        }

        private static SpriteAnimation CreateAnimation()
        {
            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(texture);
            var frames = new[] {new SpriteAnimationFrame(sprite, 1)};
            return new SpriteAnimation(frames, TimeSpan.FromSeconds(1));
        }
    }
}