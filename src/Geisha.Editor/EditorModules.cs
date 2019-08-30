using Autofac;
using Geisha.Common.Serialization;
using Geisha.Editor.Core;

namespace Geisha.Editor
{
    public static class EditorModules
    {
        public static void RegisterAll(ContainerBuilder containerBuilder)
        {
            // Register common components
            containerBuilder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();

            // Register editor Main viewmodel
            containerBuilder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            // Register editor modules
            containerBuilder.RegisterModule<CoreModule>();
        }
    }
}