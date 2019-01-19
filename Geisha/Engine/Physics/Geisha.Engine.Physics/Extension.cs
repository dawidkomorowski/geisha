using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Components.Serialization;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Physics";
        public string Description => "Provides physics system and related components.";
        public string Category => "Physics";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            // Components
            containerBuilder.RegisterType<CircleColliderDefinitionMapper>().As<ISerializableComponentMapper>().SingleInstance();
            containerBuilder.RegisterType<RectangleColliderDefinitionMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<PhysicsSystem>().As<IFixedTimeStepSystem>().SingleInstance();
        }
    }
}