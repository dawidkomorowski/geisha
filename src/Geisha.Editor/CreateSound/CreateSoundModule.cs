using Autofac;
using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.CreateSound.UserInterface;

namespace Geisha.Editor.CreateSound
{
    internal sealed class CreateSoundModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateSoundService>().As<ICreateSoundService>().SingleInstance();
            builder.RegisterType<CreateSoundCommandFactory>().As<ICreateSoundCommandFactory>().SingleInstance();
        }
    }
}