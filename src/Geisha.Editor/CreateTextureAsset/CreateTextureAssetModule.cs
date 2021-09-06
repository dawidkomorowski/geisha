using Autofac;
using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Editor.CreateTextureAsset.UserInterface;

namespace Geisha.Editor.CreateTextureAsset
{
    internal sealed class CreateTextureAssetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateTextureAssetService>().As<ICreateTextureAssetService>().SingleInstance();
            builder.RegisterType<CreateTextureAssetCommandFactory>().As<ICreateTextureAssetCommandFactory>().SingleInstance();
        }
    }
}