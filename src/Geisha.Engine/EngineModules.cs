using Autofac;
using Geisha.Engine.Animation;
using Geisha.Engine.Audio;
using Geisha.Engine.Core;
using Geisha.Engine.Input;
using Geisha.Engine.Physics;
using Geisha.Engine.Rendering;

namespace Geisha.Engine
{
    public static class EngineModules
    {
        public static void RegisterAll(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AnimationModule>();
            containerBuilder.RegisterModule<AudioModule>();
            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<InputModule>();
            containerBuilder.RegisterModule<PhysicsModule>();
            containerBuilder.RegisterModule<RenderingModule>();
        }
    }
}