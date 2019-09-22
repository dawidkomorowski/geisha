using Autofac;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor
{
    internal sealed class SceneEditorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEmptySceneService>().As<ICreateEmptySceneService>().SingleInstance();
        }
    }
}