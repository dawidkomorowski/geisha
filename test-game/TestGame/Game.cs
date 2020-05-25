using Autofac;
using Geisha.Engine;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace TestGame
{
    public sealed class Game : IGame
    {
        public string WindowTitle => "Test Game";

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TestSystem>().As<ICustomSystem>().SingleInstance();
            containerBuilder.RegisterType<TestConstructionScript>().As<ISceneConstructionScript>().SingleInstance();
        }
    }
}