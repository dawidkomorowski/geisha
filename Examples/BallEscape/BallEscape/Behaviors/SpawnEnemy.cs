using Geisha.Engine.Core.Components;

namespace BallEscape.Behaviors
{
    public class SpawnEnemy : Behavior
    {
        private readonly PrefabFactory _prefabFactory;
        private const int SpawnTickInterval = 240; // every 4 seconds

        private int _currentTick = 0;

        public SpawnEnemy(PrefabFactory prefabFactory)
        {
            _prefabFactory = prefabFactory;
        }

        public override void OnFixedUpdate()
        {
            if (_currentTick >= SpawnTickInterval)
            {
                var enemy = _prefabFactory.CreateEnemy();
                enemy.GetComponent<Transform>().Translation = Entity.GetComponent<Transform>().Translation;
                Entity.Scene.AddEntity(enemy);

                _currentTick = 0;
            }

            _currentTick++;
        }
    }
}