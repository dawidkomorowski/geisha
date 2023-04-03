using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     <see cref="SpriteRendererComponent" /> gives an <see cref="Entity" /> capability of rendering a sprite.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.SpriteRendererComponent")]
    public sealed class SpriteRendererComponent : Renderer2DComponent
    {
        private double _opacity = 1d;

        internal SpriteRendererComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Gets or sets <see cref="Sprite" /> to be rendered.
        /// </summary>
        public Sprite? Sprite { get; set; }

        /// <summary>
        ///     Gets or sets opacity of sprite. Valid range is from 0.0 meaning fully transparent to 1.0 meaning fully opaque.
        ///     Default value is <c>1.0</c>.
        /// </summary>
        public double Opacity
        {
            get => _opacity;
            set => _opacity = Math.Clamp(value, 0d, 1d);
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
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Sprite = reader.IsNull("Sprite")
                ? null
                : assetStore.GetAsset<Sprite>(reader.ReadAssetId("Sprite"));
        }
    }

    internal sealed class SpriteRendererComponentFactory : ComponentFactory<SpriteRendererComponent>
    {
        protected override SpriteRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}