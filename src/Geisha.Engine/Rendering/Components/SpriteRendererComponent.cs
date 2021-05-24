using Geisha.Engine.Core.Assets;
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

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            if (Sprite == null)
            {
                componentDataWriter.WriteNull("Sprite");
            }
            else
            {
                componentDataWriter.WriteAssetId("Sprite", assetStore.GetAssetId(Sprite));
            }
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            Sprite = componentDataReader.IsNull("Sprite")
                ? null
                : assetStore.GetAsset<Sprite>(componentDataReader.ReadAssetId("Sprite"));
        }
    }

    internal sealed class SpriteRendererComponentFactory : ComponentFactory<SpriteRendererComponent>
    {
        protected override SpriteRendererComponent CreateComponent() => new SpriteRendererComponent();
    }
}