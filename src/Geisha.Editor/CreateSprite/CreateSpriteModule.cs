using Autofac;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.CreateSprite.UserInterface;

namespace Geisha.Editor.CreateSprite
{
    public class CreateSpriteModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateSpriteService>().As<ICreateSpriteService>().SingleInstance();
            builder.RegisterType<CreateSpriteCommandFactory>().As<ICreateSpriteCommandFactory>().SingleInstance();
        }
    }
}