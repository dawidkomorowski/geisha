using System;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Animation.Components.Serialization
{
    internal sealed class SerializableSpriteAnimationComponentMapper
        : SerializableComponentMapperAdapter<SpriteAnimationComponent, SerializableSpriteAnimationComponent>
    {
        private readonly IAssetStore _assetStore;

        public SerializableSpriteAnimationComponentMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SerializableSpriteAnimationComponent MapToSerializable(SpriteAnimationComponent component)
        {
            return new SerializableSpriteAnimationComponent
            {
                Animations = component.Animations.ToDictionary(kvp => kvp.Key, kvp => _assetStore.GetAssetId(kvp.Value).Value),
                CurrentAnimation = component.CurrentAnimation switch
                {
                    null => null,
                    _ => (component.CurrentAnimation.Value.Name, _assetStore.GetAssetId(component.CurrentAnimation.Value.Animation).Value)
                },
                IsPlaying = component.IsPlaying,
                Position = component.Position,
                PlaybackSpeed = component.PlaybackSpeed
            };
        }

        protected override SpriteAnimationComponent MapFromSerializable(SerializableSpriteAnimationComponent serializableComponent)
        {
            var component = new SpriteAnimationComponent();

            if (serializableComponent.Animations == null)
                throw new ArgumentException(
                    $"{nameof(SerializableSpriteAnimationComponent)}.{nameof(SerializableSpriteAnimationComponent.Animations)} cannot be null.");

            foreach (var (name, animationAssetId) in serializableComponent.Animations)
            {
                component.AddAnimation(name, _assetStore.GetAsset<SpriteAnimation>(new AssetId(animationAssetId)));
            }

            if (serializableComponent.CurrentAnimation != null)
            {
                component.PlayAnimation(serializableComponent.CurrentAnimation.Value.Name);
            }

            component.Position = serializableComponent.Position;
            component.PlaybackSpeed = serializableComponent.PlaybackSpeed;

            return component;
        }
    }
}