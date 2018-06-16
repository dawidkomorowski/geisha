using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Geisha.Common.Logging;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystemsProvider
    {
        IEnumerable<IVariableTimeStepSystem> GetVariableTimeStepSystems();
        IEnumerable<IFixedTimeStepSystem> GetFixedTimeStepSystems();
    }

    [Export(typeof(ISystemsProvider))]
    public class SystemsProvider : ISystemsProvider
    {
        private static readonly ILog Log = LogFactory.Create(typeof(SystemsProvider));
        private readonly IEnumerable<IFixedTimeStepSystem> _fixedTimeStepSystems;
        private readonly IEnumerable<IVariableTimeStepSystem> _variableTimeStepSystems;

        [ImportingConstructor]
        public SystemsProvider(IConfigurationManager configurationManager,
            [ImportMany] IEnumerable<IFixedTimeStepSystem> fixedTimeStepSystems, [ImportMany] IEnumerable<IVariableTimeStepSystem> variableTimeStepSystems)
        {
            var systemsExecutionChain = configurationManager.GetConfiguration<CoreConfiguration>().SystemsExecutionChain;

            _fixedTimeStepSystems = fixedTimeStepSystems;
            _variableTimeStepSystems = variableTimeStepSystems;

            // Systems discovery
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

            // Systems execution chain preparation
            Log.Info("Preparing systems execution chain...");

            _fixedTimeStepSystems =
                systemsExecutionChain.SelectMany(systemFullName => _fixedTimeStepSystems.Where(s => s.GetType().FullName == systemFullName)).ToList();

            _variableTimeStepSystems = systemsExecutionChain
                .SelectMany(systemFullName => _variableTimeStepSystems.Where(s => s.GetType().FullName == systemFullName)).ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Fixed time step systems execution chain:");
            foreach (var fixedTimeStepSystem in _fixedTimeStepSystems)
            {
                stringBuilder.AppendLine(fixedTimeStepSystem.GetType().FullName);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Variable time step systems execution chain:");
            foreach (var variableTimeStepSystem in _variableTimeStepSystems)
            {
                stringBuilder.AppendLine(variableTimeStepSystem.GetType().FullName);
            }

            Log.Info(stringBuilder.ToString());
        }

        public IEnumerable<IVariableTimeStepSystem> GetVariableTimeStepSystems()
        {
            return _variableTimeStepSystems;
        }

        public IEnumerable<IFixedTimeStepSystem> GetFixedTimeStepSystems()
        {
            return _fixedTimeStepSystems;
        }
    }
}