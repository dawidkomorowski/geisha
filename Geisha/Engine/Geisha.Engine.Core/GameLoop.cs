using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    public class GameLoop
    {
        private readonly ISystemsProvider _systemsProvider;
        private readonly IDeltaTimeProvider _deltaTimeProvider;

        public GameLoop(ISystemsProvider systemsProvider, IDeltaTimeProvider deltaTimeProvider)
        {
            _systemsProvider = systemsProvider;
            _deltaTimeProvider = deltaTimeProvider;
        }

        public void Step()
        {
            var deltaTime = _deltaTimeProvider.GetDeltaTime();
            var systems = _systemsProvider.GetSystemsUpdatableForScene(null);
            systems.Update(deltaTime);
        }
    }
}