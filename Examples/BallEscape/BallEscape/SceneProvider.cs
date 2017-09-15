using System.ComponentModel.Composition;
using Geisha.Engine.Core.SceneModel;

namespace BallEscape
{
    [Export(typeof(ITestSceneProvider))]
    public class SceneProvider : ITestSceneProvider
    {
        private readonly PrefabFactory _prefabFactory;

        [ImportingConstructor]
        public SceneProvider(PrefabFactory prefabFactory)
        {
            _prefabFactory = prefabFactory;
        }

        public Scene GetTestScene()
        {
            var scene = new Scene();

            var camera = _prefabFactory.CreateCamera();
            var world = _prefabFactory.CreateWorld();
            var player = _prefabFactory.CreatePlayer();

            scene.AddEntity(camera);
            scene.AddEntity(world);
            scene.AddEntity(player);

            scene.AddEntity(_prefabFactory.CreateEnemySpawnPoint(-500, -300));
            scene.AddEntity(_prefabFactory.CreateEnemySpawnPoint(500, 100));
            scene.AddEntity(_prefabFactory.CreateEnemySpawnPoint(-350, 100));

            scene.AddEntity(_prefabFactory.CreateHole(-200, -100));
            scene.AddEntity(_prefabFactory.CreateHole(-200, 150));
            scene.AddEntity(_prefabFactory.CreateHole(300, -200));
            scene.AddEntity(_prefabFactory.CreateHole(400, 200));

            return scene;
        }
    }
}