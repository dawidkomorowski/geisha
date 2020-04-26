using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Components.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Provides rendering system and related components.
    /// </summary>
    public sealed class RenderingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SpriteAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            builder.RegisterType<SpriteManagedAssetFactory>().As<IManagedAssetFactory>().SingleInstance();
            builder.RegisterType<TextureAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            builder.RegisterType<TextureManagedAssetFactory>().As<IManagedAssetFactory>().SingleInstance();

            // Components
            builder.RegisterType<SerializableCameraComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            builder.RegisterType<SerializableEllipseRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            builder.RegisterType<SerializableRectangleRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            builder.RegisterType<SerializableSpriteRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            builder.RegisterType<SerializableTextRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            builder.RegisterType<RenderingSystem>().As<IRenderingSystem>().SingleInstance();
        }
    }
}