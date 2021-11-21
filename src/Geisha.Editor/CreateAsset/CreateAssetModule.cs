using Autofac;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.CreateAsset.UserInterface.Sound;
using Geisha.Editor.CreateAsset.UserInterface.Texture;

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

            // Texture
            builder.RegisterType<CreateTextureAssetService>().As<ICreateTextureAssetService>().SingleInstance();
            builder.RegisterType<CreateTextureAssetCommandFactory>().As<ICreateTextureAssetCommandFactory>().SingleInstance();
        }
    }
}