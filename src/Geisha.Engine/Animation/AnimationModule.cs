using Autofac;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Animation
{
    public sealed class AnimationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SpriteAnimationAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
        }
    }
}