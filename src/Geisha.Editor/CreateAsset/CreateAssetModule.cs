using Autofac;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.CreateAsset.UserInterface.Sound;
using Geisha.Editor.CreateAsset.UserInterface.Sprite;
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

            // Sprite
            builder.RegisterType<CreateSpriteAssetService>().As<ICreateSpriteAssetService>().SingleInstance();
            builder.RegisterType<AssetToolCreateSpriteAsset>().As<IAssetToolCreateSpriteAsset>().SingleInstance();
            builder.RegisterType<CreateSpriteAssetCommandFactory>().As<ICreateSpriteAssetCommandFactory>().SingleInstance();

            // Texture
            builder.RegisterType<CreateTextureAssetService>().As<ICreateTextureAssetService>().SingleInstance();
            builder.RegisterType<AssetToolCreateTextureAsset>().As<IAssetToolCreateTextureAsset>().SingleInstance();
            builder.RegisterType<CreateTextureAssetCommandFactory>().As<ICreateTextureAssetCommandFactory>().SingleInstance();
        }
    }
}