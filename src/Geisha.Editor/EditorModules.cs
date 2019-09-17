using Autofac;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling;

namespace Geisha.Editor
{
    public static class EditorModules
    {
        public static void RegisterAll(ContainerBuilder containerBuilder)
        {
            // Register editor Main viewmodel
            containerBuilder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            // Register editor modules
            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<ProjectHandlingModule>();
        }
    }
}