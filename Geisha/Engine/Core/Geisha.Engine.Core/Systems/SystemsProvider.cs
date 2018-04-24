using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Logging;

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
        private static readonly ILog Log = LogFactory.Create(typeof(SystemsProvider));
        private readonly IEnumerable<IFixedTimeStepSystem> _fixedTimeStepSystems;
        private readonly IEnumerable<IVariableTimeStepSystem> _variableTimeStepSystems;

        [ImportingConstructor]
        public SystemsProvider([ImportMany] IEnumerable<IFixedTimeStepSystem> fixedTimeStepSystems,
            [ImportMany] IEnumerable<IVariableTimeStepSystem> variableTimeStepSystems)
        {
            _fixedTimeStepSystems = fixedTimeStepSystems;
            _variableTimeStepSystems = variableTimeStepSystems;

            Log.Info("Discovering fixed time step systems...");

            foreach (var fixedTimeStepSystem in _fixedTimeStepSystems)
            {
                Log.Info($"Fixed time step system found: {fixedTimeStepSystem}");
            }

            Log.Info("Fixed time step systems discovery completed.");

            Log.Info("Discovering variable time step systems...");

            foreach (var variableTimeStepSystem in _variableTimeStepSystems)
            {
                Log.Info($"Variable time step system found: {variableTimeStepSystem}");
            }

            Log.Info("Variable time step systems discovery completed.");
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