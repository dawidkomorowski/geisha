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

        public IList<ISystem> GetVariableUpdateSystems()
        {
            return GetSystems()
                .Where(s => s.UpdateMode == UpdateMode.Variable || s.UpdateMode == UpdateMode.Both).ToList();
        }

        public IList<ISystem> GetFixedUpdateSystems()
        {
            return GetSystems()
                .Where(s => s.UpdateMode == UpdateMode.Fixed || s.UpdateMode == UpdateMode.Both).ToList();
        }

        private IEnumerable<ISystem> GetSystems()
        {
            var orderedSystems = _systems.OrderBy(s => s.Priority).ToList();
            return orderedSystems;
        }
    }
}