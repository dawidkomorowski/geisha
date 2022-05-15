using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Systems;

namespace Geisha.Engine.Input
{
    internal sealed class InputModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<InputMappingAssetLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            builder.RegisterType<InputComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<InputSystem>().As<IInputGameLoopStep>().As<ISceneObserver>().SingleInstance();
        }
    }
}