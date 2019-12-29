using Autofac;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.CreateTexture.UserInterface;

namespace Geisha.Editor.CreateTexture
{
    internal sealed class CreateTextureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateTextureService>().As<ICreateTextureService>().SingleInstance();
            builder.RegisterType<CreateTextureCommandFactory>().As<ICreateTextureCommandFactory>().SingleInstance();
        }
    }
}