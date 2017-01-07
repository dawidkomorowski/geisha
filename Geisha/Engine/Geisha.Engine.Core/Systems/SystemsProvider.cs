using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(ISystemsProvider))]
    public class SystemsProvider : ISystemsProvider
    {
        private readonly IEnumerable<ISystem> _systems;

        [ImportingConstructor]
        public SystemsProvider([ImportMany] IEnumerable<ISystem> systems)
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