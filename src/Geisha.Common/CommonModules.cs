using Autofac;
using Geisha.Common.FileSystem;
using Geisha.Common.Serialization;

namespace Geisha.Common
{
    public static class CommonModules
    {
        public static void RegisterAll(ContainerBuilder containerBuilder)
        {
            // Register common components
            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();
            containerBuilder.RegisterType<FileSystem.FileSystem>().As<IFileSystem>().SingleInstance();
        }
    }
}