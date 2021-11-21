using Autofac;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.CreateAsset.UserInterface.Sound;

namespace Geisha.Editor.CreateAsset
{
    internal sealed class CreateAssetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Sound
            builder.RegisterType<CreateSoundAssetService>().As<ICreateSoundAssetService>().SingleInstance();
            builder.RegisterType<AssetToolCreateSoundAsset>().As<IAssetToolCreateSoundAsset>().SingleInstance();
            builder.RegisterType<CreateSoundAssetCommandFactory>().As<ICreateSoundAssetCommandFactory>().SingleInstance();
        }
    }
}