using Autofac;
using Geisha.Editor.CreateSoundAsset.Model;
using Geisha.Editor.CreateSoundAsset.UserInterface;

namespace Geisha.Editor.CreateSoundAsset
{
    internal sealed class CreateSoundAssetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateSoundAssetService>().As<ICreateSoundAssetService>().SingleInstance();
            builder.RegisterType<CreateSoundAssetCommandFactory>().As<ICreateSoundAssetCommandFactory>().SingleInstance();
        }
    }
}