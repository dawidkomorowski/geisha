using Autofac;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Components.Serialization;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics
{
    /// <summary>
    ///     Provides physics system and related components.
    /// </summary>
    public sealed class PhysicsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Components
            builder.RegisterType<SerializableCircleColliderComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            builder.RegisterType<SerializableRectangleColliderComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            builder.RegisterType<PhysicsSystem>().As<IPhysicsSystem>().SingleInstance();
        }
    }
}