﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     <see cref="SpriteRendererComponent" /> gives an <see cref="Core.SceneModel.Entity" /> capability of rendering a
    ///     sprite.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.SpriteRendererComponent")]
    public sealed class SpriteRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Sprite to be rendered.
        /// </summary>
        public Sprite? Sprite { get; set; }

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
        protected override SpriteRendererComponent CreateComponent() => new SpriteRendererComponent();
    }
}