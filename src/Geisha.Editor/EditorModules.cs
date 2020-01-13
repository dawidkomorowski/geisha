using Autofac;
using Geisha.Editor.Core;
using Geisha.Editor.CreateSound;
using Geisha.Editor.CreateSprite;
using Geisha.Editor.CreateTexture;
using Geisha.Editor.ProjectHandling;
using Geisha.Editor.SceneEditor;

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
            containerBuilder.RegisterModule<CreateSoundModule>();
            containerBuilder.RegisterModule<CreateSpriteModule>();
            containerBuilder.RegisterModule<CreateTextureModule>();
            containerBuilder.RegisterModule<ProjectHandlingModule>();
            containerBuilder.RegisterModule<SceneEditorModule>();
        }
    }
}