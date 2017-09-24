using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystemsProvider
    {
        IList<IVariableTimeStepSystem> GetVariableTimeStepSystems();
        IList<IFixedTimeStepSystem> GetFixedTimeStepSystems();
    }

    [Export(typeof(ISystemsProvider))]
    public class SystemsProvider : ISystemsProvider
    {
        private readonly IEnumerable<IFixedTimeStepSystem> _fixedTimeStepSystems;
        private readonly IEnumerable<IVariableTimeStepSystem> _variableTimeStepSystems;

        [ImportingConstructor]
        public SystemsProvider([ImportMany] IEnumerable<IFixedTimeStepSystem> fixedTimeStepSystems,
            [ImportMany] IEnumerable<IVariableTimeStepSystem> variableTimeStepSystems)
        {
            _fixedTimeStepSystems = fixedTimeStepSystems;
            _variableTimeStepSystems = variableTimeStepSystems;
        }

        public IList<IVariableTimeStepSystem> GetVariableTimeStepSystems()
        {
            return _variableTimeStepSystems.OrderBy(s => s.Priority).ToList();
        }

        public IList<IFixedTimeStepSystem> GetFixedTimeStepSystems()
        {
            return _fixedTimeStepSystems.OrderBy(s => s.Priority).ToList();
        }
    }
}