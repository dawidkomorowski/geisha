using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Systems;

namespace Geisha.Engine.Input
{
    /// <summary>
    ///     Provides input system and related components.
    /// </summary>
    public sealed class InputModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<InputMappingAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            builder.RegisterType<InputMappingManagedAssetFactory>().As<IManagedAssetFactory>().SingleInstance();

            // Components
            builder.RegisterType<InputComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<InputSystem>().As<IInputSystem>().SingleInstance();
        }
    }
}