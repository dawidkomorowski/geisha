using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public class SystemsProvider : ISystemsProvider
    {
        private readonly IList<ISystem> _systems;

        public SystemsProvider(IList<ISystem> systems)
        {
            _systems = systems;
        }

        public IUpdatable GetSystemsUpdatableForScene(Scene scene)
        {
            foreach (var system in _systems)
            {
                system.Scene = scene;
            }

            var orderedSystems = _systems.OrderBy(s => s.Priority).ToList();

            return new UpdateList(orderedSystems);
        }
    }
}