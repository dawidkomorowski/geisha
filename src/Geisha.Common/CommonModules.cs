using Autofac;
using Geisha.Common.FileSystem;

namespace Geisha.Common
{
    // TODO Does it make sense that Common has modules?
    public static class CommonModules
    {
        public static void RegisterAll(ContainerBuilder containerBuilder)
        {
            // Register common components
            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<FileSystem.FileSystem>().As<IFileSystem>().SingleInstance();
        }
    }
}