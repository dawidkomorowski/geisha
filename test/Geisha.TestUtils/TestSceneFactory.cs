using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.TestUtils
{
    public static class TestSceneFactory
    {
        public static Scene Create()
        {
            return new Scene(Enumerable.Empty<ISceneBehaviorFactory>());
        }

        public static Scene Create(IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            return new Scene(sceneBehaviorFactories);
        }
    }
}