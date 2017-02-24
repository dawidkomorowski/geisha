using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

        public IEnumerable<ISystem> GetSystems()
        {
            var orderedSystems = _systems.OrderBy(s => s.Priority).ToList();
            return orderedSystems;
        }
    }
}