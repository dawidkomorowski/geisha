using Autofac;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Animation.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Animation
{
    internal sealed class AnimationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SpriteAnimationAssetLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            builder.RegisterType<SpriteAnimationComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<AnimationSystem>().As<IAnimationGameLoopStep>().As<ISceneObserver>().SingleInstance();
        }
    }
}