using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Components.Definition;
using Geisha.Engine.Input.Systems;

namespace Geisha.Engine.Input
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Input";
        public string Description => "Provides input system and related components.";
        public string Category => "Input";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            // Assets
            containerBuilder.RegisterType<InputMappingLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            containerBuilder.RegisterType<InputComponentDefinitionMapper>().As<IComponentDefinitionMapper>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<InputSystem>().As<IFixedTimeStepSystem>().SingleInstance();
        }
    }
}