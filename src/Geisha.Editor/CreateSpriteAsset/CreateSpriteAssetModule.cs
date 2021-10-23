using Autofac;
using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Editor.CreateSpriteAsset.UserInterface;

namespace Geisha.Editor.CreateSpriteAsset
{
    internal sealed class CreateSpriteAssetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateSpriteAssetService>().As<ICreateSpriteAssetService>().SingleInstance();
            builder.RegisterType<CreateSpriteAssetCommandFactory>().As<ICreateSpriteAssetCommandFactory>().SingleInstance();
        }
    }
}