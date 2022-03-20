using Geisha.Engine.Core.SceneModel;
using NSubstitute;

namespace Geisha.TestUtils
{
    public static class TestSceneFactory
    {
        public static Scene Create()
        {
            return new Scene(Substitute.For<IComponentFactoryProvider>());
        }
    }
}