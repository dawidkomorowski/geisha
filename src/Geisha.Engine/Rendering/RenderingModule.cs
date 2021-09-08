using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Components;
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
            builder.RegisterType<DebugRenderer>().As<IDebugRenderer>().As<IDebugRendererForRenderingSystem>().SingleInstance();

            // Assets
            builder.RegisterType<SpriteAssetLoader>().As<IAssetLoader>().SingleInstance();
            builder.RegisterType<TextureAssetLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            builder.RegisterType<CameraComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<EllipseRendererComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<RectangleRendererComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<SpriteRendererComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<TextRendererComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<RenderingSystem>().As<IRenderingSystem>().SingleInstance();
        }
    }
}