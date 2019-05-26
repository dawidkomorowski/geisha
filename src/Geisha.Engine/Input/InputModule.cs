using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Components.Serialization;
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
            builder.RegisterType<SerializableInputComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            builder.RegisterType<InputSystem>().As<IFixedTimeStepSystem>().SingleInstance();
        }
    }
}