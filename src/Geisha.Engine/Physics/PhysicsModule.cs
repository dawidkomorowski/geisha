using Autofac;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics
{
    internal sealed class PhysicsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Components
            builder.RegisterType<CircleColliderComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<RectangleColliderComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<TileColliderComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<KinematicRigidBody2DComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<PhysicsSystem>().As<IPhysicsGameLoopStep>().As<ISceneObserver>().SingleInstance();
        }
    }
}