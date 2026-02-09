using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     <see cref="SpriteRendererComponent" /> gives an <see cref="Entity" /> capability of rendering a sprite.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.SpriteRendererComponent")]
    public sealed class SpriteRendererComponent : Renderer2DComponent
    {
        internal SpriteRendererComponent(Entity entity) : base(entity, new DetachedSpriteNode())
        {
            Opacity = 1d;
            BitmapInterpolationMode = BitmapInterpolationMode.Linear;
        }

        internal ISpriteNode SpriteNode
        {
            get => (ISpriteNode)RenderNode;
            set => RenderNode = value;
        }

        /// <summary>
        ///     Gets or sets <see cref="Sprite" /> to be rendered.
        /// </summary>
        public Sprite? Sprite
        {
            get => SpriteNode.Sprite;
            set => SpriteNode.Sprite = value;
        }

        /// <summary>
        ///     Gets or sets opacity of sprite. Valid range is from 0.0 meaning fully transparent to 1.0 meaning fully opaque.
        ///     Default value is <c>1.0</c>.
        /// </summary>
        public double Opacity
        {
            get => SpriteNode.Opacity;
            set => SpriteNode.Opacity = Math.Clamp(value, 0d, 1d);
        }

        // TODO: Add xml documentation.
        // TODO: Add xml documentation to IRenderingContext2D interface about interpolation mode (especially for DrawSprite method).
        // TODO: Add xml documentation to SpriteBatch for BitmapInterpolationMode property.
        public BitmapInterpolationMode BitmapInterpolationMode
        {
            get => SpriteNode.BitmapInterpolationMode;
            set => SpriteNode.BitmapInterpolationMode = value;
        }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            if (Sprite == null)
            {
                writer.WriteNull("Sprite");
            }
            else
            {
                writer.WriteAssetId("Sprite", assetStore.GetAssetId(Sprite));
            }

            writer.WriteDouble("Opacity", Opacity);
            writer.WriteEnum("BitmapInterpolationMode", BitmapInterpolationMode);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Sprite = reader.IsNull("Sprite")
                ? null
                : assetStore.GetAsset<Sprite>(reader.ReadAssetId("Sprite"));
            Opacity = reader.ReadDouble("Opacity");
            BitmapInterpolationMode = reader.ReadEnum<BitmapInterpolationMode>("BitmapInterpolationMode");
        }
    }

    internal sealed class SpriteRendererComponentFactory : ComponentFactory<SpriteRendererComponent>
    {
        protected override SpriteRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}