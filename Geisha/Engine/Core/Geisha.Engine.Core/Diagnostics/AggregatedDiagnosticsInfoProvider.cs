using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Aggregates all <see cref="IDiagnosticsInfoProvider" /> implementations and provides single point of querying for
    ///     all available <see cref="DiagnosticsInfo" />.
    /// </summary>
    public interface IAggregatedDiagnosticsInfoProvider
    {
        IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo();
    }

    [Export(typeof(IAggregatedDiagnosticsInfoProvider))]
    public class AggregatedDiagnosticsInfoProvider : IAggregatedDiagnosticsInfoProvider
    {
        private readonly IEnumerable<IDiagnosticsInfoProvider> _diagnosticsInfoProviders;

        [ImportingConstructor]
        public AggregatedDiagnosticsInfoProvider([ImportMany] IEnumerable<IDiagnosticsInfoProvider> diagnosticsInfoProviders)
        {
            _diagnosticsInfoProviders = diagnosticsInfoProviders;
        }

        public IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo()
        {
            return _diagnosticsInfoProviders.SelectMany(p => p.GetDiagnosticsInfo());
        }
    }
}