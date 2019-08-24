using Autofac;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;
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
            // Register common components
            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

            // Register engine modules
            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<InputModule>();
            containerBuilder.RegisterModule<PhysicsModule>();
            containerBuilder.RegisterModule<AudioModule>();
            containerBuilder.RegisterModule<RenderingModule>();
        }
    }
}